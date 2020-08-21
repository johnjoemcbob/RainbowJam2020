using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	public static Game Instance;

	public int Stage = 0;

	private Dictionary<int, TextAsset> CharacterStories;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
    {
		StartStage( Stage );
    }

	public void NextStage()
	{
		StartStage( Stage++ );
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

	public void OnSwitchStateChanged( int swtch, bool press )
	{
		if ( press )
		{
			// Link character story to the current narrative and start
			InkHandler.Instance.inkJSONAsset = CharacterStories[swtch];
			InkHandler.Instance.StartStory();
		}
		else
		{
			InkHandler.Instance.StopStory();
		}
	}
}
