using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class AStarPathfinding : MonoBehaviour {

    PathRequestManager requestManager;

    AStarGrid grid;

    void Awake () {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<AStarGrid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
        StartCoroutine(FindPath(startPos, targetPos));
    }
   
    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {

        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool didPathSucceed = false;

        AStarNode startNode = grid.NodeFromWorldPoint(nodeWorldPosition: startPos);            // Get world position for our start node
        AStarNode targetNode = grid.NodeFromWorldPoint(nodeWorldPosition: targetPos);          // Get world position for our destination 

        if (startNode.isWalkable && targetNode.isWalkable) {                                                 // Don't bother finding path unless both nodes are walkable
            Heap<AStarNode> openSetOfNodes = new Heap<AStarNode>(maxHeapSize: grid.MaxSize);                 // List of the nodes we want to evaluate 
            HashSet<AStarNode> closedSetOfNodes = new HashSet<AStarNode>();                                  // List of nodes we have evaluated
            openSetOfNodes.Add(item: startNode);

            while (openSetOfNodes.Count > 0) {
                AStarNode currentNode = openSetOfNodes.RemoveFirst();
                closedSetOfNodes.Add(item: currentNode);                             // Add current to Node closed

                if (currentNode == targetNode) {
                    sw.Stop();
                    didPathSucceed = true;
                    break;
                }

                foreach (AStarNode neighbour in grid.GetNeighbours(node: currentNode)) {
                    if (!neighbour.isWalkable || closedSetOfNodes.Contains(item: neighbour)) {
                        continue;
                    }
                    
                    int updatedMovementCostToNeighbour = currentNode.gCost + GetDistance(nodeA: currentNode, nodeB: neighbour);           
                    if (updatedMovementCostToNeighbour < neighbour.gCost || !openSetOfNodes.Contains(item: neighbour)) {

                        neighbour.gCost = updatedMovementCostToNeighbour; 
                        neighbour.hCost = GetDistance(nodeA: neighbour, nodeB: targetNode);
                        neighbour.parent = currentNode;

                        if (!openSetOfNodes.Contains(item: neighbour))
                            openSetOfNodes.Add(item: neighbour);
                        else
                            openSetOfNodes.UpdateItem(neighbour);
                    }
                }
            }
        }
        yield return null;

        if (didPathSucceed) {
            waypoints = RetraceMainPath(startNode: startNode, endNode: targetNode);         // Return if our currentNode is the targetNode
        }
        requestManager.FinishedProcessingPath(waypoints, didPathSucceed);
    }

    Vector3[] RetraceMainPath(AStarNode startNode, AStarNode endNode) {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = endNode;

        while (currentNode != startNode) {              // Retrace our steps until we reach our start position
            path.Add(item: currentNode);
            currentNode = currentNode.parent;
        }
        
        Vector3[] waypoints = PathSimplification(path, startNode);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] PathSimplification(List<AStarNode> path, AStarNode startNode ) {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i ++) {

            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);

            if (directionNew != directionOld)                           // If this is the last node in the list, check to make sure the direction between this node and the startNode doesn't match directionOld
                waypoints.Add(path[i-1].nodeWorldPosition); 

            directionOld = directionNew;                                //  If this is true, then add this new node as a waypoint
            if (i == path.Count - 1 && directionOld != new Vector2(path[i].gridX, path[i].gridY) - new Vector2(startNode.gridX, startNode.gridY))
                waypoints.Add(path[path.Count-1].nodeWorldPosition); 

        }

        return waypoints.ToArray();
    }

    int GetDistance(AStarNode nodeA, AStarNode nodeB) {                         // Get distance between two nodes
        int distanceOnX = Mathf.Abs(value: nodeA.gridX - nodeB.gridX);
        int distanceOnY = Mathf.Abs(value: nodeA.gridY - nodeB.gridY);

        if (distanceOnX > distanceOnY) 
            return 14 * distanceOnY + 10 * (distanceOnX - distanceOnY);         // Calculation to make sure our nodes have the right grid and world positions
        return 14 * distanceOnX + 10 * (distanceOnY - distanceOnX);             // Calculation to make sure our nodes have the right grid and world positi
    }
    
}
