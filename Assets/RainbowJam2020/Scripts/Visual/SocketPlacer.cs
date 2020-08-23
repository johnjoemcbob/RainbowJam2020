using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SocketPlacer : MonoBehaviour
{
	[Header( "Variables ")]
	public int Sockets = 24;
	public int PerRow = 8;
	public Vector2 Start;
	public Vector2 Offset;

	[Header( "Assets")]
	public GameObject Prefab;
	public Sprite[] Sprites;

	private GameObject[] sockets = new GameObject[0];

    void Update()
    {
        if ( sockets.Length != Sockets )
		{
			// Find the sockets
			sockets = new GameObject[transform.childCount];
			foreach ( Transform child in transform )
			{
				sockets[child.GetSiblingIndex()] = child.gameObject;
			}

			// Delete any old first
			for ( int soc = 0; soc < sockets.Length; soc++ )
			{
				DestroyImmediate( sockets[soc] );
			}

			// Create the prefabs here
			sockets = new GameObject[Sockets];
			for ( int soc = 0; soc < Sockets; soc++ )
			{
				sockets[soc] = Instantiate( Prefab, transform );
			}
		}

		// Update the objects to match the new generation variables
		for ( int soc = 0; soc < sockets.Length; soc++ )
		{
			// Pos
			var col = soc % PerRow;
			var row = Mathf.FloorToInt( soc / PerRow );
			var pos = Start + new Vector2( col * Offset.x, row * Offset.y );
			sockets[soc].transform.localPosition = pos;

			// Sprite
			sockets[soc].GetComponentInChildren<SpriteRenderer>().sprite = Sprites[soc];
		}
    }
}
