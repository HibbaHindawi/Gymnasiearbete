using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour {

    // public Transform player; proof that grid track works
    AStarNode[,] grid;                  // 2D Node array representing our grid
    public Vector2 sizeOfWorldGrid;     // Represents the coordinates of our grid
    public float radiusOfNode;          // Size of individual node
    public LayerMask unwalkableMask;
    public bool displayGridGizmos;

    float diamaterOfNode;
    int gridSizeX, gridSizeY;

    void Awake() {     
        diamaterOfNode = radiusOfNode * 2;
        gridSizeX = Mathf.RoundToInt(f: sizeOfWorldGrid.x/diamaterOfNode);     // Gives us the amount of nodes that can fit in our grid for x axis
        gridSizeY = Mathf.RoundToInt(f: sizeOfWorldGrid.y/diamaterOfNode);     // Gives us the amount of nodes that can fit in our grid for y axis

        CreateGrid();
    }

    public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

    void CreateGrid() {
        grid = new AStarNode[gridSizeX,gridSizeY];
        Vector3 bottomLeftOfWorld = transform.position - Vector3.right * sizeOfWorldGrid.x / 2 - Vector3.forward * sizeOfWorldGrid.y / 2; 

        for(int x = 0; x < gridSizeX; x++) {
            for(int y = 0; y < gridSizeY; y++) {
                Vector3 worldPoint = bottomLeftOfWorld + Vector3.right * (x * diamaterOfNode * radiusOfNode) + Vector3.forward * (y * diamaterOfNode * radiusOfNode);       // Get the world position of our node collision
                bool isWalkable = !(Physics.CheckSphere(position: worldPoint, radius: radiusOfNode, layerMask: unwalkableMask));                                                                         // Check for collision     
                grid[x,y] = new AStarNode(_isWalkable: isWalkable, _nodeWorldPosition: worldPoint, _gridX: x, _gridY: y);
            }
        }
    }
    
    public List<AStarNode> GetNeighbours(AStarNode node) {
        List<AStarNode> neighbours = new List<AStarNode>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    AStarNode neighbour = grid[checkX, checkY];
                    if (neighbour.isWalkable && !IsNodeOccupiedByOtherAgent(neighbour)) {
                        neighbours.Add(neighbour);
                    }
                }
            }
        }

        return neighbours;
    }

    bool IsNodeOccupiedByOtherAgent(AStarNode node) {
        // Check if any other agent's position matches the given node's position
        AStarAgent[] agents = FindObjectsOfType<AStarAgent>();
        foreach (AStarAgent agent in agents) {
            if (agent != this && agent.transform.position == node.nodeWorldPosition) {
                return true;
            }
        }
        return false;
    }

    public AStarNode NodeFromWorldPoint(Vector3 nodeWorldPosition) {                                // Finds a specific node, say the one the player is standing on
        float percentX = (nodeWorldPosition.x + sizeOfWorldGrid.x / 2) / sizeOfWorldGrid.x;         // Get position for x
        float percentY = (nodeWorldPosition.z + sizeOfWorldGrid.y / 2) / sizeOfWorldGrid.y;         // Get position for y
        percentX = Mathf.Clamp01(value: percentX);                                                         // Clamp x between 0 - 1
        percentY = Mathf.Clamp01(value: percentY);                                                         // Clamp y between 0 - 1

        int x = Mathf.RoundToInt(f: (gridSizeX-1) * percentX);
        int y = Mathf.RoundToInt(f: (gridSizeY-1) * percentY);
        return grid[x,y];
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(center: transform.position, size: new Vector3(x: sizeOfWorldGrid.x, y: 1 ,z: sizeOfWorldGrid.y));      // Since we are using Vector2 we have to represent z axis with y

        if (grid != null && displayGridGizmos){ 
            // AStarNode playerNode = NodeFromWorldPoint(player.position); // proof that grid track works
            foreach (AStarNode nodes in grid) {
                Gizmos.color = (nodes.isWalkable)?Color.white:Color.red;                                    // Show if node is walkable red = not walkable white = walkable
                Gizmos.DrawCube(center: nodes.nodeWorldPosition, size: Vector3.one * (diamaterOfNode-.1f));
                /*if (playerNode == nodes) // proof that grid track works
                    Gizmos.color = Color.cyan;*/
            }
        }
    }

}