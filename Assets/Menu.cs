using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	public GameObject options;
	public Button continueButton;

	private void Start()
	{
		if (false)	//TODO: savestates
		{
			continueButton.interactable = true;
		}
	}

	public void NewGame()
	{
		SceneManager.LoadScene("Play");
	}

	public void Continue()
	{
		Debug.LogError("How");
	}

	public void Options()
	{
		Debug.LogError("Not yet");
	}

	public void Quit()
	{
		Application.Quit();
	}
}
