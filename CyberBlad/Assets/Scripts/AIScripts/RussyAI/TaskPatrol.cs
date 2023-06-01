/*  
    ============================================================

    The general BT architecture was written by Kevin Johansson.

    Additional info.
        Since this script (task patrol) isn't a monobehaviour 
        (but rather an abstract blob of data), we are forced 
        to pass it which transform to use.

    ============================================================
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class TaskPatrol : Node {
    private Transform _transform;
    private Transform[] _waypoints;
    private int _currentWaypointIndex = 0;
    private float _waitTime = 1f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;
    private AStarAgent _aStarAgent;

    public TaskPatrol(Transform transform, Transform[] waypoints, AStarAgent aStarAgent) {
        _transform = transform;
        _waypoints = waypoints;
        _aStarAgent = aStarAgent;
        _aStarAgent.OnPathCompleted += OnPathCompleted;
    }

    public override NodeState Evaluate() {
        if (_waiting) {
            _waitCounter += Time.deltaTime;

            if (_waitCounter >= _waitTime) {
                _waiting = false;
                SetNextWaypoint();
            }

        } else if (!_aStarAgent.IsFollowingPath) {
            SetNextWaypoint();
        }

        state = NodeState.RUNNING;
        return state;
    }

    private void SetNextWaypoint() {
        _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
        Transform wp = _waypoints[_currentWaypointIndex];
        _aStarAgent.SetTarget(wp);
        _waiting = true;
        _waitCounter = 0f;
    }

    private void OnPathCompleted() {
        // Path completed, continue to the next waypoint
        SetNextWaypoint();
    }
}