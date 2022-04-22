using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour, IEnemyAttack
{
	public GameObject projectilePrefab;
	public float fireRate;
	[SerializeField]
	protected float findPlayerRadius;

	public void Attack()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, findPlayerRadius))
		{
			if (hit.collider.gameObject.tag == "Player")
				Destroy(Instantiate(projectilePrefab, transform.position, transform.rotation), 5f);
		}
	}
}
