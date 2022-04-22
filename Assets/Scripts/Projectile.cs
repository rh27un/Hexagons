using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float speed;
	protected void Update()
	{
		transform.position += transform.forward * speed * Time.deltaTime;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			other.GetComponent<PlayerHealth>().Damage(10f);
			Destroy(gameObject);
		}
	}
}
