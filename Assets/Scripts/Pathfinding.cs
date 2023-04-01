using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public static Pathfinding Instance { get; private set; }

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private int width;
    private int height;
    private float cellSize;
    private GridSystemHex<PathNode> gridSystem;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("PathFinding instance already exists, destroying object!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Setup(int width, int height , float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystemHex<PathNode>(width, height, cellSize, 
            (GridSystemHex<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        //gridSystem.CreateDebugOjbects(gridDebugObjectPrefab);        

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = gridSystem.GetWorldPosition(gridPosition);
                // Copilot solution
                //bool walkable = !Physics.CheckSphere(worldPosition, cellSize * 0.5f, LayerMask.GetMask("Obstacle"));
                float raycastOffsetDistance = 5f;
                if (Physics.Raycast(
                        worldPosition + Vector3.down * raycastOffsetDistance, 
                        Vector3.up,
                        raycastOffsetDistance * 2,
                        obstaclesLayerMask))
                {
                    gridSystem.GetGridObject(gridPosition).SetIsWalkable(false);
                }

                PathNode pathNode = gridSystem.GetGridObject(gridPosition);
                pathNode.setGCost(int.MaxValue);
                pathNode.setHCost(0);
                pathNode.CalculateFCost();
                pathNode.SetCameFromPathNode(null);
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);
                pathNode.setGCost(int.MaxValue);
                pathNode.setHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }
        
        startNode.setGCost(0);
        startNode.setHCost(CalculateHeuristicDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode)
            {
                pathLength = endNode.getFCost(); 
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }

                if (!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.getGCost() + 1; // Distance between current and neighbour is always 1
                if (tentativeGCost < neighbourNode.getGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.setGCost(tentativeGCost);
                    neighbourNode.setHCost(CalculateHeuristicDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }

        }

        // No path found
        pathLength = 0;
        return null;
    }

    public int CalculateHeuristicDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        return Mathf.RoundToInt(
            Vector3.Distance(gridSystem.GetWorldPosition(gridPositionA), 
                             gridSystem.GetWorldPosition(gridPositionB))
        );
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].getFCost() < lowestFCostPathNode.getFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }

        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        GridPosition gridPosition = currentNode.GetGridPosition();

        bool oddRow = gridPosition.z % 2 == 1;

        List<GridPosition> neighborGridPositionList = new List<GridPosition>
        {
            gridPosition + new GridPosition(-1, 0),
            gridPosition + new GridPosition(+1, 0),

            gridPosition + new GridPosition(0, +1),
            gridPosition + new GridPosition(0, -1),

            gridPosition + new GridPosition(oddRow ? +1 : -1, +1),
            gridPosition + new GridPosition(oddRow ? +1 : -1, -1),

        };    


        foreach (GridPosition neighborGridPosition in neighborGridPositionList)
        {
            int x = neighborGridPosition.x;
            int z = neighborGridPosition.z;
            if ( (x < 0) || (x >= gridSystem.GetWidth()) )
            {
                continue;
            }

            if ( (z < 0) || (z >= gridSystem.GetHeight()) )
            {
                continue;
            }
            
            neighbourList.Add(gridSystem.GetGridObject(neighborGridPosition));
        }

        return neighbourList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();
        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GetGridPosition());
        }

        return gridPositionList;
    }
 
    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }
    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int getPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
