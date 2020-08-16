using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourPalette : MonoBehaviour
{
	public static ColourPalette Instance;

	public Color[] Palette;

	public void Awake()
	{
		Instance = this;
	}

	public Color GetLoopedColour( int index )
	{
		index = index % Palette.Length;
		return Palette[index];
	}
}
