using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
	public static Wire CurrentHeld;

	[Header( "Variables" )]
	public float SPEED_MOVE = 5;
	public float SPEED_ROTATE = 5;
	public float PLUG_OFFSET = 0.5f;
	public float SEG_EXTEND_MULT = 5;
	public bool CLAMP_BOTTOM = true;

	[Header( "References" )]
	public Transform StartPoint;
	public Transform EndPoint;
	public SpriteRenderer HeadSprite;
	public GameObject DeploySprite;
	public GameObject SocketedSprite;

	[Header( "Assets" )]
	public AudioClip Clip_Drag;
	public AudioClip Clip_Drop;
	public AudioClip Clip_Remove;
	public AudioClip Clip_Hover;
	public AudioClip Clip_Retract;

	private LineRenderer lineRenderer;
	private List<RopeSegment> ropeSegments = new List<RopeSegment>();
	private float ropeSegLen = 0.25f;
	private int segmentLength = 32;
	private float lineWidth = 0.1f;
	private int constraintIterations = 50;
	private Vector3 LastMoveDirection = Vector3.up;
	private Vector3 RetractToPos;

	private State CurrentState = State.Idle;
	private Port Port;

	#region Monobehaviour
	void Start()
	{
		this.lineRenderer = this.GetComponent<LineRenderer>();
		lineRenderer.material.color = ColourPalette.Instance.GetLoopedColour( transform.GetSiblingIndex() );

		Vector3 ropeStartPoint = StartPoint.position;
		for ( int i = 0; i < segmentLength; i++ )
		{
			this.ropeSegments.Add( new RopeSegment( ropeStartPoint ) );
			ropeStartPoint.y -= ropeSegLen;
		}

		RetractToPos = EndPoint.position;
	}

	void Update()
	{
		// Lerp position
		var target = EndPoint.position;
		if ( CurrentState == State.Idle || CurrentState == State.Retract )
		{
			target = RetractToPos;
		}
		if ( CurrentState == State.Held || CurrentState == State.HeldHoverSocket )
		{
			// Head/endpoint follows mouse
			target = Camera.main.ScreenToWorldPoint( Input.mousePosition );
			target.z = 0;
			var dir = ( target - EndPoint.position ).normalized;
				// Target is backwards from the end of the wire, to offset the end of the plug
				if ( CurrentState == State.HeldHoverSocket )
				{
					dir = new Vector3( -1, -1 ).normalized;
					target -= dir * PLUG_OFFSET;
				}
				else if ( CurrentState != State.Socketed )
				{
					target -= dir * PLUG_OFFSET / 2;
				}
			LastMoveDirection = dir;
		}
		if ( CurrentState == State.Socketed )
		{
			target = Port.transform.position;
		}
		EndPoint.position = Vector3.MoveTowards( EndPoint.position, target, SPEED_MOVE * Time.deltaTime );

		// Lerp length
		ropeSegLen = 0.05f + Vector3.Distance( StartPoint.position, EndPoint.position ) * SEG_EXTEND_MULT;

		// Lerp sprite angle in direction of movement
		//if ( EndPoint.position != target )
		{
			float rot_z = Mathf.Atan2(LastMoveDirection.y, LastMoveDirection.x) * Mathf.Rad2Deg;
			var targetang = new Vector3( 0, 0, rot_z - 90 );
				if ( CurrentState == State.Retract )
				{
					// If retracting then lerp towards straight up
					targetang.z = 0;
				}
				if ( CurrentState == State.HeldHoverSocket )
				{
					targetang.z = 180 - 40;
				}
			var rot = Quaternion.Lerp( EndPoint.transform.localRotation, Quaternion.Euler( targetang ), SPEED_ROTATE * Time.deltaTime );
			EndPoint.transform.localRotation = rot;
		}

		// Change sprite based on active state
		// TODO should only be on state change
		var socketed = CurrentState == State.Socketed;
		DeploySprite.SetActive( !socketed );
		SocketedSprite.SetActive( socketed );

		this.DrawRope();
	}

	private void FixedUpdate()
	{
		this.Simulate();
	}
	#endregion

	#region States
	public enum State
	{
		Idle,
		Held,
		HeldHoverSocket,
		Retract,
		Socketed
	}

	public void SwitchState( State newstate )
	{
		CurrentState = newstate;
	}
	#endregion

	#region Simulation
	private void Simulate()
	{
		// SIMULATION
		Vector2 forceGravity = new Vector2(0f, -1f);

		for ( int i = 1; i < this.segmentLength; i++ )
		{
			RopeSegment firstSegment = this.ropeSegments[i];
			Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
			firstSegment.posOld = firstSegment.posNow;
			firstSegment.posNow += velocity;
			firstSegment.posNow += forceGravity * Time.fixedDeltaTime;

			// Clamp to wire rack
			if ( CLAMP_BOTTOM && firstSegment.posNow.y < 0 )
			{
				firstSegment.posNow.x = Mathf.Lerp( firstSegment.posNow.x, 0, Time.deltaTime );
			}
			this.ropeSegments[i] = firstSegment;
		}

		//CONSTRAINTS
		for ( int i = 0; i < constraintIterations; i++ )
		{
			this.ApplyConstraint();
		}
	}

	private void ApplyConstraint()
	{
		//Constrant to First Point 
		RopeSegment firstSegment = this.ropeSegments[0];
		firstSegment.posNow = this.StartPoint.position;
		this.ropeSegments[0] = firstSegment;


		//Constrant to Second Point 
		RopeSegment endSegment = this.ropeSegments[this.ropeSegments.Count - 1];
		endSegment.posNow = this.EndPoint.position;
		this.ropeSegments[this.ropeSegments.Count - 1] = endSegment;

		for ( int i = 0; i < this.segmentLength - 1; i++ )
		{
			RopeSegment firstSeg = this.ropeSegments[i];
			RopeSegment secondSeg = this.ropeSegments[i + 1];

			float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
			float error = Mathf.Abs(dist - this.ropeSegLen);
			Vector2 changeDir = Vector2.zero;

			if ( dist > ropeSegLen )
			{
				changeDir = ( firstSeg.posNow - secondSeg.posNow ).normalized;
			}
			else if ( dist < ropeSegLen )
			{
				changeDir = ( secondSeg.posNow - firstSeg.posNow ).normalized;
			}

			Vector2 changeAmount = changeDir * error;
			if ( i != 0 )
			{
				firstSeg.posNow -= changeAmount * 0.5f;
				this.ropeSegments[i] = firstSeg;
				secondSeg.posNow += changeAmount * 0.5f;
				this.ropeSegments[i + 1] = secondSeg;
			}
			else
			{
				secondSeg.posNow += changeAmount;
				this.ropeSegments[i + 1] = secondSeg;
			}
		}
	}

	private void DrawRope()
	{
		float lineWidth = this.lineWidth;
		lineRenderer.startWidth = lineWidth;
		lineRenderer.endWidth = lineWidth;

		Vector3[] ropePositions = new Vector3[this.segmentLength];
		for ( int i = 0; i < this.segmentLength; i++ )
		{
			ropePositions[i] = this.ropeSegments[i].posNow;
		}

		lineRenderer.positionCount = ropePositions.Length;
		lineRenderer.SetPositions( ropePositions );
	}

	public struct RopeSegment
	{
		public Vector2 posNow;
		public Vector2 posOld;

		public RopeSegment( Vector2 pos )
		{
			this.posNow = pos;
			this.posOld = pos;
		}
	}
	#endregion

	#region Interactions
	public void Pickup()
	{
		CurrentHeld = this;
		CurrentState = State.Held;

		if ( Port != null )
		{
			RemovePort();
			AudioSource.PlayClipAtPoint( Clip_Remove, Vector3.zero );
		}
		else
		{
			AudioSource.PlayClipAtPoint( Clip_Drag, Vector3.zero );
		}

		// Bring to front
		lineRenderer.sortingOrder += 100;
		HeadSprite.sortingOrder += 100;
	}

	public static void TryDrop()
	{
		if ( CurrentHeld )
		{
			CurrentHeld.Drop();
		}
	}

	public void Drop()
	{
		if ( Port.Hovered )
		{
			CurrentHeld.TryAddPort( Port.Hovered );
		}
		else
		{
			CurrentHeld.Retract();
		}
		CurrentHeld = null;

		// Back to normal ordering
		lineRenderer.sortingOrder -= 100;
		HeadSprite.sortingOrder -= 100;
	}

	public void Retract()
	{
		CurrentState = State.Retract;
		AudioSource.PlayClipAtPoint( Clip_Retract, Vector3.zero );
	}

	public static void TryHover()
	{
		if ( CurrentHeld )
		{
			CurrentHeld.CurrentState = State.HeldHoverSocket;
			AudioSource.PlayClipAtPoint( CurrentHeld.Clip_Hover, Vector3.zero );
		}
	}

	public static void TryUnHover()
	{
		if ( CurrentHeld )
		{
			CurrentHeld.CurrentState = State.Held;
		}
	}
	#endregion

	#region Ports
	public void TryAddPort( Port port )
	{
		var ind = port.transform.GetSiblingIndex() + 1;
		bool success = FindObjectOfType<InkHandler>().ChooseChoice( ind );

		if ( success )
		{
			Port = port;
			CurrentState = State.Socketed;
			AudioSource.PlayClipAtPoint( Clip_Drop, Vector3.zero );
		}
		else
		{
			Retract();
		}
	}

	public void RemovePort()
	{
		Port = null;
		FindObjectOfType<InkHandler>().StartStory();
	}
	#endregion
}