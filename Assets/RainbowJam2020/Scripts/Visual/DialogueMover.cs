using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMover : MonoBehaviour
{
	[Header( "Variables" )]
	public float UIMult = 1;
	public float HiddenYOff = 1;

	[Header( "References" )]
	public Transform UIMover;
	public Transform SpriteMover;

    void Start()
    {
        
    }

    void Update()
    {
		var pos = new Vector3( 0, Mathf.Abs( Mathf.Sin( Time.time ) ) * HiddenYOff, 0 );
		UIMover.localPosition = pos * UIMult;
		SpriteMover.localPosition = pos;
	}
}
