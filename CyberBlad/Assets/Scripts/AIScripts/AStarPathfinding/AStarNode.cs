using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode : IHeapItem<AStarNode> {
    
    public bool isWalkable;               // Two node states, walkable or unwalkable
    public Vector3 nodeWorldPosition;     // What point in the world does the node have?
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public AStarNode parent; 
    int heapIndex;

    public AStarNode(bool _isWalkable, Vector3 _nodeWorldPosition, int _gridX, int _gridY) {      // Assign values to new node
        isWalkable = _isWalkable;
        nodeWorldPosition = _nodeWorldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }     

    public int fCost {                  // We get fCost through gCost + hCost, thats why we don't set any values
        get {
            return gCost + hCost;
        }
    }   

    public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(AStarNode nodeToCompare) {
		int compare = fCost.CompareTo(value: nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(value: nodeToCompare.hCost);
		}
		return -compare;
	}                            
}