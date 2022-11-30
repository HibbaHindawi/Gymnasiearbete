/*  
    ============================================================

    The general BT architecture was written by Kevin Johansson.

    Additional info.
        [0] = // Accessing the shared data in our root 
        requiers us to go up by two levels, hens the 
        use of parent.parent

    ============================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CheckEnemyInFOVRange : Node {
    private static int _enemyLayerMask = 1 << 6;

    private Transform _transform;
    // private Animator _animator; Use later when we get animations

    public CheckEnemyInFOVRange(Transform transform) {
        _transform = transform;
        // _animator = transform.GetComponent<Animator>(); Use later when we get animations
    }

    public override NodeState Evaluate() {
        object t = GetData(key: "target");
        if (t == null) {
            Collider[] colliders = Physics.OverlapSphere(position: _transform.position, radius: RussyBT.fovRange, layerMask: _enemyLayerMask);

            if (colliders.Length > 0) {
                parent.parent.SetData(key: "target", value: colliders[0].transform); // [0]
                // _animator.SetBool("Walking", true); Use later when we get animations
                state = NodeState.SUCCESS;
                return state;
            }

            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
