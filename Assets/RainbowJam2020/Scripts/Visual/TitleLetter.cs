using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLetter : MonoBehaviour
{
	public const int POINTS_PER_TRI = 50;
	public const int LERP_SPEED = 5;

	public Vector2[] Points;

	private LineRenderer lineRenderer;
	private Transform[] Heads;
	private Vector3[] Targets;

    void Start()
    {
		lineRenderer = GetComponent<LineRenderer>();

		// Want inspector to be 2d vector for prettiness, but line renderer wants 3d
		Targets = new Vector3[Points.Length];
			for ( int i = 0; i < Points.Length; i++ )
			{
				Targets[i] = Points[i];
			}
		lineRenderer.positionCount = Points.Length;
		for ( int i = 0; i < Points.Length; i++ )
		{
			var target = ( Targets[0] - Targets[1] ) * -( 10 + i );
			lineRenderer.SetPosition( i, target );
		}

		// Store wire heads for following line
		Heads = new Transform[2];
		Heads[0] = transform.GetChild( 0 );
		Heads[1] = transform.GetChild( 1 );
	}

	int reached = 0;
	void Update()
    {
		for ( int i = 0; i < Points.Length; i++ )
		{
			var pos = lineRenderer.GetPosition( i );
			// TODO TEMP REMOVE
			Targets[i] = Points[i];

			// Lerp each point towards next closest target, or follow the leader if its not reached the targets yet
			var target = Targets[i];
			if ( reached < i )
			{
				// Otherwise follow the leader
				target = Targets[Targets.Length - reached - 1];
			}
			pos = Vector3.MoveTowards( pos, target, Time.deltaTime * LERP_SPEED );
			Debug.Log( Targets.Length - reached - 1 );
			if ( reached + 1 < Targets.Length && Vector3.Distance( pos, Targets[Targets.Length-reached-1] ) < 0.2f )
			{
				reached++;
				Debug.Log( "reached " + reached );
			}

			// Sway a little
			//pos += new Vector3( Mathf.Sin( Time.time + i ), Mathf.Cos( Time.time - i ), 0 ) / 100;
			lineRenderer.SetPosition( i, pos );

			// Follow with wire heads
			if ( i == 0 )
			{
				FollowHead( 0, i, i + 1 );
			}
			if ( i == Points.Length - 1 )
			{
				FollowHead( 1, i, i - 1 );
			}
		}
	}

	void FollowHead( int head, int point, int next )
	{
		var pos = lineRenderer.GetPosition( point );
		Heads[head].position = pos;

		var dir = ( pos - lineRenderer.GetPosition( next ) ).normalized;
		float rot_z = Mathf.Atan2( dir.y, dir.x ) * Mathf.Rad2Deg;
			// Sway a little
			rot_z += Mathf.Sin( Time.time + head ) * 5;
		var targetang = new Vector3( 0, 0, rot_z - 90 );
		Heads[head].eulerAngles = targetang;
	}

	void CreateQuadraticBezierCurves()
	{
		var tris = Points.Length / 3;
		lineRenderer.positionCount = POINTS_PER_TRI * tris;
		for ( int tri = 0; tri < tris; tri++ )
		{
			int i = tri * 3;
			CreateQuadraticBezierCurve( tri, Points[i], Points[i+1], Points[i+2] );
		}
	}

	// From: https://www.codinblack.com/how-to-draw-lines-circles-or-anything-else-using-linerenderer/
	void CreateQuadraticBezierCurve( int tri, Vector3 point0, Vector3 point1, Vector3 point2 )
	{
		float t = 0f;
		Vector3 B = new Vector3(0, 0, 0);
		for ( int i = 0; i < POINTS_PER_TRI; i++ )
		{
			int index = i + ( POINTS_PER_TRI * tri );
			B = ( 1 - t ) * ( 1 - t ) * point0 + 2 * ( 1 - t ) * t * point1 + t * t * point2;
			lineRenderer.SetPosition( index, B );
			t += ( 1 / (float) lineRenderer.positionCount );
		}
	}
}
