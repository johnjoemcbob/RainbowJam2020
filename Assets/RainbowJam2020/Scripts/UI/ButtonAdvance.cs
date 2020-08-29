using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAdvance : MonoBehaviour
{
	public static ButtonAdvance Instance;

	static Coroutine Highlighter;

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

	public static void Highlight()
	{
		if ( Highlighter == null )
		{
			Highlighter = Instance.StartCoroutine( Instance.Co_Highlight() );
		}
	}

	public IEnumerator Co_Highlight()
	{
		var ren = GetComponent<Image>();
		var defaultsprite = ren.sprite;
		var but = GetComponent<Button>();

		var flashes = 2;
		var between = 0.2f;
		var hold = 0.05f;
		for ( int flash = 0; flash < flashes; flash++ )
		{
			ren.sprite = but.spriteState.pressedSprite;

			yield return new WaitForSeconds( hold );

			ren.sprite = defaultsprite;

			yield return new WaitForSeconds( between );
		}

		Highlighter = null;
	}
}
