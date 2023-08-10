using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using System.Linq;

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
	public HexMapEditor editor;
	protected Player player;

	public int levelNumber;

	protected int rarityMin;
	protected int rarityMax;

	public int defaultCardIndex;

	protected float[] weights = new float[5]
	{
		0.5f,
		0.3f,
		0.1f,
		0.06f,
		0.04f
	};

	protected int[] coinValues = new int[6]
	{
		1,
		3,
		6,
		12,
		24,
		48
	};

	protected float[] healthValues = new float[6]
	{
		9f,
		15f,
		27f,
		45f,
		69f,
		102f
	};

	private Vector3 mapCamOffset;
	private void Awake()
	{
		//DontDestroyOnLoad(gameObject);  why?
		deck = CreateDeckJson();
		var skillGrid = GameObject.Find("SkillGrid").GetComponent<HexGrid>();
		skillGrid.defaultCardData = deck.Cards[defaultCardIndex];
		editor = hand.transform.parent.GetComponent<HexMapEditor>();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		mapCamOffset = mapCam.transform.position - player.transform.position;
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
				mapCam.transform.position = player.transform.position + mapCamOffset;
				Time.timeScale = 0f;
				isPaused = true;
				isInMap = true;
				mapCam.SetActive(true);
			}
		}
		if (Input.GetButtonDown("Restart"))
		{
			SceneManager.LoadScene("Play");
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
	protected int GetRarity()
	{
		int rarity = 0;
		float weightSum = 0f;
		for (int i = rarityMin - 1; i < rarityMax; i++)
		{
			weightSum += weights[i];
		}
		float rnd = Random.Range(0f, weightSum);
		for (int i = rarityMin - 1; i < rarityMax; i++)
		{
			if (rnd < weights[i])
			{
				rarity = i + 1;
				break;
			}
			rnd -= weights[i];
		}
		if (Random.Range(0f, 100f) < player.Stat(StatType.Luck))
			rarity++;

		return rarity;
	}

	public HexCardMetrics GetRandomCard()
	{
		int rarity = GetRarity();
		var cards = deck.Cards.Where(c => c.Rarity == rarity).ToList();
		return cards[Random.Range(0, cards.Count)];
	}

	public int GetCoinValue()
	{
		return coinValues[GetRarity() - 1];
	}
	public float GetHealthValue()
	{
		return healthValues[GetRarity() - 1];
	}
	public void AddCardToHand(HexCardMetrics card)
	{
		editor.AddCardToHand(card);
	}


	public void NextLevel()
	{
		levelNumber++;
		rarityMin = Mathf.Clamp(Mathf.FloorToInt(levelNumber / 5), 1, 5);
		rarityMax = Mathf.Clamp(Mathf.CeilToInt(levelNumber / 3), 1, 5);
	}
}
