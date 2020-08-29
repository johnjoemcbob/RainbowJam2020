using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;

public class InkHandler : MonoBehaviour
{
	public static InkHandler Instance;

	public static event Action<Story> OnCreateStory;

	[Header( "References" )]
	[SerializeField]
	private Canvas canvas = null;
	public Transform UIParent;
	// UI Prefabs
	[SerializeField]
	private Text textPrefab = null;
	[SerializeField]
	private Button buttonPrefab = null;

	[Header( "Assets" )]
	[SerializeField]
	public TextAsset inkJSONAsset = null;
	public Story story;

	private Coroutine Refreshing;
	[HideInInspector]
	public bool WaitingForMore = false;
	private bool ForceNewLine = false;
	List<string> PreparedLines = new List<string>();

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		// Remove the default message
		RemoveChildren();
		StartStory();
	}
	#endregion

	#region Story
	public void StartStory()
	{
		story = new Story( inkJSONAsset.text );
		if ( OnCreateStory != null ) OnCreateStory( story );
		PreparedLines.Clear();
		RefreshView();

		// Check variables
		if ( story.variablesState.GlobalVariableExistsWithName( "win" ) )
		{
			// Check for win in no options dialogue
			if ( story.variablesState.GetVariableWithName( "win" ).ToString() == "1" )
			{
				Game.Instance.OnSwitchOutcome( true );
			}

			story.ObserveVariable( "win", EndObserver );
			story.ObserveVariable( "lose", EndObserver );
		}
		if ( story.variablesState.GlobalVariableExistsWithName( "character1" ) )
		{
			for ( int cha = 0; cha < Game.CHARACTERS; cha++ )
			{
				var variable = "character" + ( cha + 1 );
				var name = story.variablesState.GetVariableWithName( variable ).ToString();
				Game.Instance.SetCharacterName( cha, name );
				story.ObserveVariable( variable, NameObserver );
			}
		}
		if ( story.variablesState.GlobalVariableExistsWithName( "portrait1" ) )
		{
			for ( int cha = 0; cha < Game.CHARACTERS; cha++ )
			{
				var variable = "portrait" + ( cha + 1 );
				var portrait = story.variablesState.GetVariableWithName( variable ).ToString() == "1";
				Game.Instance.SetCharacterPortrait( cha, portrait );
				story.ObserveVariable( variable, PortraitObserver );
			}
		}
	}

	public void StopStory()
	{
		story = null;

		DialogueMover.Instance.Hide();
	}

	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView()
	{
		if ( Refreshing == null )
		{
			Refreshing = StartCoroutine( Co_RefreshView() );
		}
	}

	IEnumerator Co_RefreshView()
	{
		DialogueMover.Instance.Hide();

		PrepareNextStorySegment();

		yield return new WaitForSeconds( 5 / DialogueMover.Instance.Speed );

		if ( story )
		{
			RefreshView_Func();

			//yield return new WaitForSeconds( 1 );

			DialogueMover.Instance.Show();
		}

		Refreshing = null;
	}

	void RefreshView_Func()
	{
		// Remove all the UI on screen
		RemoveChildren();

		var max = 40;

		WaitingForMore = false;
		bool hasprevious = false;
		// Read all the content until we can't continue any more
		int lines = 0;
		while ( PreparedLines.Count > 0 )
		{
			// Only 3 at a time
			lines++;
			if ( lines > 3 )
			{
				WaitingForMore = true;
				break;
			}

			// Continue gets the next line of the story
			string text = PreparedLines[0];

			// Replace any weird characters
			text = text.Replace( "…", "..." );
			text = text.Replace( "‘", "'" );

			// Check valid
			if ( text.Length > max )
			{
				var diff = text.Length - max;

				// Find last space before this
				string cut = text.Substring( 0, max );
				int space = cut.LastIndexOf( ' ' );
				string leftover = text.Substring( space + 1 );
				if ( PreparedLines.Count > 1 )
				{
					PreparedLines[1] = leftover + " " + PreparedLines[1];
				}
				else
				{
					PreparedLines.Add( leftover );
				}
				text = text.Substring( 0, space );
			}

			// Display the text on screen!
			CreateContentView( text );
			if ( Game.Instance.HasMessageReceived( text ) )
			{
				hasprevious = true;
			}
			Game.Instance.AddMessageReceived( text );

			PreparedLines.RemoveAt( 0 );
		}
		// Force 3 lines always for layout purposes
		while ( lines < 3 )
		{
			CreateContentView( "" );
			lines++;
		}

		// If this dialogue is being viewed again, just skip to the end!
		if ( WaitingForMore && hasprevious )
		{
			RefreshView_Func();
		}
	}

	void PrepareNextStorySegment()
	{
		while ( story && story.canContinue )
		{
			// Continue gets the next line of the story
			string text = story.Continue();
			// This removes any white space from the text.
			text = text.Trim();
			// . signifies a blank line
			if ( text == "." )
			{
				text = "";
			}

			PreparedLines.Add( text );
		}
	}

	void CreateContentView( string text )
	{
		Text storyText = Instantiate (textPrefab) as Text;
		storyText.text = text;
		storyText.transform.SetParent( UIParent.transform, false );
	}

	void RemoveChildren()
	{
		int childCount = UIParent.transform.childCount;
		for ( int i = childCount - 1; i >= 0; --i )
		{
			GameObject.Destroy( UIParent.transform.GetChild( i ).gameObject );
		}
	}

	public bool ChooseChoice( int choice )
	{
		var index = GetChoiceIndex( choice );
		if ( index >= 0 && index < story.currentChoices.Count )
		{
			story.ChooseChoiceIndex( index );
			PreparedLines.Clear();
			RefreshView();
			return true;
		}
		else
		{
			PreparedLines.Clear();
			if ( choice == Game.Instance.CurrentStory )
			{
				PreparedLines.Add( Game.GENERIC_ECHO );
			}
			else
			{
				PreparedLines.Add( Game.GENERIC_WRONG );
			}
			RefreshView();
			return true;
		}
	}

	public int GetChoiceIndex( int choice )
	{
		int index = -1;
		{
			for ( int i = 0; i < story.currentChoices.Count; i++ )
			{
				if ( story.currentChoices[i].text == choice.ToString() )
				{
					index = i;
					break;
				}
			}
		}
		return index;
	}

	public bool ContainsChoice( int choice )
	{
		return ( GetChoiceIndex( choice ) != -1 );
	}

	public bool TryAdvance()
	{
		if ( WaitingForMore )
		{
			RefreshView();
			return true;
		}
		else
		{
			DialogueMover.Instance.Hide();
		}
		return false;
	}
	#endregion

	#region Variables
	public void EndObserver( string variableName, object newValue )
	{
		Game.Instance.OnSwitchOutcome( variableName == "win" );
	}

	public void NameObserver( string variableName, object newValue )
	{
		string name = newValue.ToString();
		var cha = int.Parse( variableName.Replace( "character", "" ) );
		Game.Instance.SetCharacterName( cha - 1, name );
	}

	public void PortraitObserver( string variableName, object newValue )
	{
		var portrait = newValue.ToString() == "1";
		var cha = int.Parse( variableName.Replace( "portrait", "" ) );
		Game.Instance.SetCharacterPortrait( cha - 1, portrait );
	}
	#endregion

	public void Reset()
	{
		WaitingForMore = false;
		PreparedLines.Clear();

		// Stop coroutine
		if ( Refreshing != null )
		{
			StopCoroutine( Refreshing );
			Refreshing = null;
		}

		PortraitUpdater.Instance.SetPortrait( -1 );
	}
}
