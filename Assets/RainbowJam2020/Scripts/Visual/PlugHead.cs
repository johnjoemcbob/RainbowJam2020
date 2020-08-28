using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class PlugHead : MonoBehaviour
{
	[Header( "Variables" )]
	public bool IsDeployedJack;

	[Header( "Assets" )]
	public Sprite[] JackSprites;

	private Wire Parent;

	private void Awake()
	{
		if ( IsDeployedJack )
		{
			var index = transform.parent.parent.GetSiblingIndex();
			GetComponent<SpriteRenderer>().sprite = JackSprites[index];
		}
	}

	private void Start()
	{
		Parent = GetComponentInParent<Wire>();
	}

	private void Update()
	{
		if ( Input.GetMouseButtonUp( 0 ) )
		{
			Wire.TryDrop();
		}
	}

	private void OnMouseDown()
	{
		if ( Game.Instance.IsBlocked() ) return;

		Parent.TryPickup();
	}
}
