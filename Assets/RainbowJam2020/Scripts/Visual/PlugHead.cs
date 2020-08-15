using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugHead : MonoBehaviour
{
	private Wire Parent;

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
		Parent.Pickup();
	}
}
