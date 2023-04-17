/*  
    ============================================================

    The general BT architecture was written by Kevin Johansson.

    ============================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CheckEnemyInAttackRange : Node
{
    private Transform _transform;
    // private Animator _animator;  Use one we get animations

    public CheckEnemyInAttackRange(Transform transform) {
        _transform = transform;
        // _animator = transform.GetComponent<Animator>(); Use one we get animations
    }

    public override NodeState Evaluate() {
        object t = GetData(key: "target");
        if (t == null) {
            state = NodeState.FAILURE;
            return state;
        }

        Transform target = (Transform)t;
        if (Vector3.Distance(a: _transform.position, b: target.position) <= RussyBT.attackRange) {
            // _animator.SetBool("Attacking", true); Use one we get animations
            // _animator.SetBool("Walking", false); Use one we get animations

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}