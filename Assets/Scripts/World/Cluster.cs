using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Cluster
{
    int size;

    public int x;
    public int y;

    Tile[,] tiles;
    HashSet<Tile> tileset = new HashSet<Tile>();
    HashSet<Tile> edges = new HashSet<Tile>();
    HashSet<Tile> beenChecked = new HashSet<Tile>();
    public List<Region> regions = new List<Region>();

    public Cluster(WorldGrid grid, int _x, int _y)
    {
        size = RegionManager.regionSize;
        tiles = new Tile[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                tiles[x, y] = grid.GetTile(x + (size * _x), y + (size * _y));
                tileset.Add(tiles[x, y]);
            }
        }

        x = _x;
        y = _y;

        UpdateCluster();
        SetEdges();
    }
    public void SetEdges()
    {
        if (edges.Count > 0)
        {
            edges.Clear();
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (x == 0 || y == 0 || x == size - 1 || y == size - 1)
                {
                    if (tiles[x, y].IsAccessible() != Accessibility.IMPASSABLE)
                    {
                        edges.Add(tiles[x, y]);
                    }
                }
            }
        }
    }
    public void UpdateCluster()
    {
        regions.Clear();
        beenChecked.Clear();

        int half = (size / 2) - 1;

        Region toCheck;

        int tilesSize = size * size;

        while (beenChecked.Count != tilesSize)
        {
            toCheck = new Region();

            Tile tile = GetUncheckedTile();

            if (tile == null)
            {
                continue;
            }

            FloodFillCluster(tile, toCheck);

            if (toCheck.tiles.Count > 0 && regions.Contains(toCheck) == false)
            {
                regions.Add(toCheck);
                toCheck.UpdateRegion();
            }
        }
    }
    Tile GetUncheckedTile()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (beenChecked.Contains(tiles[x, y]) == false)
                {
                    return tiles[x, y];
                }
            }
        }

        return null;
    }
    void FloodFillCluster(Tile startTile, Region toCheck)
    {
        FloodFillFromTile(startTile, toCheck);
    }

    void FloodFillFromTile(Tile tile, Region toCheck)
    {
        if(tile.IsObjectInstalled() == true)
        {
            beenChecked.Add(tile);
            return;
        }

        Stack<Tile> stack = new Stack<Tile>();

        if (tile != null && beenChecked.Contains(tile) == false && tileset.Contains(tile) == true)
        {
            stack.Push(tile);
        }

        while (stack.Count > 0)
        {
            Tile t = stack.Pop();

            beenChecked.Add(t);
            toCheck.AddTile(t);

            /*if(regions.Count == 0 && regions.Contains(toCheck) == false)
            {
                t.SetFloorType(FloorTypes.TASK);
            }
            else if(regions.Count == 1  && regions.Contains(toCheck) == false)
            {
                t.SetFloorType(FloorTypes.WOOD);
            }
            else if(regions.Count == 2 && regions.Contains(toCheck) == false)
            {
                t.SetFloorType(FloorTypes.NONE);
            }*/

            Dictionary<Tile, Direction> neighbours = t.GetNeighboursDict();

            foreach (Tile t2 in neighbours.Keys)
            {
                if (t2 != null && beenChecked.Contains(t2) == false && tileset.Contains(t2) == true && t2.IsObjectInstalled() == false && toCheck.Contains(t2) == false)
                {
                    stack.Push(t2);
                }
            }
        }
    }
}
