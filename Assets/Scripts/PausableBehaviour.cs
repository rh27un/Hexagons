using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausableBehaviour : MonoBehaviour
{
    protected GameManager gameManager;
    // Start is called before the first frame update
    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.isPaused)
            PausableUpdate();    }

    protected virtual void PausableUpdate()
	{

	}
}
