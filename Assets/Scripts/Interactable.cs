using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
	protected bool isInRange;
	[SerializeField]
	protected TMP_Text text;
	[SerializeField]
	protected string readableName;
	private void Awake()
	{
		text = GameObject.FindGameObjectWithTag("InteractText").GetComponent<TMP_Text>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			isInRange = true;
			text.text = $"{readableName} [E]";
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player")
		{
			isInRange=false;
			text.text = string.Empty;
		}
	}
	private void Update()
	{
		if (isInRange)
		{
			if (Input.GetButtonDown("Interact"))
			{
				text.text = string.Empty;
				Interact();
			}
		}
	}

	protected virtual void Interact() => throw new NotImplementedException();
}
