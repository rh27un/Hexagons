using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlash : MonoBehaviour, IEnemyAttack
{
	protected ParticleSystem[] slashParticles;
	protected int slashDir = 0;
	protected Transform player;
	[SerializeField]
	protected float range;
	public void Start()
	{
		slashParticles = GetComponentsInChildren<ParticleSystem>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	public void Attack()
	{
		slashParticles[slashDir % 2].Play();
		slashDir++;
		if ((transform.position - player.position).magnitude < range)
		{
			player.GetComponent<PlayerHealth>().Damage(10f);
		}
	}
}
