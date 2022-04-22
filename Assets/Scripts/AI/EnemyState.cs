using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Enemy State", menuName = "State/Enemy", order = 1)]
public class EnemyState : ScriptableObject
{
	public float approachPlayerWant;
	public float avoidPlayerWant;
	public float approachNearestWant;
	public float avoidNearestWant;
	public float avoidWallWant;
	public float attackTime;
	public float stateCycle; // Time in seconds before cycling to the next state
}
