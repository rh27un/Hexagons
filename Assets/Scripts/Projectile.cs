using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float speed;
	public string targetTag;
	public float damage;
	protected void Update()
	{
		transform.position += transform.forward * speed * Time.deltaTime;
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == targetTag)
		{
			if(other.GetComponent<Health>().Damage(damage))
				Destroy(gameObject);
		} else if(other.tag == "Wall")
		{
			Destroy(gameObject);
		}
	}
}
