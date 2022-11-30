/*  
    ============================================================

    The general BT architecture was written by Kevin Johansson.

    ============================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskGoToTarget : Node {
    private Transform _transform;
    private float destination;

    public TaskGoToTarget(Transform transform) {
        _transform = transform;
    }

    public override NodeState Evaluate() {
        Transform target = (Transform)GetData(key: "target");

        if (Vector3.Distance(a: _transform.position, b: target.position) >0.01f) {
            _transform.position = Vector3.MoveTowards(current: _transform.position, target: target.position, maxDistanceDelta: RussyBT.speed * Time.deltaTime);
            _transform.LookAt(worldPosition: target.position);
        }

        state = NodeState.RUNNING;
        return state;
    }
}
