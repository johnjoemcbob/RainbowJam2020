using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMover : MonoBehaviour
{
	public static DialogueMover Instance;

	[Header( "Variables" )]
	public float UIMult = 1;
	public float HiddenYOff = 1;
	public float Speed = 5;

	[Header( "References" )]
	public Transform UIMover;
	public Transform SpriteMover;

	private Vector3 TargetPos;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
    {
		Hide();
		SpriteMover.localPosition = TargetPos;
	}

    void Update()
    {
		var pos = Vector3.MoveTowards( SpriteMover.localPosition, TargetPos, Time.deltaTime * Speed );
		UIMover.localPosition = pos * UIMult;
		SpriteMover.localPosition = pos;
	}

	public void Show()
	{
		TargetPos = Vector3.zero;
	}

	public void Hide()
	{
		TargetPos = new Vector3( 0, HiddenYOff, 0 );
	}
}
