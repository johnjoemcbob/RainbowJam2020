﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Port : MonoBehaviour
{
	public static Port Hovered;

	[Header( "Variables" )]
	public Color ColourHover;
	public Color ColourDefault;

	[HideInInspector]
	public int Number = -1;

	private SpriteRenderer spriteRenderer;

	private void Start()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();

		Number = transform.parent.GetSiblingIndex() + 1;
		GetComponentInChildren<Text>().text = ( Number ).ToString();
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
