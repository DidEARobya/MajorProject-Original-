using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : INodeData
{
    int size;

    public RegionNeighbour[] neighbours = new RegionNeighbour[4];
    Tile[,] tiles;
    HashSet<Tile> edges = new HashSet<Tile>();

    int x;
    int y;

    public Region(WorldGrid grid, int _x, int _y)
    {
        size = RegionManager.regionSize;
        tiles = new Tile[size, size];

        for(int x = 0; x < size; x++)
        {
            for(int y = 0; y < size; y++)
            {
                tiles[x, y] = grid.GetTile(x + (size * _x), y + (size * _y));
            }
        }

        x = _x;
        y = _y;

        SetEdges();
    }
    public void SetEdges()
    {
        if(edges.Count > 0)
        {
            edges.Clear();
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if(x == 0 || y == 0 || x == size - 1 || y == size - 1)
                {
                    if (tiles[x, y].IsAccessible() != Accessibility.IMPASSABLE)
                    {
                        edges.Add(tiles[x, y]);
                    }
                }
            }
        }
    }
    public Accessibility IsAccessible()
    {
        return Accessibility.ACCESSIBLE;
    }
    public int GetCost(bool isPlayer)
    {
        return 0;
    }
    public void SetNode(Node node)
    {

    }

    public Region GetRegion()
    {
        return this;
    }
}
public struct RegionNeighbour
{
    public Region neighbour;
    public Path_AStar path;
}
