using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
	public static Music Instance;

	[Header( "Variables" )]
	public float Volume = 0.1f;
	public float FadeTime = 0.5f;

	[Header( "References" )]
	public AudioSource[] Sources;

	private int Current = 0;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
    {
		for ( int source = 1; source < Sources.Length; source++ )
		{
			Sources[source].volume = 0;
		}
		Sources[0].volume = Volume;
	}

	public void SetTrack( int source )
	{
		StartCoroutine( FadeInOut( Current, 0 ) );
		Current = source;
		StartCoroutine( FadeInOut( Current, Volume ) );
	}

    IEnumerator FadeInOut( int source, float target )
	{
		var start = Sources[source].volume;
		var divisions = 20;
		var between = FadeTime / divisions;
		for ( int i = 0; i < divisions; i++ )
		{
			float progress = ( i * between ) / FadeTime;
			Sources[source].volume = Mathf.MoveTowards( start, target, progress );

			yield return new WaitForSeconds( between );
		}
	}
}
