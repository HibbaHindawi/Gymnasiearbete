/*  
    ============================================================

    The general BT architecture was written by Kevin Johansson.

    ============================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class TaskGoToTarget : Node
{
    private Transform _transform;
    private AStarAgent _aStarAgent;

    public TaskGoToTarget(Transform transform, AStarAgent aStarAgent)
    {
        _transform = transform;
        _aStarAgent = aStarAgent;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData(key: "target");

        if (_aStarAgent != null)
        {
            _aStarAgent.SetTarget(target);
        }

        state = NodeState.RUNNING;
        return state;
    }
}