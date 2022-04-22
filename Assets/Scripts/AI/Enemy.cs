using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy : MonoBehaviour
{
	public Health health;

	protected CharacterController characterController;

	private Vector3 deltaPos;
	public float decelFactor;
	public float knockBackAmount;

	public GameObject hitPrefab;
	public GameObject diePrefab;

	public float speed;
	public EnemyState[] states;
	private EnemyState state;
	private int stateIndex = 0;
	public float stateCycle; // Flat multiplyer for the length of each state

	public bool stopMovingOnAttack;
	public float findNearestRadius;
	public float findPlayerRadius;

	private Transform player;
	private Collider[] enemies;
	public Vector3 direction; // Unit vector, equal to the normal of all the weighted Want directions

	private Vector3 toPlayer;
	private Vector3 toNearestEnemy;

	private float lastAttack = 0f;

	private IEnemyAttack attack;

	private Vector3[] wallVectors =
	{
		new Vector3(0.86602540378f, 0f, 0.5f),
		new Vector3(0f, 0f, 1f),
		new Vector3(-0.86602540378f, 0f, 0.5f),
		new Vector3(0.86602540378f, 0f, -0.5f),
		new Vector3(0f, 0f , -1f),
		new Vector3(-0.86602540378f, 0f, -0.5f)
	};
	
	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		state = states[stateIndex];
		stateCycle += Random.Range(0.0f, 1.0f);
		characterController = GetComponent<CharacterController>();
		attack = GetComponent<IEnemyAttack>();
		StartCoroutine("CycleStates");
	}

	private void Update()
	{
		FindNearestEnemy();
		direction = Vector3.zero;

		toPlayer = player.position - transform.position;
		float distanceToPlayer = toPlayer.magnitude;

		transform.LookAt(player);
		RaycastHit hit;
		if (Physics.Raycast(transform.position, toPlayer, out hit, findPlayerRadius))
		{
			if (hit.collider.gameObject.tag != "Player")
				return;
		}
		else
			return;

		direction += toPlayer.normalized * state.approachPlayerWant;
		direction -= toPlayer.normalized * (1/distanceToPlayer) * state.avoidPlayerWant;

		if (toNearestEnemy.magnitude > 0)
		{
			direction += toNearestEnemy.normalized * state.approachNearestWant;

			direction -= toNearestEnemy.normalized * (1 / toNearestEnemy.magnitude) * state.avoidNearestWant;
		}

		Vector3 toNearestWall = Vector3.positiveInfinity;

		foreach (var vector in wallVectors)
		{
			if(Physics.Raycast(transform.position, vector, out hit, 10f, 1 << 10))
			{
				if (hit.distance < toNearestWall.magnitude)
					toNearestWall = vector * hit.distance;
			}
		}

		if(toNearestWall.magnitude > 0 &&  !float.IsInfinity(toNearestWall.x))
		{
			direction -= toNearestWall.normalized * (1 / toNearestWall.magnitude) * state.avoidWallWant;
		}

		if(direction.magnitude > 0.1)
			deltaPos += direction.normalized * speed * Time.deltaTime;
		deltaPos *= decelFactor;
		deltaPos.y = 0;
		characterController.Move(deltaPos);

		if (state.attackTime == -1f)
			return;

		if(Time.time > lastAttack + state.attackTime)
		{
			attack.Attack();
			lastAttack = Time.time;
		}
	}
	public void Hit(Vector3 hitPos, float dmg)
	{
		Destroy(Instantiate(hitPrefab, hitPos, Quaternion.identity), 0.5f);
		Vector3 knockBackDirection = (transform.position - hitPos).normalized;
		deltaPos += knockBackDirection * knockBackAmount;
		health.Damage(dmg);
	}

	public void FindNearestEnemy()
	{
		enemies = Physics.OverlapSphere(transform.position, findNearestRadius);
		float distance = Mathf.Infinity;
		foreach(var go in enemies)
		{
			if ((go.transform.position - transform.position).magnitude < distance && (go.transform.position - transform.position).magnitude != 0)
			{
				toNearestEnemy = (go.transform.position - transform.position);
				distance = toNearestEnemy.magnitude;
			}
			
		}
	}

	public IEnumerator CycleStates()
	{
		//Debug.Log(state.stateCycle * stateCycle);
		yield return new WaitForSeconds(state.stateCycle * stateCycle);

		stateIndex++;

		state = states[stateIndex % states.Length];

		lastAttack = Time.time;
		//Debug.Log(state.ToString());
		StartCoroutine("CycleStates");
	}

}
