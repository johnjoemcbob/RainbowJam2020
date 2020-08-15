using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Port : MonoBehaviour
{
	public static Port Hovered;

	public Color ColourHover;
	public Color ColourDefault;

	private SpriteRenderer spriteRenderer;

	private void Start()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

	private void OnMouseEnter()
	{
		Wire.TryHover();
		spriteRenderer.color = ColourHover;
		Hovered = this;
	}

	private void OnMouseExit()
	{
		Wire.TryUnHover();
		spriteRenderer.color = ColourDefault;
		Hovered = null;
	}
}
