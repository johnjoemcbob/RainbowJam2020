using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageLog : MonoBehaviour
{
	public static MessageLog Instance;

	[Header( "References" )]
	public Transform MessageParent;

	[Header( "Assets" )]
	public GameObject MessagePrefab;

	void Awake()
	{
		Instance = this;

		gameObject.SetActive( false );
	}

    void OnEnable()
    {
		// TODO
		// Clear old message log
		foreach ( Transform child in MessageParent )
		{
			Destroy( child.gameObject );
		}

		// Populate with current character story (if story not -1)
		foreach ( var msg in Game.Instance.GetCurrentMessageLog() )
		{
			var obj = Instantiate( MessagePrefab, MessageParent );
			obj.GetComponent<Text>().text = msg;
		}
    }

	void OnDisable()
	{

	}
}
