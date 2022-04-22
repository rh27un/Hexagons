using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System;

[Serializable]
public class Deck
{
	public List<HexCardMetrics> Cards = new List<HexCardMetrics>();

	public override string ToString()
	{
		string newString = "";
		foreach(var card in Cards)
		{
			newString += card.ToString();
		}
		return newString;
	}

}
