using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : Health
{
	public Blood blood;
	public float bloodScale;
	public Image healthBar;
	protected bool invincible;
	public override void Damage(float damage)
	{
		if (invincible)
			return;
		base.Damage(damage);
		blood.AddBlood(damage * bloodScale);
		if (healthBar == null)
			healthBar = GameObject.Find("Healthbar").GetComponent<Image>();
		healthBar.fillAmount = GetHealthPercentage();
		StartCoroutine("DamageInvincibility");
	}
	public override void Die()
	{
		blood.isDead = true;
		GetComponent<Player>().enabled = false;
		Debug.Log("Dead");
	}
	public void SetToMax()
	{
		cur = max;
	}

	public void Dodge(float sec)
	{
		StartCoroutine(DodgeInvincibility(sec));
	}

	protected IEnumerator DamageInvincibility()
	{
		invincible = true;
		yield return new WaitForSeconds(0.7f);
		invincible = false;
	}
	
	protected IEnumerator DodgeInvincibility(float sec)
	{
		invincible = true;
		yield return new WaitForSeconds(sec);
		invincible = false;
	}
}
