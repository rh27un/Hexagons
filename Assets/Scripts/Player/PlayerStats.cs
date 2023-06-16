using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum StatType
{
	None,
	MaxHealth,
	Speed,
	AttackDamage,
	AttackRange,
	AttackCooldown,
	DashDistance,
	DashCooldown,
	Knockback,
	Stun,
	AttackAngle,
	Shield,
	Luck,
	Blessing
	
}

public enum SpecialEffect
{
	Slash,
	Projectile,
	Boomerang,
	Ricochet,
	Beam
}
public class PlayerStats : MonoBehaviour
{
	public GameObject statPanelPrefab;
	public float panelOffset;
	private Dictionary<StatType, GameObject> statPanels;
	public Dictionary<StatType, float> baseStats;
	public Dictionary<StatType, float> stats;
	public List<SpecialEffect> specialEffects = new List<SpecialEffect>();
	public void Awake()
	{
		baseStats = new Dictionary<StatType, float>()
		{
			{ StatType.MaxHealth, 100f },
			{ StatType.Speed, 50f },
			{ StatType.AttackDamage, 1f },
			{ StatType.AttackRange, 0f },
			{ StatType.AttackCooldown, 0.25f },
			{ StatType.DashDistance, 5f },
			{ StatType.DashCooldown, 0.9f },
			{ StatType.Knockback, 2f },
			{ StatType.Stun, 1f },
			{ StatType.AttackAngle, 30f },
			{ StatType.Shield, 0f },
			{ StatType.Luck, 1f },
			{ StatType.Blessing, 0f }
		};
		stats = new Dictionary<StatType, float>(baseStats);
		statPanels = new Dictionary<StatType, GameObject>()
		{
			{ StatType.MaxHealth, GameObject.Find("Health Value") },
			{ StatType.Speed, GameObject.Find("Speed Value") },
			{ StatType.AttackDamage, GameObject.Find("Attack Damage Value") },
			{ StatType.AttackRange, GameObject.Find("Attack Range Value") },
			{ StatType.AttackCooldown, GameObject.Find("Attack Cooldown Value") },
			{ StatType.DashDistance, GameObject.Find("Dash Distance Value") },
			{ StatType.DashCooldown, GameObject.Find("Dash Cooldown Value") },
			{ StatType.Knockback, GameObject.Find("Knockback Value") },
			{ StatType.Stun, GameObject.Find("Stun Value") },
			{ StatType.Shield, GameObject.Find("Shield Value") },
			{ StatType.Luck, GameObject.Find("Luck Value") },
			{ StatType.AttackAngle, GameObject.Find("Attack Angle Value") },
			{ StatType.Blessing, GameObject.Find("Blessing Value") }
		};
		foreach(var stat in Enum.GetValues(typeof(StatType)))
		{
			UpdateStats((StatType)stat, 0f);
		}
	}
	public void UpdateStats(StatType type, float value)
	{
		if(stats.ContainsKey(type))
			stats[type] += value;
		if(statPanels.ContainsKey(type))
			statPanels[type].GetComponent<TextMeshProUGUI>().text = stats[type].ToString();
	}

	public void UpdateStatsPanel()
	{
		foreach(var panel in statPanels)
		{
			panel.Value.GetComponent<TextMeshProUGUI>().text = stats[panel.Key].ToString();
		}
	}

	public void AddSpecial(string special)
	{
		SpecialEffect toAdd;
		if(Enum.TryParse(special, out toAdd))
		{
			specialEffects.Add(toAdd);
		}
	}

	public bool HasSpecial(SpecialEffect special)
	{
		return specialEffects.Contains(special);
	}

	public List<SpecialEffect> GetSpecials()
	{
		return specialEffects;
	}
}
