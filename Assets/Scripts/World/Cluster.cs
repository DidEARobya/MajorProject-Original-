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
        size = RegionManager.clusterSize;
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

        UpdateCluster(true);
    }
    public void UpdateCluster(bool isStart = false)
    {
        foreach(Region region in regions)
        {
            region.Delete();
        }

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


            if (tile.installedObject != null)
            {

            }

            FloodFillCluster(tile, toCheck);

            if (toCheck.tiles.Count > 0 && regions.Contains(toCheck) == false)
            {
                regions.Add(toCheck);

                if(isStart == false)
                {
                    toCheck.UpdateRegion();
                }
            }
        }

        Debug.Log(regions.Count);
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
        if(tile.IsObjectInstalled() == true && tile.IsAccessible() == Accessibility.IMPASSABLE)
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


            Dictionary<Tile, Direction> neighbours = t.GetNeighboursDict();

            foreach (Tile t2 in neighbours.Keys)
            {
                if (t2 != null && beenChecked.Contains(t2) == false && tileset.Contains(t2) == true && t2.IsObjectInstalled() == false && tile.IsAccessible() != Accessibility.IMPASSABLE && toCheck.Contains(t2) == false)
                {
                    stack.Push(t2);
                }
            }
        }
    }
}
