using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCard : HexCell
{
	public StatType statToChange;
	public float change;
	public HexCardMetrics baseData;
	public LiveHexCardMetrics actualData;

	public override string ToString() => actualData.EffectsToString() + baseData.RulesToString();
}
