using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HexGrid : MonoBehaviour {

	public Transform camTransform;

	public int width = 6;
	public int height = 6;
	public int handSize = 3;

	public int defaultCardX;
	public int defaultCardZ;
	public HexCardMetrics defaultCardData;

	public Color defaultColor = Color.white;

	public HexCell cellPrefab;
	public HexCard cardPrefab;
	public Text cellLabelPrefab;

	public PlayerStats stats;

	protected HexCell[] cells;
	protected Text[] labels;
	private	List<HexCard> hand = new List<HexCard>();
	protected Canvas gridCanvas;
	protected HexMesh hexMesh;

	private HexCoordinates defaultCard;
	public int IndexFromCoordinates(HexCoordinates coordinates) => coordinates.X + coordinates.Z * width + coordinates.Z / 2;

	void Awake () {
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();
		camTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
		defaultCard = new HexCoordinates(defaultCardX, defaultCardZ);

		// Draw Grid

		cells = new HexCell[height * width];
		labels = new Text[height * width];

		for (int z = 0, i = 0; z < height; z++)
		{
			for (int x = 0; x < width; x++)
			{
				CreateCell(x, z, i++);
			}
		}
		Generate();
	}

	protected virtual void Generate()
	{

	}
	protected virtual	void Start () {
		hexMesh.Triangulate(cells);
		PlaceCard(defaultCard, defaultCardData, true);
	}

	public HexCoordinates GetCoordinatesFromPosition(Vector3 position)
	{
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		return coordinates;
	}
	public Vector3 GetPositionFromCoordinates(HexCoordinates coords)
	{
		Vector3 position;
		position.x = (coords.X + coords.Z * 0.5f) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = coords.Z * (HexMetrics.outerRadius * 1.5f);
		return transform.TransformPoint(position);
	}

	public void ColorCell(HexCoordinates coordinates, Color color)
	{
		HexCell cell = GetCell(coordinates);
		if(cell)
			cell.color = color;
		hexMesh.Triangulate(cells);
	}

	protected HexCell CreateCell (int x, int z, int i) 
	{
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.color = defaultColor;

		if (cellLabelPrefab == null)
			return cell;

		Text label = labels[i] = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		//label.text = cell.coordinates.ToStringOnSeparateLines();
		return cell;
	}

	public bool PlaceCard(HexCoordinates coordinates, HexCardMetrics data, bool placeAnywhere = false)
	{

		int i = IndexFromCoordinates(coordinates);
		if (cells[i] != null && (cells[i].canPlace || placeAnywhere))
		{
			HexCard card = Instantiate<HexCard>(cardPrefab);
			card.baseData = data;
			card.actualData = new LiveHexCardMetrics(data);
			card.transform.SetParent(transform, false);
			card.transform.localPosition = cells[i].transform.position;
			card.coordinates = coordinates;
			card.color = Color.white; // TODO: get colour from data
			
			Destroy(cells[i].gameObject);
			cells[i] = card;

			hexMesh.Triangulate(cells);
			CalculateCardRules(card);
			UpdateCardLabel(card.coordinates);
			var neighbours = coordinates.GetNeighbours();
			foreach (var n in neighbours)
			{
				HexCell cell = GetCell(n);
				
				if (cell && !(cell is HexCard))
				{
					cell.canPlace = true;
					ColorCell(cell.coordinates, Color.white);

				}else if(cell is HexCard)
				{
					CalculateCardRules((HexCard)cell);
				}
			}
			foreach(var effect in data.Effects)
			{
				if (!string.IsNullOrEmpty(effect.Special))
				{
					stats.AddSpecial(effect.Special);
				}
			}

			UpdateStats();
			return true;
		}
		return false;
	}

	public HexCell GetCell(HexCoordinates coordinates)
	{
		int index = IndexFromCoordinates(coordinates);
		if (coordinates.X + coordinates.Z / 2 >= width)
			return null;
		if (coordinates.Y > coordinates.X)
			return null;
		if (coordinates.Z < 0 || coordinates.Z > height)
			return null;
		if (index < 0 || index >= height * width)
			return null;
		return cells[index];
	}
	
	public void CalculateCardRules(HexCard card)
	{
		var data = card.actualData;
		var neighbours = card.coordinates.GetNeighbours();
		Dictionary<int, HexCard> neighbourCards = new Dictionary<int, HexCard>();

		for(int i = 0; i < 6; i++)
		{
			var neighbour = neighbours[i];
			var cell = GetCell(neighbour);
			if (cell is HexCard)
			{
				neighbourCards.Add(i, (HexCard)cell);
			}
		}

		
		foreach(var neighbour in neighbourCards)
		{
			foreach (var rule in neighbour.Value.actualData.Rules)
			{
				if (card.actualData.Tags.Contains(rule.Tag))
				{
					if (data.AddEffect(rule.Effect, neighbour.Value.actualData.CardName, neighbour.Key))
					{
						UpdateCardLabel(card.coordinates);
					}
				}
			}
			foreach (var rule in data.Rules)
			{
				if (neighbour.Value.actualData.Tags.Contains(rule.Tag))
				{
					if (neighbour.Value.actualData.AddEffect(rule.Effect, data.CardName, (neighbour.Key + 3) % 6))
					{
						UpdateCardLabel(neighbour.Value.coordinates);
					}
				}
			}
		}
		//foreach(var effect in data.Effects)
		//{
		//	if (!string.IsNullOrEmpty(effect.StatType))
		//	{
		//		StatType statType = (StatType)Enum.Parse(typeof(StatType), effect.StatType);
		//		stats.UpdateStats(statType, effect.Additive);
		//	}
		//}
		//if(card.data is AbilityCardMetrics)
		//{
		//	var data = (AbilityCardMetrics)card.data;
		//}
		//if(card.data is BonusCardMetrics)
		//{
		//	var data = (BonusCardMetrics)card.data;
		//	card.statToChange = data._keys[0];
		//	card.change = data._values[0];

		//	var neighbours = card.coordinates.GetNeighbours();

		//	foreach (var n in neighbours)
		//	{
		//		var cell = GetCell(n);
		//		if (cell is HexCard)
		//		{
		//			// Polymorphism is wack yo
		//			HexCard c = (HexCard)cell;
		//			if (c.data is StatCardMetrics)
		//			{
		//				CalculateCardValue(c);
		//			}
		//		}
		//	}

		//	UpdateCardLabel(card.coordinates);
		//}
		//if(card.data is StatCardMetrics)
		//{
		//	var data = (StatCardMetrics)card.data;
		//	var originalValue = card.change;
		//	card.statToChange = data._keys[0];
		//	card.change = data._values[0];
		//	var neighbours = card.coordinates.GetNeighbours();
		//	var multiplier = 1f;
		//	foreach (var n in neighbours)
		//	{
		//		var cell = GetCell(n);
		//		if(cell is HexCard)
		//		{
		//			// Polymorphism is wack yo
		//			HexCard c = (HexCard)cell;
		//			if(c.data is BonusCardMetrics)
		//			{
		//				multiplier *= c.change;
		//			}
		//		}
		//	}
		//	card.change = data._values[0] * multiplier;
		//	UpdateCardLabel(card.coordinates);
		//	stats.UpdateStats(card.statToChange, card.change - originalValue);
		//}
	}

	public void UpdateCardLabel(HexCoordinates coordinates)
	{
		int index = IndexFromCoordinates(coordinates);
		HexCard card = (HexCard)cells[index];
		if (card)
		{
			card.actualData.CalculateMultiplier();
			labels[index].text = "";
			var labelTransform = labels[index].transform;
			labelTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = card.baseData.CardName;
			labelTransform.GetChild(1).GetComponent<TextMeshProUGUI>().text = card.baseData.TagsToString();
			labelTransform.GetChild(2).GetComponent<TextMeshProUGUI>().text = card.actualData.DetailsToString();
			foreach(var img in labelTransform.GetComponentsInChildren<Image>())
			{
				img.enabled = true;
			}
		}
	}

	public void UpdateStats()
	{
		var currentStats = new Dictionary<StatType, float>(stats.baseStats);
		foreach(var cell in cells)
		{
			if(cell != null && cell is HexCard)
			{
				var card = (HexCard)cell;
				foreach(var effect in card.actualData.multipliedEffects)
				{
					if(!string.IsNullOrEmpty(effect.StatType))
					{
						StatType statType = (StatType)Enum.Parse(typeof(StatType), effect.StatType);
						currentStats[statType] += effect.Additive;
					}
				}
				//foreach(var effect in card.actualData.effectsFromRules)
				//{
				//	if (effect != null)
				//	{
				//		if (!string.IsNullOrEmpty(effect.StatType))
				//		{
				//			StatType statType = (StatType)Enum.Parse(typeof(StatType), effect.StatType);
				//			currentStats[statType] += effect.Additive;
				//		}
				//	}
				//}
			}
		}

		stats.stats = new Dictionary<StatType, float>(currentStats);
		stats.UpdateStatsPanel();
	}
}