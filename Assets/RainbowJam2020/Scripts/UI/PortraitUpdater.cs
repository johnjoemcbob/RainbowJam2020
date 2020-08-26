using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitUpdater : MonoBehaviour
{
	enum State
	{
		Idle,
		First90,
		ChangeOrder,
		Last90
	}

	public static PortraitUpdater Instance;

	public Sprite[] Portraits;

	private Transform[] SpriteRenderers;
	private int Main = 0;
	private State CurrentState;

	private float Speed = 180;

	private void Awake()
	{
		Instance = this;

		// Store references to the two renderers used for the rotation
		SpriteRenderers = new Transform[2];
		SpriteRenderers[0] = transform;
		SpriteRenderers[1] = transform.GetChild( 0 );
	}

	private void Update()
	{
		switch ( CurrentState )
		{
			case State.Idle:
				break;
			case State.First90:
				transform.localEulerAngles = Vector3.MoveTowards( transform.localEulerAngles, new Vector3( 0, 90, 0 ), Time.deltaTime * Speed );
				if ( transform.localEulerAngles.y >= 90 )
				{
					CurrentState = State.ChangeOrder;
				}
				break;
			case State.ChangeOrder:
				SpriteRenderers[Main].GetComponent<SpriteRenderer>().sortingOrder = -15;
				SpriteRenderers[GetNotMain()].GetComponent<SpriteRenderer>().sortingOrder = -14;
				SpriteRenderers[GetNotMain()].localScale = new Vector3( -1, 1, 1 );
				CurrentState = State.Last90;
				break;
			case State.Last90:
				transform.localEulerAngles = Vector3.MoveTowards( transform.localEulerAngles, new Vector3( 0, 180, 0 ), Time.deltaTime * Speed );
				if ( transform.localEulerAngles.y >= 180 )
				{
					CurrentState = State.Idle;
					Main = GetNotMain();

					SpriteRenderers[Main].localEulerAngles = Vector3.zero;
					SpriteRenderers[Main].localScale = new Vector3( 1, 1, 1 );
					SpriteRenderers[GetNotMain()].localEulerAngles = new Vector3( 0, 180, 0 );
				}
				break;
			default:
				break;
		}
	}

	public void SetPortrait( int character )
	{
		// 1 based for actual portraits, 0 is empty
		character++;
		if ( character >= Portraits.Length )
		{
			character = 0;
		}
		if ( character > 0 && !Game.Instance.GetCharacterPortrait( character - 2 ) )
		{
			character = 0;
		}

		SpriteRenderers[GetNotMain()].GetComponent<SpriteRenderer>().sprite = Portraits[character];
		StartSwitch();
	}

	void StartSwitch()
	{
		CurrentState = State.First90;
	}

	int GetNotMain()
	{
		return ( Main == 0 ) ? 1 : 0;
	}
}
