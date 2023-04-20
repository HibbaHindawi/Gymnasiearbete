/*  
    ============================================================

    The general BT architecture was written by Kevin Johansson.

    ============================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskAttack : Node {
    private Transform _lastTarget;
    private EnemyManager _enemyManager;

    private float _attackTime = 1f;
    private float _attackCounter = 0f;

    public static float projectileSpeed = 20;

    public TaskAttack(Transform transform) {
        
    }

    public override NodeState Evaluate() {

        Transform target = (Transform)GetData(key: "target");
        if (target != _lastTarget) {
            _enemyManager = target.GetComponent<EnemyManager>();
            _lastTarget = target;
        }

        _attackCounter += Time.deltaTime;
        if (_attackCounter >= _attackTime) {

            bool enemyIsDead = _enemyManager.TakeHit();
            if (enemyIsDead) {

                ClearData(key: "target"); 
            } else {

                _attackCounter = 0f;
            }
        }

        state = NodeState.RUNNING;
        return state;
    }
}