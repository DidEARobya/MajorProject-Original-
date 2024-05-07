using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public static class RegionManager
{
    public const int clusterSize = 10;
    public static Cluster[,] clusters;
    //Change how regions are stored
    public static HashSet<Region> regions;

    static bool hasGenerated = false;

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
    public static Region GetRegionAtTile(Tile tile)
    {
        Cluster cluster = GetClusterAtTile(tile);

        foreach(Region region in cluster.regions)
        {
            if(region.Contains(tile))
            {
                return region;
            }
        }

        return null;
    }
    public static Cluster GetClusterAtTile(Tile tile)
    {
        int x = Mathf.FloorToInt(tile.x / clusterSize);
        int y = Mathf.FloorToInt(tile.y / clusterSize);

        return clusters[x, y];
    }
    public static void UpdateCluster(Cluster cluster)
    {
        if(hasGenerated == false)
        {
            return;
        }

        cluster.UpdateCluster();
    }
    public static void UpdateClusters()
    {
        foreach(Cluster cluster in clusters)
        {
            cluster.UpdateCluster();
        }

        hasGenerated = true;
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
