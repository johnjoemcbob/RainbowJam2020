using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	public static Game Instance;

	public const int CHARACTERS = 8;
	public static string[] CharacterNames = new string[CHARACTERS];

	#region Structs
	struct CharacterState
	{
		public List<string> Messages;
		public bool PortraitUnlocked;
		public bool NameUnlocked;
	}
	#endregion

	#region Variables - Public
	[Header( "Varialbes" )]
	public int Stage = 0;

	[Header( "References" )]
	public GameObject thing;
	#endregion

	#region Variables - Private
	private Dictionary<int, TextAsset> CharacterStories;
	private List<CharacterState> CharacterStates = new List<CharacterState>();
	private List<CharacterState> StageCharacterStates = new List<CharacterState>();
	private int CurrentStory = -1;

	private float WaitingForEnd = -1;
	private bool Won = false;
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;

		for ( int character = 0; character < CHARACTERS; character++ )
		{
			CharacterStates.Add( new CharacterState() );
		}
	}

	void Start()
    {
		StartStage( Stage );
    }
	#endregion

	#region Stage
	public void NextStage()
	{
		CharacterStates = StageCharacterStates;

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

		ShallowCopyCharacterStates();

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

		// Reset any wires
		if ( Wire.CurrentHeld )
		{
			Wire.CurrentHeld.Drop( true );
		}
	}

	public void TryAdvanceFinishCharacter()
	{
		if ( WaitingForEnd != -1 )
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
		StageCharacterStates[character].Messages.Add( msg );
	}

	void ShallowCopyCharacterStates()
	{
		StageCharacterStates.Clear();
		foreach ( var character in CharacterStates )
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
				state.NameUnlocked = character.NameUnlocked;
				state.PortraitUnlocked = character.PortraitUnlocked;
			}
			StageCharacterStates.Add( state );
		}
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
