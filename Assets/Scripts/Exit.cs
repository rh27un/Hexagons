using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : Interactable
{
	public LevelGrid level;
	protected bool playerInRange;
	private void Start()
	{
		level = GameObject.Find("Level Grid").GetComponent<LevelGrid>();
	}
	protected override void Interact()
	{
		level.Regenerate();
	}
}
