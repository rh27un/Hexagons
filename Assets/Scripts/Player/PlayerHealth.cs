using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : Health
{
	public Blood blood;
	public float bloodScale;
	public Image healthBar;
	protected bool invincible;
	[SerializeField]
	protected GameObject gameOver;

	protected string[] youDieds = new string[11]
	{
		"died",
		"expired",
		"perished",
		"succumbed",
		"croaked",
		"departed",
		"are no more",
		"breathed your last",
		"ceased to exist",
		"gave up the ghost",
		"kicked the bucket"
	};
	public void Heal(float amount)
	{
		base.Damage(-amount);
		if (healthBar == null)
			healthBar = GameObject.Find("Healthbar").GetComponent<Image>();
		healthBar.fillAmount = GetHealthPercentage();
	}
	public override bool Damage(float damage)
	{
		if (invincible)
			return false;
		base.Damage(damage);
		blood.AddBlood(damage * bloodScale);
		if (healthBar == null)
			healthBar = GameObject.Find("Healthbar").GetComponent<Image>();
		healthBar.fillAmount = GetHealthPercentage();
		StartCoroutine("DamageInvincibility");
		return true;
	}
	public override void Die()
	{
		if (!blood.isDead)
		{
			blood.isDead = true;
			GetComponent<Player>().enabled = false;
			gameOver.SetActive(true);
			gameOver.GetComponentInChildren<TextMeshProUGUI>().text = "You " + youDieds[Random.Range(0, youDieds.Length)];
		}
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
