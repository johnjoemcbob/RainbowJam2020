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

	[Header( "Variables" )]
	public float SpeedMultiplier;

	[Header( "Assets" )]
	public Sprite[] Portraits;

	private SpriteRenderer SpriteRenderer;
	private State CurrentState;
	private Sprite NextSprite;
	private Vector3 BasePos;

	private void Awake()
	{
		Instance = this;

		SpriteRenderer = GetComponent<SpriteRenderer>();

		BasePos = transform.localPosition;
	}

	private void Update()
	{
		switch ( CurrentState )
		{
			case State.Idle:
				break;
			case State.First90:
				var y = 13;
				transform.localPosition = Vector3.MoveTowards( transform.localPosition, new Vector3( BasePos.x, y, 0 ), Time.deltaTime * DialogueMover.Instance.Speed * SpeedMultiplier );
				if ( transform.localPosition.y >= y )
				{
					CurrentState = State.ChangeOrder;
				}
				break;
			case State.ChangeOrder:
				SpriteRenderer.sprite = NextSprite;
				CurrentState = State.Last90;
				break;
			case State.Last90:
				transform.localPosition = Vector3.MoveTowards( transform.localPosition, BasePos, Time.deltaTime * DialogueMover.Instance.Speed * SpeedMultiplier );
				if ( transform.localPosition.y <= BasePos.y )
				{
					CurrentState = State.Idle;
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

		NextSprite = Portraits[character];
		StartSwitch();
	}

	void StartSwitch()
	{
		if ( SpriteRenderer.sprite != NextSprite )
		{
			CurrentState = State.First90;
		}
	}
}
