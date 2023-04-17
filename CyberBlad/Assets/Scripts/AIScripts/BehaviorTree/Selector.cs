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

using System.Collections.Generic;

namespace BehaviorTree {
    public class Selector : Node {

        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate() {
            foreach (Node node in children) {
                switch (node.Evaluate()) 
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}