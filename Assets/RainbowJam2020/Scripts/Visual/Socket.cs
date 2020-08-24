using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : MonoBehaviour
{
	[Header( "Variables" )]
	public AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
	public Vector3 start;
	public Vector3 end;
	public float duration = 1.0f;

	[Header( "Assets" )]
	public AudioClip Jingle;

	float t = 1;
	Vector3 DefaultPos;

	private void Awake()
	{
		DefaultPos = transform.localPosition;
	}

	void Update()
	{
		t += Time.deltaTime;
		float s = t / duration;
		transform.localPosition = DefaultPos + Vector3.Lerp( start, end, curve.Evaluate( s ) );
	}

	private void OnMouseEnter()
	{
		if ( t >= 1 )
		{
			t = 0.0f;

			//var pitch = Random.Range( 0.8f, 1.2f );
			var pitch = 0.4f + ( (float) transform.GetSiblingIndex() / transform.parent.childCount ) * 1.2f;
			StaticHelpers.SpawnAudioSource( Jingle, Vector3.zero, pitch, 0.1f );
		}
	}
}
