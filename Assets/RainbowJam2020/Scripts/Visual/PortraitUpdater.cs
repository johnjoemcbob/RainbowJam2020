using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitUpdater : MonoBehaviour
{
	public static PortraitUpdater Instance;

	public Sprite[] Portraits;

	private void Awake()
	{
		Instance = this;
	}

	public void SetPortrait( int character )
	{
		// 1 based for actual portraits, 0 is empty
		character++;
		if ( character >= Portraits.Length )
		{
			character = 0;
		}

		GetComponent<SpriteRenderer>().sprite = Portraits[character];
	}
}
