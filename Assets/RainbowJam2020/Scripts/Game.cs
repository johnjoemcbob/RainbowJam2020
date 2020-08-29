using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	public static Game Instance;

	public const int CHARACTERS = 8;
	public const string GENERIC_ECHO = "That's (that's) my (my) own (own) channel! (-channel)";
	public const string GENERIC_WRONG = "That's not what I asked for...";

	public static bool SoundsEnabled = false;

	#region Structs
	struct CharacterState
	{
		public List<string> Messages;
		public string Name;
		public bool PortraitUnlocked;
	}
	#endregion

	#region Variables - Public
	[Header( "Variables" )]
	public int Stage = 0;
	public bool Debug = false;

	[Header( "References" )]
	public GameObject EndScreen;
	#endregion

	#region Variables - Private
	private Dictionary<int, TextAsset> CharacterStories;
	private List<CharacterState> CharacterStates = new List<CharacterState>();
	private List<CharacterState> StageCharacterStates = new List<CharacterState>();
	[HideInInspector]
	public int CurrentStory = -1;

	private float WaitingForEnd = -1;
	private bool Won = false;
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;

		for ( int character = 0; character < CHARACTERS; character++ )
		{
			var state = new CharacterState();
				state.Messages = new List<string>();
				state.Messages.Add( "Stage 1" );
			CharacterStates.Add( state );
		}
		StartStage( Stage );

		SoundsEnabled = true;
	}

	private void Update()
	{
		if ( Application.isEditor )
		{
			if ( Input.GetKeyDown( KeyCode.P ) )
			{
				NextStage();
			}
		}
	}
	#endregion

	#region Stage
	public void NextStage()
	{
		if ( IsNextStageValid() )
		{
			ShallowCopyCharacterStates( CharacterStates, StageCharacterStates );

			// Add stage headers to space message logs better
			for ( int character = 0; character < CHARACTERS; character++ )
			{
				var state = CharacterStates[character];
				{
					state.Messages.Add( "" );
					state.Messages.Add( "Stage " + ( Stage + 1 ) );
				}
				CharacterStates.Add( state );
			}

			StartStage( Stage + 1 );
		}
		else
		{
			StartCoroutine( ShowEnd() );
		}
		StartTransition();
	}

	IEnumerator ShowEnd()
	{
		// This handles fade out/in by itself
		Music.Instance.SetTrack( 1 );

		Fader.Instance.GetComponentInChildren<Text>().text = "Victory!";

		yield return new WaitForSeconds( 1 );

		EndScreen.SetActive( true );
	}

	bool IsNextStageValid()
	{
		var stage = Stage + 1;
		for ( int character = 1; character <= CHARACTERS; character++ )
		{
			Object res = Resources.Load( "Narrative/Stages/" + stage + "/" + character );
			if ( res )
			{
				return true;
			}
		}
		return false;
	}

	public void ResetStage()
	{
		StartStage( Stage );
		StartTransition();
	}

	void StartStage( int stage )
	{
		Stage = stage;

		SoundsEnabled = false;

		ShallowCopyCharacterStates( StageCharacterStates, CharacterStates );

		// Find all characters for this stage
		CharacterStories = new Dictionary<int, TextAsset>();
		for ( int character = 1; character <= CHARACTERS; character++ )
		{
			Object res = Resources.Load( "Narrative/Stages/" + Stage + "/" + character );
			if ( res )
			{
				CharacterStories.Add( character, res as TextAsset );
			}
		}

		// Reset any wires
		if ( Wire.CurrentHeld )
		{
			Wire.CurrentHeld.Drop( true );
		}
		Wire.UnPortAll();

		// Reset dialogue
		InkHandler.Instance.Reset();
		DialogueMover.Instance.Hide();

		// Light correct switches
		WaitingForEnd = -1;
		Switch.LightSwitches( CharacterStories );

		SoundsEnabled = true;
	}

	public void TryAdvanceFinishCharacter()
	{
		if ( WaitingForEnd != -1 )
		{
			if ( Won )
			{
				if ( CurrentStory != -1 )
				{
					// Unlight
					Switch.Get( CurrentStory - 1 ).SetLit( false );

					// Check for stage complete
					if ( Switch.CountLit() == 0 )
					{
						NextStage();
					}
				}
			}
			else
			{
				// Reset stage
				ResetStage();
			}

			Won = false;
			WaitingForEnd = -1;
		}
	}
	#endregion

	#region Transitions
	void StartTransition()
	{
		Fader.Instance.StartTransition();
	}
	#endregion

	#region Character State
	public void AddMessageReceived( string msg )
	{
		if ( CurrentStory == -1 ) return;

		var character = CurrentStory - 1;

		if ( StageCharacterStates[character].Messages == null )
		{
			CharacterState state = StageCharacterStates[character];
				state.Messages = new List<string>();
			StageCharacterStates[character] = state;
		}
		if ( !StageCharacterStates[character].Messages.Contains( msg ) && msg != GENERIC_ECHO && msg != GENERIC_WRONG )
		{
			StageCharacterStates[character].Messages.Add( msg );
		}
	}

	public bool HasMessageReceived( string msg )
	{
		if ( CurrentStory == -1 ) return false;
		return StageCharacterStates[CurrentStory - 1].Messages.Contains( msg );
	}

	public List<string> GetCurrentMessageLog()
	{
		if ( CurrentStory != -1 )
		{
			return StageCharacterStates[CurrentStory - 1].Messages;
		}
		return new List<string>();
	}

	void ShallowCopyCharacterStates( List<CharacterState> to, List<CharacterState> from )
	{
		to.Clear();
		foreach ( var character in from )
		{
			CharacterState state = new CharacterState();
			{
				state.Messages = new List<string>();
				if ( character.Messages != null )
				{
					foreach ( var msg in character.Messages )
					{
						state.Messages.Add( msg );
					}
				}
				state.Name = character.Name;
				state.PortraitUnlocked = character.PortraitUnlocked;
			}
			to.Add( state );
		}
	}

	public void SetCharacterName( int character, string name )
	{
		var state = StageCharacterStates[character];
		{
			state.Name = name;
		}
		StageCharacterStates[character] = state;

		// Update visuals if currently active
		if ( CurrentStory - 1 == character )
		{
			CharacterNameText.Instance.Set( Game.Instance.GetCharacterName( CurrentStory - 1 ) );
		}
	}

	public string GetCharacterName( int character )
	{
		return StageCharacterStates[character].Name;
	}

	public void SetCharacterPortrait( int character, bool portrait )
	{
		var state = StageCharacterStates[character];
		{
			state.PortraitUnlocked = portrait;
		}
		StageCharacterStates[character] = state;

		// Update visuals if currently active
		if ( CurrentStory - 1 == character )
		{
			PortraitUpdater.Instance.SetPortrait( CurrentStory );
		}
	}

	public bool GetCharacterPortrait( int character )
	{
		return StageCharacterStates[character].PortraitUnlocked;
	}
	#endregion

	#region Callbacks
	public void OnSwitchStateChanged( int swtch, bool press )
	{
		if ( press )
		{
			CurrentStory = swtch;

			// Link character story to the current narrative and start
			InkHandler.Instance.inkJSONAsset = CharacterStories[swtch];
			InkHandler.Instance.StartStory();
		}
		else
		{
			InkHandler.Instance.StopStory();
		}
	}

	public void OnSwitchOutcome( bool win )
	{
		WaitingForEnd = Time.time;
		Won = win;
	}
	#endregion

	public bool IsBlocked()
	{
		return MessageLog.Instance.gameObject.activeSelf || EndScreen.activeSelf;
	}
}
