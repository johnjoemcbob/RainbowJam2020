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
		bool consumed = InkHandler.Instance.TryAdvance();
		if ( !consumed )
		{
			Game.Instance.TryAdvanceFinishCharacter();
		}
	}
}
