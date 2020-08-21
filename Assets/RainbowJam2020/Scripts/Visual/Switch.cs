using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Switch : MonoBehaviour
{
	[Header( "Assets" )]
	public Sprite[] UpSprites;
	public Sprite[] UpSprites_Lit;
	public Sprite[] DownSprites;
	public Sprite[] DownSprites_Lit;

	[HideInInspector]
	public bool Pressed = false;
	[HideInInspector]
	public bool Lit = false;
	[HideInInspector]
	public Wire Wire;

	private int Number = -1;
	private static Switch CurrentSwitch;

	private void Awake()
	{
		UpdateSprite();

		Number = transform.GetSiblingIndex() + 1;
	}

	void UpdateSprite()
	{
		// Get sprite array based on current state
		var array = UpSprites;
		if ( Pressed )
		{
			if ( Lit )
			{
				array = DownSprites_Lit;
			}
			else
			{
				array = DownSprites;
			}
		}
		else
		{
			if ( Lit )
			{
				array = UpSprites_Lit;
			}
			else
			{
				array = UpSprites;
			}
		}

		// And get the correct sprite for this switch index
		var index = transform.GetSiblingIndex();
		GetComponentInChildren<SpriteRenderer>().sprite = array[index];
	}

	public void SetPressed( bool press )
	{
		if ( Lit )
		{
			// Remove from port if inside
			if ( !press && Wire.Port != null )
			{
				Wire.TryPickup( true );
				Wire.TryDrop();
			}
			// Only one at a time!
			if ( press )
			{
				if ( CurrentSwitch != null )
				{
					CurrentSwitch.SetPressed( false );
				}
				CurrentSwitch = this;
			}
			else
			{
				CurrentSwitch = null;
			}
			// Portraits
			if ( press )
			{
				PortraitUpdater.Instance.SetPortrait( Number );
			}
			else
			{
				PortraitUpdater.Instance.SetPortrait( -1 );
			}
			// Start story if pressed
			Game.Instance.OnSwitchStateChanged( Number, press );

			Pressed = press;
			UpdateSprite();
		}
	}

	public void SetLit( bool lit )
	{
		Lit = lit;
		UpdateSprite();
	}

	private void OnMouseDown()
	{
		SetPressed( !Pressed );
	}

	public static void LightSwitches( Dictionary<int, TextAsset> characters )
	{
		// Reset all
		foreach ( var swtch in FindObjectsOfType<Switch>() )
		{
			swtch.SetLit( false );
			swtch.SetPressed( false );
		}

		// Light the correct ones
		var parent = FindObjectOfType<Switch>().transform.parent;
		foreach ( var index in characters.Keys )
		{
			parent.GetChild( index - 1 ).GetComponent<Switch>().SetLit( true );
		}
	}
}
