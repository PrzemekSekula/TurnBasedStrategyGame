using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridPosition gridPosition;
    private int gCost;
    private int hCost;
    private int fCost;
    private PathNode cameFromPathNode;

    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    } 
    
    public override string ToString()
    {
        return gridPosition.ToString();
    }

    public int getGCost()
    {
        return gCost;
    }

    public int getHCost()
    {
        return hCost;
    }

    public int getFCost()
    {
        return fCost;
    }

    public void setGCost(int gCost)
    {
        this.gCost = gCost;
        CalculateFCost();
    }

    public void setHCost(int hCost)
    {
        this.hCost = hCost;
        CalculateFCost();
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public PathNode GetCameFromPathNode()
    {
        return cameFromPathNode;
    }

    public void SetCameFromPathNode(PathNode pathNode)
    {
        cameFromPathNode = pathNode;
    }

    public void ResetCameFromPathNode()
    {
        cameFromPathNode = null;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

}

