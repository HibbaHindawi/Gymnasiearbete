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
    public class Sequence : Node {

        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate() {
            bool anyChildIsRunning = false;

            foreach (Node node in children) {
                switch (node.Evaluate()) {
                    
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }

            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }

    }

}