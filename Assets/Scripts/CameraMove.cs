using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
	public float speed;
	public float scrollSpeed;
	protected GameManager gameManager;
	protected Camera camera;
	public float zoomMin;
	public float zoomMax;

	private void Start()
	{
		gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
		camera = GetComponent<Camera>();
	}

	private void Update()
	{
		if (gameManager.isInPauseMenu)
			return;
		transform.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized * speed * Time.unscaledDeltaTime * camera.orthographicSize;
		//transform.position += transform.forward * Input.GetAxisRaw("Mouse ScrollWheel") * scrollSpeed * Time.unscaledDeltaTime;
		camera.orthographicSize = Mathf.Clamp(camera.orthographicSize - Input.GetAxisRaw("Mouse ScrollWheel") * camera.orthographicSize * scrollSpeed, zoomMin, zoomMax);
		transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 0f, 300f), transform.position.z);
	}
}
