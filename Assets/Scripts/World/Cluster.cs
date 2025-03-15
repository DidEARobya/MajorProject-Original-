using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class Cluster
{
    int size;

    public int x;
    public int y;

    Tile[,] tiles;
    HashSet<Tile> tileset = new HashSet<Tile>();
    HashSet<Tile> beenChecked = new HashSet<Tile>();

    public HashSet<Region> regions = new HashSet<Region>();

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

        UpdateCluster();
    }
    public void UpdateCluster()
    {
        foreach (Region region in regions)
        {
            region.Delete();
        }

        regions.Clear();
        beenChecked.Clear();

        Region toCheck;

        int tilesSize = size * size;

        while (beenChecked.Count != tilesSize)
        {
            toCheck = new Region(this);

            Tile tile = GetUncheckedTile();

            if (tile == null)
            {
                continue;
            }

            FloodFillFromTile(tile, toCheck);

            if (toCheck.tiles.Count > 0 && regions.Contains(toCheck) == false)
            {
                regions.Add(toCheck);
            }
        }

        if (regions.Count == 0)
        {
            return;
        }

        foreach (Region region in regions)
        {
            region.UpdateRegion();
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
                    if (tiles[x, y].IsAccessible() == Accessibility.IMPASSABLE)
                    {
                        beenChecked.Add(tiles[x, y]);
                        continue;
                    }

                    return tiles[x, y];
                }
            }
        }

        return null;
    }
    void FloodFillFromTile(Tile tile, Region toCheck)
    {
        if(tile.IsObjectInstalled() == true && ObjectManager.Contains(tile.installedObject) && tile.installedObject.type == InstalledObjectType.FURNITURE && (tile.installedObject as Furniture).furnitureType == FurnitureTypes.DOOR)
        {
            beenChecked.Add(tile);

            DoorRegion doorRegion = new DoorRegion(this);
            doorRegion.AddTile(tile);
            regions.Add(doorRegion);
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

            if(t.region == null)
            {
                toCheck.AddTile(t);
            }

            foreach (Tile t2 in t.GetAdjacentNeigbours())
            {
                if (t2.IsObjectInstalled() == true && ObjectManager.Contains(t2.installedObject) && t2.installedObject.type == InstalledObjectType.FURNITURE && (t2.installedObject as Furniture).furnitureType == FurnitureTypes.DOOR)
                {
                    continue;
                }

                if (t2 != null && beenChecked.Contains(t2) == false && tileset.Contains(t2) == true && toCheck.Contains(t2) == false && t2.IsAccessible() != Accessibility.IMPASSABLE)
                {
                    stack.Push(t2);
                }
            }
        }
    }
}
