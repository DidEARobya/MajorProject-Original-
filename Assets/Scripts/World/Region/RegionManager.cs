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
    public static void ClearRegionDisplayAt(Region region)
    {
        if (region == null)
        {
            return;
        }

        region.DestroyDisplayTiles(false);
    }
    public static void ClearRegionDisplay()
    {
        foreach (Region region in regions)
        {
            region.DestroyDisplayTiles(true);
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
        if(regionsMap.ContainsKey(hash))
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
        UpdateClusterAt(x, y + 1);
        UpdateClusterAt(x - 1, y);
        UpdateClusterAt(x + 1, y );
        UpdateClusterAt(x, y - 1);

        cluster.UpdateRegionsNeighbours();
        UpdateClusterNeighboursAt(x, y + 1);
        UpdateClusterNeighboursAt(x - 1, y);
        UpdateClusterNeighboursAt(x + 1, y);
        UpdateClusterNeighboursAt(x, y - 1);

    }
    static void UpdateClusterAt(int x, int y)
    {
        if(x >= 0 && x < clusterMapSize && y >= 0 && y < clusterMapSize)
        {
            clusters[x, y].UpdateCluster();
        }
    }
    static void UpdateClusterNeighboursAt(int x, int y)
    {
        if (x >= 0 && x < clusterMapSize && y >= 0 && y < clusterMapSize)
        {
            clusters[x, y].UpdateRegionsNeighbours();
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
}
