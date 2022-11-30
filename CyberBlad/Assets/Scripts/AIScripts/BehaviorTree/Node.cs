/*  
    ============================================================

    The general BT architecture was written by Kevin Johansson.
    Info on how this works and how to implement it was gathered 
    from: https://medium.com/geekculture/how-to-create-a-simple-behaviour-tree-in-unity-c-3964c84c060e

    This script implements the generic 
    Behavior Tree architecture (aka BT
    architecture).

    definitions:
        - enum       : user-defined value type used to represent a list of named integer constants.
        - dictionary : a generic collection which is generally used to store key/value pairs.

    ============================================================
*/

using System.Collections;
using System.Collections.Generic;
 
namespace BehaviorTree {
    public enum NodeState {
        RUNNING,
        SUCCESS,
        FAILURE,
    }
    public class Node {
        protected NodeState state;

        public Node parent;
        protected List<Node> children = new List<Node>();

        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        public Node() {
            parent = null;
        }
        public Node(List<Node> children) {
            foreach (Node child in children) {
                _Attach(node: child);
            }
        }

        private void _Attach(Node node) {
            node.parent = this;
            children.Add(item: node);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        public void SetData(string key, object value) {
            _dataContext[key: key] = value;
        }

        public object GetData(string key) {

            object value = null;
            if (_dataContext.TryGetValue(key: key, value: out value))
                return value;

            Node node = parent;
            while (node != null) {
                value = node.GetData(key: key);
                if (value != null)
                    return value;
                node = node.parent;
            }
            return null;

        }

        public bool ClearData(string key) {
            
            if (_dataContext.ContainsKey(key: key)) {
                _dataContext.Remove(key: key);
                return true;
            }

            Node node = parent;
            while (node != null) {
                     
                bool cleared = node.ClearData(key: key);
                if (cleared)
                    return true;
                node = node.parent;

            }
            return false;
        }
    }
}