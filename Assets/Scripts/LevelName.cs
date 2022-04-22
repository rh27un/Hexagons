using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelName : MonoBehaviour
{
	protected int levelNumber = 0;

	protected List<string> adjectives = new List<string>
	{
		"Dark",
		"Dismal",
		"Dreary",
		"Dreadful",
		"Damp",
		"Dank",
		"Terrible",
		"Stinky",
		"Smelly",
		"Horrendous",
		"Bad",
		"Scary",
		"Terrifying",
		"Worrying",
		"Abandoned",
		"Average"
	};
	protected List<string> nouns = new List<string>
	{
		"Dungeon",
		"Cellar",
		"Caves",
		"Oubliette",
		"Tower",
		"Mines",
		"Basement",
		"Car park",
		"Shithole"
	};

	protected TextMeshProUGUI text;
	protected float alpha;
	public float fade;

	void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	// Update is called once per frame
	void Update()
	{
		alpha = Mathf.Clamp(alpha - fade * Time.deltaTime, 0f, 1f);
		text.color = Color.Lerp(Color.clear, Color.black, alpha);
	}

	public void NextLevel()
	{
		if(text == null)
			text = GetComponent<TextMeshProUGUI>();

		levelNumber++;
		text.text = $"Level {levelNumber}: \n {adjectives[Random.Range(0, adjectives.Count)]} {nouns[Random.Range(0, nouns.Count)]}";
		alpha = 1;
	}

}
