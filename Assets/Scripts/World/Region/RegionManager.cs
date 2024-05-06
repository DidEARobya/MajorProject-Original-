using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public static class RegionManager
{
    public const int regionSize = 10;
    public static Cluster[,] clusters;
    //Change how regions are stored
    public static HashSet<Region> regions;

    public static void Init(WorldGrid grid, int size)
    {
        clusters = new Cluster[size, size];
        regions = new HashSet<Region>();

        for(int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                clusters[x, y] = new Cluster(grid, x, y);
            }
        }
    }

    public static Cluster GetClusterAtTile(Tile tile)
    {
        int x = Mathf.FloorToInt(tile.x / regionSize);
        int y = Mathf.FloorToInt(tile.y / regionSize);

        return clusters[x, y];
    }
    public static void UpdateCluster(Cluster cluster)
    {
        cluster.UpdateCluster();
    }
    public static void UpdateRegionDict(Region region, FurnitureTypes furnitureType, int amount)
    {
        if(region == null)
        {
            return;
        }

        if(furnitureType != null)
        {
            region.UpdateDict(furnitureType, amount);
            return;
        }
    }
    public static void UpdateRegionDict(Region region, OreTypes oreType, int amount)
    {
        if (region == null)
        {
            return;
        }

        if (oreType != null)
        {
            region.UpdateDict(oreType, amount);
            return;
        }
    }
    public static void UpdateRegionDict(Region region, ItemTypes itemType, int amount)
    {
        if (region == null)
        {
            return;
        }

        if (itemType != null)
        {
            region.UpdateDict(itemType, amount);
            return;
        }
    }
    public static int CheckIfRegionContains(Region region, FurnitureTypes furnitureType)
    {
        if (furnitureType != null)
        {
            return region.Contains(furnitureType);

        }

        return 0;
    }
    public static int CheckIfRegionContains(Region region, OreTypes oreType)
    {
        if (oreType != null)
        {
            return region.Contains(oreType);

        }

        return 0;
    }
    public static int CheckIfRegionContains(Region region, ItemTypes itemType)
    {
        if (itemType != null)
        {
            return region.Contains(itemType);

        }

        return 0;
    }
    /*public static void SetNeighbourRegion(Region region)
    {
        int length = regions.GetLength(0);

        int checkX = region.x;
        int checkY = region.y + 1;

        if (checkX >= 0 && checkX < length && checkY >= 0 && checkY < length)
        {
            region.neighbours[0] = new RegionNeighbour(regions[checkX, checkY], Direction.N);
        }
        else
        {
            region.neighbours[0] = new RegionNeighbour(null, Direction.N);
        }

        checkX = region.x + 1;
        checkY = region.y;

        if (checkX >= 0 && checkX < length && checkY >= 0 && checkY < length)
        {
            region.neighbours[1] = new RegionNeighbour(regions[checkX, checkY], Direction.E);
        }
        else
        {
            region.neighbours[1] = new RegionNeighbour(null, Direction.E);
        }

        checkX = region.x;
        checkY = region.y - 1;

        if (checkX >= 0 && checkX < length && checkY >= 0 && checkY < length)
        {
            region.neighbours[2] = new RegionNeighbour(regions[checkX, checkY], Direction.S);
        }
        else
        {
            region.neighbours[2] = new RegionNeighbour(null, Direction.S);
        }

        checkX = region.x - 1;
        checkY = region.y;

        if (checkX >= 0 && checkX < length && checkY >= 0 && checkY < length)
        {
            region.neighbours[3] = new RegionNeighbour(regions[checkX, checkY], Direction.W);
        }
        else
        {
            region.neighbours[3] = new RegionNeighbour(null, Direction.W);
        }
    }*/
}
