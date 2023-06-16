using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : Pickup
{
	int value;
	[SerializeField]
	protected Material[] materials = new Material[5];
	protected override void PicksUp(Player player)
	{
		player.AddCoin(value);
		Destroy(gameObject);
	}

	protected override void Start()
	{
		base.Start();
		value = gameManager.GetCoinValue();
		GetComponent<MeshRenderer>().material = materials[IndexOfValue()];
	}

	protected int IndexOfValue()
	{
		if (value == 1)
			return 0;
		else if (value == 3)
			return 1;
		else if (value == 6)
			return 2;
		else if (value == 12)
			return 3;
		else if (value == 24)
			return 4;
		else return 0;
	}
}
