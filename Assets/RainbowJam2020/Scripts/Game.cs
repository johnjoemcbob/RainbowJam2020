using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	public static Game Instance;

	#region Variables - Public
	[Header( "Varialbes" )]
	public int Stage = 0;

	[Header( "References" )]
	public GameObject thing;
	#endregion

	#region Variables - Private
	private Dictionary<int, TextAsset> CharacterStories;
	private int CurrentStory = -1;

	private float WaitingForEnd = -1;
	private bool Won = false;
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;
	}

	void Start()
    {
		StartStage( Stage );
    }

	private void Update()
	{
		if ( WaitingForEnd != -1 )
		{
			if ( ( Time.time - WaitingForEnd ) >= 0.5f && Input.GetMouseButtonDown( 0 ) )
			{
				if ( Won )
				{
					// Unlight
					Switch.Get( CurrentStory - 1 ).SetLit( false );

					// Check for stage complete
					if ( Switch.CountLit() == 0 )
					{
						NextStage();
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
	}
	#endregion

	#region Stage
	public void NextStage()
	{
		StartStage( Stage + 1 );
		StartTransition();
	}

	public void ResetStage()
	{
		StartStage( Stage );
		DialogueMover.Instance.Hide();
		StartTransition();
	}

	void StartStage( int stage )
	{
		Stage = stage;

		// Find all characters for this stage
		CharacterStories = new Dictionary<int, TextAsset>();
		for ( int character = 1; character < 9; character++ )
		{
			Object res = Resources.Load( "Narrative/Stages/" + Stage + "/" + character );
			if ( res )
			{
				CharacterStories.Add( character, res as TextAsset );
			}
		}

		// Light correct switches
		Switch.LightSwitches( CharacterStories );
	}
	#endregion

	#region Transitions
	void StartTransition()
	{
		Fader.Instance.StartTransition();
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
}
