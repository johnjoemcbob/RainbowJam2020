using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScroller : MonoBehaviour
{
	public float Speed = 5;
	public Vector2 TopLeft;
	public Vector2 BottomRight;

	int Target = 0;
	List<Vector2> Loop = new List<Vector2>();

	void Start()
    {
		var topright = new Vector2( BottomRight.x, TopLeft.y );
		var bottomleft = new Vector2( TopLeft.x, BottomRight.y );

		Loop.Add( topright );
		Loop.Add( TopLeft );
		Loop.Add( bottomleft );
		Loop.Add( BottomRight );

		// Starting point
		SetPivot( Loop[0] );
	}

    void Update()
    {
		// Move towards current target
		Vector2 pivot = GetPivot();
		pivot = Vector2.MoveTowards( pivot, Loop[Target], Time.deltaTime * Speed );
		SetPivot( pivot );

		// Reach target
		if ( pivot == Loop[Target] )
		{
			Target++;
			if ( Target >= Loop.Count )
			{
				// Loop around constantly
				Target = 0;
			}
		}
	}

	void SetPivot( Vector2 pivot )
	{
		RectTransform trans = transform as RectTransform;
		trans.pivot = pivot;
	}

	Vector2 GetPivot()
	{
		RectTransform trans = transform as RectTransform;
		return trans.pivot;
	}
}
