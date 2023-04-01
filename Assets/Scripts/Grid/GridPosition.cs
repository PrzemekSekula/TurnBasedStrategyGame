using System;
using System.Collections.Generic;
using System.Numerics;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int z;

    public GridPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
               x == position.x &&
               z == position.z;
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public override string ToString()
    {
        return $"x: {x}, z: {z}";
    }

    public static bool operator ==(GridPosition a, GridPosition b) 
    {
        return a.x == b.x && a.z == b.z;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.z + b.z);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.z - b.z);
    }  

    public List<GridPosition> FindNeighbors(int distance = 1)
    {
        if (distance == 1)
        {
            bool oddRow = this.z % 2 == 1;
            return new List<GridPosition>
            {
                this + new GridPosition(-1, 0),
                this + new GridPosition(+1, 0),

                this + new GridPosition(0, +1),
                this + new GridPosition(0, -1),

                this + new GridPosition(oddRow ? +1 : -1, +1),
                this + new GridPosition(oddRow ? +1 : -1, -1),
            };
        }

        List<GridPosition> neighborList = new List<GridPosition>();
        for (int x = this.x - distance; x <= this.x + distance; x++)
        {
            for (int z = this.z - distance; z <= this.z + distance; z++)
            {
                if (x == 0 && z == 0)
                {
                    continue;
                }

                float d = Vector2.Distance(new Vector2(this.x, this.z), new Vector2(x, z));
                if (d > distance)
                {
                    continue;
                }

                neighborList.Add(new GridPosition(x, z));
            }
        }

        return neighborList;


    } 

}