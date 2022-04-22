using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
	public float speed;
	public float scrollSpeed;
	protected GameManager gameManager;

	private void Start()
	{
		gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
	}

	private void Update()
	{
		if (gameManager.isInPauseMenu)
			return;
		transform.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized * speed * Time.unscaledDeltaTime;
		transform.position += transform.forward * Input.GetAxisRaw("Mouse ScrollWheel") * scrollSpeed * Time.unscaledDeltaTime;
		transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 0f, 100f), transform.position.z);
	}
}
