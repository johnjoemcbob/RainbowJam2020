using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMessageLog : MonoBehaviour
{
	public static ButtonMessageLog Instance;

	private void Awake()
	{
		Instance = this;
	}

	public void OnClick()
	{
		InkHandler.Instance.TryAdvance();
	}
}
