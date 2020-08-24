using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterNameText : MonoBehaviour
{
	public static CharacterNameText Instance;

	private void Awake()
	{
		Instance = this;
	}

	public void Set( string name )
	{
		GetComponent<Text>().text = name;
	}
}
