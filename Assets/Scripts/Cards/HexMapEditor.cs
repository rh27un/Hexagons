using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexMapEditor : MonoBehaviour {

	public Color[] colors;
	public List<HexCardMetrics> cards = new List<HexCardMetrics>();
	public GameObject handTogglePrefab;
	public float panelOffset;

	public HexGrid hexGrid;

	private Color activeColor;
	private HexCardMetrics activeCard;
	private GameManager gameManager;

	[SerializeField]
	private Camera hexCam;

	[SerializeField]
	protected Transform content;

	void Awake () {
		Transform content = GameObject.FindGameObjectWithTag("HandContent").transform;
		gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
		int i = 0;
		content.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Math.Abs(panelOffset * cards.Count));
		foreach (var card in cards)
		{
			var newPanel = Instantiate(handTogglePrefab);
			newPanel.transform.SetParent(content);
			newPanel.transform.localPosition = new Vector3(panelOffset * i, 0, 0);
			var text = newPanel.GetComponentInChildren<Text>();
			text.text = card.CardName;
			var toggle = newPanel.GetComponentInChildren<Toggle>();
			toggle.onValueChanged.AddListener(delegate
			{
				SelectCard();
			});
			toggle.group = content.GetComponent<ToggleGroup>();
			i++;
		}
		if (!gameManager.isInSkillGrid)
			content.parent.parent.gameObject.SetActive(false);

	}

	void Update () {
		if (
			Input.GetMouseButtonDown(0) &&
			!EventSystem.current.IsPointerOverGameObject() &&
			gameManager.isInSkillGrid &&
			!gameManager.isInPauseMenu
		) {
			HandleInput();
		}
	}

	void HandleInput () {
		Ray inputRay = hexCam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit))
		{
			HexCoordinates coordinates = hexGrid.GetCoordinatesFromPosition(hit.point);
			hexGrid.PlaceCard(coordinates, activeCard);

		}
	}

	public void SelectCard ()
	{
		var toggles = GameObject.FindGameObjectWithTag("HandContent").GetComponentsInChildren<Toggle>();
		var index = 0;
		foreach(var toggle in toggles)
		{
			if(toggle.isOn)
			{
				activeCard = cards[index];
				return;
			}
			index++;
		}
	}
	public void AddCardToHand(HexCardMetrics card)
	{
		cards.Add(card);
		content.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Math.Abs(panelOffset * cards.Count));
		
		var newPanel = Instantiate(handTogglePrefab);
		newPanel.transform.SetParent(content);
		newPanel.transform.localPosition = new Vector3(panelOffset * cards.Count, 0, 0);
		var text = newPanel.GetComponentInChildren<Text>();
		text.text = card.CardName;
		var toggle = newPanel.GetComponentInChildren<Toggle>();
		toggle.onValueChanged.AddListener(delegate
		{
			SelectCard();
		});
		toggle.group = content.GetComponent<ToggleGroup>();
	}
}