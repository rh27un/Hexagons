using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
	public float max;
	protected float cur;
	public bool overHeal;
	public GameObject diePrefab;
	public GameObject coinDrop;
	public GameObject healDrop;
	public void Start()
	{
		cur = max;
	}

	public virtual bool Damage(float damage)
	{
		if (!overHeal)
			cur = Mathf.Clamp(cur - damage, 0f, max);
		else
			cur = Mathf.Clamp(cur - damage, 0f, float.PositiveInfinity);

		if (cur == 0)
		{
			Die();
		}
		return true;
	}
	public void Update()
	{

	}

	public virtual void Die()
	{
		Destroy(gameObject);
		Destroy(Instantiate(diePrefab, transform.position, Quaternion.identity), 0.5f);
		float value = Random.value;
		if(value < 0.5f)
		{
			if(value < 0.2f)
			{
				Instantiate(healDrop, transform.position, Quaternion.identity);
			} 
			else
			{
				Instantiate(coinDrop, transform.position, Quaternion.identity);
			}
		}
	}

	public float GetHealthPercentage()
	{
		return cur / max;
	}
}
