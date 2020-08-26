using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectoryCharacterNameUpdater : MonoBehaviour
{
	Text Text;
	string BaseText;

    void Start()
    {
		Text = GetComponent<Text>();
		BaseText = Text.text;
	}

    void Update()
    {
		Text.text = GetText();
    }

	string GetText()
	{
		var txt = BaseText;
		for ( int cha = 0; cha < Game.CHARACTERS; cha++ )
		{
			txt = txt.Replace( "{character" + ( cha + 1 ) + "}", Game.Instance.GetCharacterName( cha ) );
		}
		return txt;
	}
}
