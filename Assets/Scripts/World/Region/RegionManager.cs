using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RegionManager
{
    static int clusterMapSize;
    public const int clusterSize = 10;
    public static Cluster[,] clusters;
    //Change how regions are stored
    public static HashSet<Region> regions;
    public static Dictionary<int, HashSet<Region>> regionsMap;

    static bool hasGenerated = false;

    public static void Init(WorldGrid grid, int size)
    {
        clusterMapSize = size;
        clusters = new Cluster[size, size];
        regions = new HashSet<Region>();
        regionsMap = new Dictionary<int, HashSet<Region>>();

        for(int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                clusters[x, y] = new Cluster(grid, x, y);
            }
        }
    }
    public static Region GetNeighbour(int hash, Region region)
    {
        if(regionsMap.ContainsKey(hash) == false)
        {
            return null;
        }

        foreach(Region r in regionsMap[hash])
        {
            if(region != r)
            {
                return r;
            }
        }

        return null;
    }
    public static void AddHash(int hash, Region region)
    {
        if(regionsMap.ContainsKey(hash) && regionsMap[hash].Contains(region) == false)
        {
            regionsMap[hash].Add(region);
            return;
        }

        regionsMap.Add(hash, new HashSet<Region>());
        regionsMap[hash].Add(region);
    }
    public static void RemoveHash(int hash, Region region)
    {
        if (regionsMap.ContainsKey(hash) == false)
        {
            Debug.Log("Invalid removal");
            return;
        }

        regionsMap[hash].Remove(region);

        if (regionsMap[hash].Count == 0)
        {
            regionsMap.Remove(hash);
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

        int x = cluster.x;
        int y = cluster.y;

        cluster.UpdateCluster();
        clusters[x + 1, y].UpdateCluster();
        clusters[x - 1, y].UpdateCluster();
        clusters[x, y + 1].UpdateCluster();
        clusters[x, y - 1].UpdateCluster();

        foreach (Cluster c in clusters)
        {
            c.UpdateRegionsNeighbours();
        }
    }
    public static void UpdateMaps()
    {
        foreach(Cluster cluster in clusters)
        {
            cluster.UpdateCluster();
        }
        foreach(Cluster cluster in clusters)
        {
            cluster.UpdateRegionsNeighbours();
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
