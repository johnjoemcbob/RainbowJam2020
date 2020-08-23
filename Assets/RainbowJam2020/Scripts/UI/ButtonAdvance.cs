using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAdvance : MonoBehaviour
{
	public static ButtonAdvance Instance;

	private void Awake()
	{
		Instance = this;
	}

	public void OnClick()
	{
		InkHandler.Instance.TryAdvance();
		Game.Instance.TryAdvanceFinishCharacter();
	}
}
