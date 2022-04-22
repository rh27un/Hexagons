using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Xml.Serialization;

public class GameManager : MonoBehaviour
{
	public GameObject pauseMenu;
	public GameObject hexCam;
	public GameObject playCam;
	public GameObject mapCam;
	public GameObject hand;

	public bool isPaused;
	public bool isInPauseMenu;
	public bool isInSkillGrid;
	public bool isInMap;

	public string deckPath;
	public string jsonDeckPath;
	public Deck deck;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		deck = CreateDeckJson();
		var skillGrid = GameObject.Find("SkillGrid").GetComponent<HexGrid>();
		skillGrid.defaultCardData = deck.Cards[0];
		var editor = hand.transform.parent.GetComponent<HexMapEditor>();
		//foreach (var card in deck.Cards)
		//{
		//	editor.AddCardToHand(card);
		//}
		Debug.Log(deck.ToString());
	}

	private Deck CreateDeckJson()
	{
		string json = File.ReadAllText(jsonDeckPath);
		return JsonUtility.FromJson<Deck>(json);
	}

	private Deck CreateDeckXml()
	{
		var serializer = new XmlSerializer(typeof(Deck));
		var stream = new FileStream(deckPath, FileMode.Open);
		deck = serializer.Deserialize(stream) as Deck;
		stream.Close();
		return deck;
	}
	private void Update()
	{
		if (Input.GetButtonDown("Pause"))
		{
			if (!isInPauseMenu)
			{
				isPaused = true;
				isInPauseMenu = true;
				pauseMenu.SetActive(true);
				Time.timeScale = 0f;
			}
			else
			{
				Resume();
			}
		}
		if(Input.GetButtonDown("OpenSkillGrid") && !isInPauseMenu)
		{
			if (isInSkillGrid)
			{
				Time.timeScale = 1f;
				isPaused = false;
				isInSkillGrid = false;
				hexCam.SetActive(false);
				playCam.SetActive(true);
				hand.SetActive(false);
			}
			else
			{
				if (isInMap)
				{
					mapCam.SetActive(false);
					isInMap = false;
				}
				else
					playCam.SetActive(false);
				Time.timeScale = 0f;
				isPaused = true;
				isInSkillGrid = true;
				hexCam.SetActive(true);
				hand.SetActive(true);
			}
		}
		if(Input.GetButtonDown("OpenMap") && !isInPauseMenu)
		{
			if (isInMap)
			{
				Time.timeScale = 1f;
				isPaused = false;
				isInMap = false;
				mapCam.SetActive(false);
				playCam.SetActive(true);
			}
			else
			{
				if (isInSkillGrid)
				{
					hexCam.SetActive(false);
					hand.SetActive(false);
					isInSkillGrid = false;
				}
				else
					playCam.SetActive(false);
				Time.timeScale = 0f;
				isPaused = true;
				isInMap = true;
				mapCam.SetActive(true);
			}
		}
	}

	public void Resume()
	{
		pauseMenu.SetActive(false);
		isInPauseMenu = false;

		if (!isInSkillGrid && !isInMap)
		{
			isPaused = false;
			Time.timeScale = 1f;
		}
	}

	public void Leave()
	{

	}
}
