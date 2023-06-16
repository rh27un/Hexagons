using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPickup : Pickup
{
	protected HexCardMetrics card;
	protected TextMeshPro text;
	protected override void Start()
	{
		base.Start();
		text = GetComponentInChildren<TextMeshPro>();
		card = gameManager.GetRandomCard();
		text.text = card.CardName;
	}

	protected override void PicksUp(Player player)
	{
		gameManager.AddCardToHand(card);
		Destroy(gameObject);
	}
}
