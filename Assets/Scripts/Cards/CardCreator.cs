using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public class CardCreator : MonoBehaviour
{
	public string deckPath;
	public Deck deck;
	public HexCardMetrics card;

	private void Start()
	{
		var serializer = new XmlSerializer(typeof(Deck));
		var stream = new FileStream(deckPath, FileMode.Open);
		deck = serializer.Deserialize(stream) as Deck;
		stream.Close();
		Debug.Log(deck.ToString());
	}

	public void NewCard()
	{
		card = new HexCardMetrics();
	}
	public void SetCardName(string name)
	{
		card.CardName = name;
	}
	public void AddCardToDeck()
	{
		deck.Cards.Add(card);
	}
	public void SaveDeck()
	{
		var serializer = new XmlSerializer(typeof(Deck));
		var stream = new FileStream(deckPath, FileMode.Create);
		serializer.Serialize(stream, deck);
		stream.Close();
	}

	public void NewRule()
	{
		var rule = new Rule
		{
			Effect = new Effect()
		};
		card.Rules.Add(rule);
	}

	public void NewEffect()
	{
		card.Effects.Add(new Effect());
	}
}
