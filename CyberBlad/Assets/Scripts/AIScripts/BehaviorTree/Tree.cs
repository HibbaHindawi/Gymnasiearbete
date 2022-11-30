/*  
    ============================================================

    The general BT architecture was written by Kevin Johansson.
    Info on how this works and how to implement it was gathered 
    from: https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e

    This script helps to implement the 
    generic Behavior Tree architecture 
    (aka BT architecture).

    ============================================================
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree {
    public abstract class Tree : MonoBehaviour {
        private Node _root = null;

        protected void Start() {
            _root = SetupTree();
        }

        private void Update() {
            if (_root != null)
                _root.Evaluate();
        }

        protected abstract Node SetupTree();
    }
}