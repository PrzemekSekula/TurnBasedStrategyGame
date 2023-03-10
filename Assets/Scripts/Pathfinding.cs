using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinding Instance { get; private set; }

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private int width;
    private int height;
    private float cellSize;
    private GridSystem<PathNode> gridSystem;

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

        gridSystem = new GridSystem<PathNode>(width, height, cellSize, 
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
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
        startNode.setHCost(CalculateDistance(startGridPosition, endGridPosition));
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

                int tentativeGCost = currentNode.getGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());
                if (tentativeGCost < neighbourNode.getGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.setGCost(tentativeGCost);
                    neighbourNode.setHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
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

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        int xDistance = Mathf.Abs(gridPositionDistance.x); 
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + 
                MOVE_STRAIGHT_COST * Mathf.Abs(xDistance - zDistance); 
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

        // Differs from the course code
        for (int x = gridPosition.x - 1; x <=gridPosition.x + 1; x++)
        {
            if ( (x < 0) || (x >= gridSystem.GetWidth()) )
            {
                continue;
            }

            for (int z = gridPosition.z - 1; z <= gridPosition.z + 1; z++)
            {
                if ( (z < 0) || (z >= gridSystem.GetHeight()) )
                {
                    continue;
                }
                if ( (x == gridPosition.x) && (z == gridPosition.z) )
                {
                    continue;
                }
                neighbourList.Add(GetNode(x, z));
            }
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
 
    public void setIsWalkable(GridPosition gridPosition, bool isWalkable)
    {
        gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }
    public bool isWalkableGridPosition(GridPosition gridPosition)
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
