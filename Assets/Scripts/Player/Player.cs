using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Xml.Schema;

public class Player : MonoBehaviour
{
	protected PlayerHealth health;
	[SerializeField]
	protected PlayerStats stats;
	protected CharacterController characterController;

	public float camFollowStiffness;
	public float slashDistance;

	protected ParticleSystem[] slashParticles;
	protected int slashDir = 0;

	protected Camera mainCamera;
	protected Vector3 cameraOffset;

	protected Vector3 deltaPos = new Vector3();
	protected GameManager gameManager;
	[SerializeField]
	protected FogGrid fogGrid;

	[SerializeField]
	protected float collisionDist;

	protected float cameraShake = 0f;
	[SerializeField]
	protected float shakeScale = 1f;
	[SerializeField]
	protected float shakeSpeed = 10f;

	protected float lastSlash;
	protected float lastDash;

	protected int coins;

	protected TextMeshProUGUI coinsText;
	[SerializeField]
	protected GameObject projectilePrefab;
	[SerializeField]
	protected float projectileSize;

	[SerializeField]
	protected MeshRenderer doorMesh;
	
	private void Awake()
	{
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		fogGrid = GameObject.FindGameObjectWithTag("Fog").GetComponent<FogGrid>();
		cameraOffset = mainCamera.transform.position - transform.position;
		coinsText = GameObject.Find("CoinsText").GetComponent<TextMeshProUGUI>();
		characterController = GetComponent<CharacterController>();
	}
	private void Start()
	{
		slashParticles = GetComponentsInChildren<ParticleSystem>();
		gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
		fogGrid.UpdateFog(transform.position);
		health = GetComponent<PlayerHealth>();
		health.max = Stat(StatType.MaxHealth);
		health.SetToMax();
		//doorMesh.SetActive(false);
	}

	private void FixedUpdate()
	{
		RaycastHit hit;
		int layerMask = 1 << 9;
		if(Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100f, layerMask))
		{
			transform.LookAt(hit.point);
		}
		else
		{
			transform.rotation = Quaternion.identity;
		}
		Vector3 deltaV = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized * Stat(StatType.Speed) * Time.fixedDeltaTime;
		deltaPos = Vector3.ClampMagnitude(deltaPos + deltaV, 500f) * 0.9f;

		if (deltaPos.sqrMagnitude > 0)
		{
			Move(deltaPos * Time.fixedDeltaTime);
		}
		mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, transform.position + cameraOffset, camFollowStiffness);
		mainCamera.transform.position += mainCamera.transform.right * Mathf.Lerp(-1, 1, Mathf.PerlinNoise(0f, Time.time * shakeSpeed)) * cameraShake * shakeScale;
		mainCamera.transform.position += mainCamera.transform.up * Mathf.Lerp(-1, 1, Mathf.PerlinNoise(0f, Time.time * shakeSpeed)) * cameraShake * shakeScale;
		cameraShake *= 0.7f;
	}
	
	private void Update()
	{

		if (Input.GetMouseButton(0) && Time.time > lastSlash + Stat(StatType.AttackCooldown) && !gameManager.isPaused)
		{
			lastSlash = Time.time;
			foreach (SpecialEffect effect in stats.GetSpecials())
			{
				if (effect == SpecialEffect.Slash)
				{
					Slash();
				}
				if (effect == SpecialEffect.Projectile)
				{
					Projectile();
				}
				if (effect == SpecialEffect.Boomerang)
				{

				}
				if (effect == SpecialEffect.Ricochet)
				{

				}
				if (effect == SpecialEffect.Beam)
				{

				}
			}

		}
		if (Input.GetMouseButton(1) && Time.time > lastDash + Stat(StatType.DashCooldown) && !gameManager.isPaused)
		{
			lastDash = Time.time;
			Vector3 dash = transform.forward * Stat(StatType.DashDistance);
			Move(dash);
			health.Dodge(0.7f);
		}
	}

	protected void Move(Vector3 vector)
	{
		characterController.Move(vector);
		if (fogGrid.UpdateFog(transform.position))
		{
			Physics.IgnoreLayerCollision(0, 11, false);
			doorMesh.enabled = true;
		}
		if (!fogGrid.AnyEnemyInRoom(transform.position))
		{
			Physics.IgnoreLayerCollision(0, 11, true);
			doorMesh.enabled = false;
		}
	}

	public void MoveTo(Vector3 position)
	{
		transform.position = position;
		mainCamera.transform.position = transform.position + cameraOffset;
	}
	public float Stat(StatType type)
	{
		return stats.stats[type];
	}

	public void AddCoin(int _coins)
	{
		coins += _coins;
		coinsText.text = coins.ToString();
	}

	public void Heal(float amt)
	{
		health.Damage(-amt);
	}

	protected void Slash()
	{
		Vector3 slash = transform.forward * slashDistance;
		Move(slash);
		slashParticles[slashDir % 2].Play();
		slashDir++;
		var collisions = Physics.OverlapSphere(transform.position, Stat(StatType.AttackRange));
		foreach (var col in collisions)
		{
			if (Vector3.Angle(transform.forward, col.transform.position - transform.position) < Stat(StatType.AttackAngle))
			{
				if (col.gameObject.tag == "Enemy")
				{
					RaycastHit hit;
					if (Physics.Raycast(transform.position, col.transform.position - transform.position, out hit, 10f))
					{
						if (hit.collider == col)
						{
							col.gameObject.GetComponent<Enemy>().Hit(transform.position, Stat(StatType.AttackDamage));
							cameraShake = 1f;
						}
					}
				}
			}
		}
	}

	protected void Projectile()
	{
		var angle = Random.Range(-Stat(StatType.AttackAngle) / 2, Stat(StatType.AttackAngle) / 2);
		var rotation = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
		var projectile = Instantiate(projectilePrefab, transform.position, rotation);
		projectile.transform.localScale = Vector3.one * Stat(StatType.AttackAngle) * projectileSize;
		projectile.GetComponent<Projectile>().targetTag = "Enemy";
		projectile.GetComponent<Projectile>().damage = Stat(StatType.AttackDamage);
		Destroy(projectile, Stat(StatType.AttackRange));
	}

}
