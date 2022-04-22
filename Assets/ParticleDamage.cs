using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDamage : MonoBehaviour
{
	public float damage;
	public ParticleSystem particle;
	public List<ParticleCollisionEvent> collisionEvents;

	private void Start()
	{
		particle = GetComponent<ParticleSystem>();
		collisionEvents = new List<ParticleCollisionEvent>();
	}
	private void OnParticleCollision(GameObject other)
	{
		Debug.Log("collision with " + other.name);
		int numCollisionEvents = particle.GetCollisionEvents(other, collisionEvents);
		Enemy enemy = other.GetComponent<Enemy>();
		if(enemy != null)
		{
			enemy.Hit(collisionEvents[1].intersection, damage);
		}
	}
}
