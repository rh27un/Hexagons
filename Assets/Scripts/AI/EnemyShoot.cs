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
			{
				var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
				projectile.GetComponent<Projectile>().targetTag = "Player";
				projectile.GetComponent<Projectile>().damage = 10f;
				Destroy(projectile, 5f);
			}

		}
	}
}
