/*  
    ============================================================

    The general BT architecture was written by Kevin Johansson
    - Jason Potgieter helped with adding projectiles for the 
      enemies.

    ============================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class TaskAttack : Node
{
    private Transform lastTarget;
    private EnemyManager enemyManager;

    private float attackTime = 1f;
    private float attackCounter = 0f;

    public static float projectileSpeed = 20;

    public TaskAttack(Transform transform)
    {

    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData(key: "target");
        if (target != lastTarget)
        {
            enemyManager = target.GetComponent<EnemyManager>();

            // Unsubscribe from previous enemy events
            if (lastTarget != null)
            {
                enemyManager.OnEnemyHit -= HandleEnemyHit;
                enemyManager.OnEnemyDeath -= HandleEnemyDeath;
            }

            // Subscribe to new enemy events
            enemyManager.OnEnemyHit += HandleEnemyHit;
            enemyManager.OnEnemyDeath += HandleEnemyDeath;

            lastTarget = target;
        }

        attackCounter += Time.deltaTime;
        if (attackCounter >= attackTime)
        {
            enemyManager.TakeHit();

            attackCounter = 0f;
        }

        state = NodeState.RUNNING;
        return state;
    }

    private void HandleEnemyHit()
    {
        // Perform actions when enemy is hit
        Debug.Log("Enemy Hit!");
    }

    private void HandleEnemyDeath()
    {
        // Perform actions when enemy dies
        Debug.Log("Enemy Died!");
        ClearData(key: "target");
    }
}
