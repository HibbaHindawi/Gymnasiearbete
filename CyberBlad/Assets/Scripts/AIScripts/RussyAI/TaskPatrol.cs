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
using UnityEngine.AI;

public class TaskPatrol : Node {
    private Transform _transform;
    // private Animator _animator; Add once we get animations
    private Transform[] _waypoints;
    NavMeshAgent agent;

    private int _currentWaypointIndex = 0;

    private float _waitTime = 1f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;

    // Constructor for NavMeshAgent
    public TaskPatrol(NavMeshAgent agent) => this.agent = agent;

    // constructor gatering additional info such as waypoints, but also a referance to the agents preforming this task
    public TaskPatrol(Transform transform, Transform[] waypoints) { 
        _transform = transform;
       // _animator = transform.GetComponent<Animator>(); Add once we get animations
        _waypoints = waypoints;
    }

    public override NodeState Evaluate() { // Overrides the node
        if (_waiting) {

            _waitCounter += Time.deltaTime;
            if(_waitCounter < _waitTime) {
                _waiting = false;
                // _animator.SetBool(name: "Walking", value: true); Add once we get animations
            }

        } else {

            Transform wp = _waypoints [_currentWaypointIndex];
            
            if (Vector3.Distance(a: _transform.position, b: wp.position) < 0.01f) {
                _transform.position = wp.position;
                _waitCounter = 0f;
                _waiting = true;

                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                // _animator.SetBool(name: "Walking", value: false); Add once we get animations

            } else {
                _transform.position = Vector3.MoveTowards(current: _transform.position, target: wp.position, maxDistanceDelta: RussyBT.speed * Time.deltaTime);
                _transform.LookAt(worldPosition: wp.position);
            } 
        }

        state = NodeState.RUNNING;
        return state;
    }
}