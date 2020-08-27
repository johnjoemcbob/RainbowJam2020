using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	public static Game Instance;

	public const int CHARACTERS = 8;

	#region Structs
	struct CharacterState
	{
		public List<string> Messages;
		public string Name;
		public bool PortraitUnlocked;
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
			CharacterStates.Add( new CharacterState() );
		}
		StartStage( Stage );
	}
	#endregion

	#region Stage
	public void NextStage()
	{
		ShallowCopyCharacterStates( CharacterStates, StageCharacterStates );

		StartStage( Stage + 1 );
		StartTransition();
	}

	public void ResetStage()
	{
		StartStage( Stage );
		StartTransition();
	}

	void StartStage( int stage )
	{
		Stage = stage;

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
}
