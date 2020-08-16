using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Port : MonoBehaviour
{
	public static Port Hovered;

	public Color ColourHover;
	public Color ColourDefault;

	private SpriteRenderer spriteRenderer;

	private void Start()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();

		GetComponentInChildren<Text>().text = ( transform.GetSiblingIndex() + 1 ).ToString();
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
