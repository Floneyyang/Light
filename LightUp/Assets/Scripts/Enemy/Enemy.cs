using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    public enum EnemyState
    {
        Static,
        Patrol,
        Detect,
        Attack,
        Null
    }

    public enum EnemyType
    {
        Angle,
        Switch
    }

    [Header("Enemy General Settings")]
    public EnemyState state = EnemyState.Patrol;
    public EnemyType type;
    public float detectTime = 0.8f;
    public float coolDownTime = 2f;
    public float attackTime = 4f;

    [Header("Enemy Features")]
    public float detectionRadius;
    public float damageAmount = 1f;

}
