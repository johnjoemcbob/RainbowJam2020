using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
	public static Fader Instance;

	#region Enums
	enum TransState
	{
		Idle,
		TransIn,
		Hold,
		TransOut
	}
	#endregion

	[Header( "Variables" )]
	public float Speed = 5;
	public float HoldTime = 0.5f;

	[Header( "References" )]
	public Graphic[] Graphics;

	private TransState CurrentTrans;
	private float CurrentTime;

	private void Awake()
	{
		Instance = this;

		FadeAlpha( 0, true );
	}

	void Update()
    {
		switch ( CurrentTrans )
		{
			case TransState.Idle:
				break;
			case TransState.TransIn:
				if ( FadeAlpha( 1 ) )
				{
					SwitchState( TransState.Hold );
				}
				break;
			case TransState.Hold:
				if ( ( Time.time - CurrentTime ) >= HoldTime )
				{
					SwitchState( TransState.TransOut );
				}
				break;
			case TransState.TransOut:
				if ( FadeAlpha( 0 ) )
				{
					SwitchState( TransState.Idle );
				}
				break;
			default:
				break;
		}
	}

	bool FadeAlpha( float a, bool force = false )
	{
		bool all = true;
		foreach ( var graphic in Graphics )
		{
			Color target = new Color( graphic.color.r, graphic.color.g, graphic.color.b, a );
			float change = Time.deltaTime * Speed;
				if ( force )
				{
					change = 1;
				}
			graphic.color = Color.Lerp( graphic.color, target, change );

			// Flag to end when all are finished
			if ( Mathf.Abs( graphic.color.a - a ) >= 0.01f )
			{
				all = false;
			}
		}
		return all;
	}

	void SwitchState( TransState state )
	{
		CurrentTrans = state;
		CurrentTime = Time.time;
	}

	public void StartTransition()
	{
		SwitchState( TransState.TransIn );
	}
}
