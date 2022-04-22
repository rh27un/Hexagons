using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
	public LevelGrid level;
	protected bool playerInRange;
	private void Start()
	{
		level = GameObject.Find("Level Grid").GetComponent<LevelGrid>();
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			playerInRange = true;
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			playerInRange = false;
		}
	}
	private void Update()
	{
		if(playerInRange)
		{
			if (Input.GetButtonDown("Interact"))
			{
				level.Regenerate();
			}
		}
	}
}
