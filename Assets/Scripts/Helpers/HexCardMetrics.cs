using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class LiveHexCardMetrics : HexCardMetrics
{
	public LiveHexCardMetrics(HexCardMetrics original)
	{
		CardName = original.CardName;
		Color = new Color(original.Color.r, original.Color.g, original.Color.b, original.Color.a);
		ImagePath = original.ImagePath;
		Tags = new List<string>(original.Tags);

		Rules = new List<Rule>();
		foreach(var rule in original.Rules)
		{
			Rules.Add(new Rule(rule));
		}

		Effects = new List<Effect>();
		effectsFromRules = new Effect[6];
		multipliedEffects = new List<Effect>();

		foreach(var effect in original.Effects)
		{
			Effects.Add(new Effect(effect));
		}
	}
	public Effect[] effectsFromRules;
	public List<Effect> multipliedEffects;

	public void CalculateMultiplier()
	{
		float multiplier = 0;
		foreach(var effect in Effects)
		{
			multiplier += effect.Multiplier;
		}
		foreach(var effect in effectsFromRules)
		{
			if(effect != null)
				multiplier += effect.Multiplier;
		}
		if (multiplier == 0f)
			multiplier = 1f;
		multipliedEffects.Clear();
		foreach (var effect in Effects)
		{
			multipliedEffects.Add(new Effect(effect, multiplier));
		}
		foreach (var effect in effectsFromRules)
		{
			if(effect != null)
				multipliedEffects.Add(new Effect(effect, multiplier));
		}
	}
	public bool AddEffect(Effect effect, string by, int fromDirection)
	{

		var newEffect = new Effect(effect);
		newEffect.By = by;
		if (effectsFromRules[fromDirection] == null)
		{
			effectsFromRules[fromDirection] = newEffect;
			return true;
		}
		return false;
	}
	public bool RemoveEffect(int fromDirection)
	{
		effectsFromRules[fromDirection] = null;
		return true;
	}
	public override string DetailsToString()
	{
		var newString = "";
		for (int i = 0; i < Rules.Count; i++)
		{
			newString += Rules[i].ToString() + "\n";
		}
		for (int i = 0; i < multipliedEffects.Count; i++)
		{
			newString += multipliedEffects[i].ToString() + "\n";
		}
		return newString;
	}
}
[Serializable]
public class HexCardMetrics
{
	public HexCardMetrics()
	{
		Tags = new List<string>();
		Rules = new List<Rule>();
		Effects = new List<Effect>();
	}
	public string CardName;				// Name of the card
	public Color Color;					// Colour of the card
	public string ImagePath;				// Image of the card

	public List<string> Tags;           // Tags of the card, determines which rules this card follows, and applies hidden rules

	
	public List<Rule> Rules;            // Rules this card applies to its neighbours


	
	public List<Effect> Effects;        // Effects of this card

	public override string ToString()
	{
		var newString = $"{CardName}: ";
		foreach(var rule in Rules)
		{
			newString += rule.ToString();
		}
		foreach(var effect in Effects)
		{
			newString += effect.ToString();
		}
		return newString;
	}

	public string TagsToString()
	{
		var newString = "";
		for(int i = 0; i < Tags.Count; i++)
		{
			newString += Tags[i];
			if(i < Tags.Count - 1)
			{
				newString += " - ";
			}
		}
		return newString;
	}

	public virtual string DetailsToString()
	{
		var newString = "";
		for (int i = 0; i < Rules.Count; i++)
		{
			newString += Rules[i].ToString() + "\n";
		}
		for (int i = 0; i < Effects.Count; i++)
		{
			newString += Effects[i].ToString() + "\n";
		}
		return newString;
	}

	public string RulesToString()
	{
		var newString = "";
		for (int i = 0; i < Rules.Count; i++)
		{
			newString += Rules[i].ToString() + "\n";
		}
		return newString;
	}
	public string EffectsToString()
	{
		var newString = "";
		for (int i = 0; i < Effects.Count; i++)
		{
			newString += Effects[i].ToString() + "\n";
		}
		return newString;
	}

	public int GetEffect(StatType type)
	{
		if (type == StatType.None)
			return -1;
		for(int i = 0; i < Effects.Count; i++)
		{
			if (Effects[i].StatType == type.ToString())
				return i;
		}
		return -1;
	}

}

[Serializable]
public class Rule
{
	public Rule()
	{
		Tag = string.Empty;
	}
	public Rule(Rule original)
	{
		Tag = original.Tag;
		Effect = new Effect(original.Effect);
	}

	public string Tag;					// Which tag of cards this rule applies to
	public Effect Effect;				// Effect to apply to appropriate neighbours

	public override string ToString()
	{
		return $"{Effect} to adjacent {Tag}";
	}
}

[Serializable]
public class Effect
{
	public Effect()
	{
		Special = string.Empty;
		SpecialReadable = string.Empty;
		StatType = "None";
		Additive = 0;
		Multiplier = 0;
		By = string.Empty;
	}
	public Effect(Effect original, float _multiplier = 1f)
	{
		Special = original.Special;
		SpecialReadable = original.SpecialReadable;
		StatType = original.StatType;
		Additive = original.Additive * _multiplier;
		Multiplier = original.Multiplier;
		By = original.By;
	}
	public string Special;				// Special effect, like a new mechanic. Toggles a bool in PlayerStats with the same name
	public string SpecialReadable;		// Human readable string for special
	public string StatType;				// Type of stat that additive applies to
	public float Additive;				// Number to add to the stat of said type. Can be negative
	public float Multiplier;			// Multiplier. Applies to all stats on this card
	public string By;                   // Which card this effect comes from. Blank for base effects


	public override string ToString()
	{
		if (!string.IsNullOrWhiteSpace(Special))
			return SpecialReadable;

		if(Multiplier == 0)
		{
			return $"{(string.IsNullOrWhiteSpace(By) ? "" : "<color=#FFFFFF>")} {(Additive >= 0f ? "+" : "-")} {Additive} {StatType}{(string.IsNullOrWhiteSpace(By) ? "" : $" ({By})</color>")}";
		}
		else
		{
			return $"{(string.IsNullOrWhiteSpace(By) ? "" : "<color=#FFFFFF>")}x{Multiplier}{(string.IsNullOrWhiteSpace(By) ? "" : $" ({By})</color>")}";
		}
	}

	
}
