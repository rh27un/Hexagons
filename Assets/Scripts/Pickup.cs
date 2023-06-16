using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
	protected GameManager gameManager;

	// Start is called before the first frame update
	protected virtual void Start()
    {
		gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			PicksUp(other.GetComponent<Player>());
		}
	}

	protected abstract void PicksUp(Player player);
}
