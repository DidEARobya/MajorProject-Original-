using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager
{
    static int clusterMapSize;
    public const int clusterSize = 10;
    public  Cluster[,] clusters;
    public  HashSet<Region> regions;
    public  Dictionary<int, HashSet<Region>> regionsMap;

    bool hasGenerated = false;

    public void Init(WorldGrid grid, int size)
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
    public void ClearRegionDisplayAt(Region region)
    {
        if (region == null)
        {
            return;
        }

        region.DestroyDisplayTiles(false);
    }
    public void ClearRegionDisplay()
    {
        foreach (Region region in regions)
        {
            region.DestroyDisplayTiles(true);
        }
    }
    public Region GetNeighbour(int hash, Region region)
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
    public void AddHash(int hash, Region region)
    {
        if(regionsMap.ContainsKey(hash))
        {
            regionsMap[hash].Add(region);
            return;
        }

        regionsMap.Add(hash, new HashSet<Region>());
        regionsMap[hash].Add(region);
    }
    public void RemoveHash(int hash, Region region)
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
    public Region GetRegionAtTile(Tile tile)
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
    public Cluster GetClusterAtTile(Tile tile)
    {
        int x = Mathf.FloorToInt(tile.x / clusterSize);
        int y = Mathf.FloorToInt(tile.y / clusterSize);

        return clusters[x, y];
    }
    public void UpdateCluster(Cluster cluster)
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

        foreach (Cluster c in clusters)
        {
            c.UpdateRegionsNeighbours();
        }
    }
    void UpdateClusterAt(int x, int y)
    {
        if(x >= 0 && x < clusterMapSize && y >= 0 && y < clusterMapSize)
        {
            clusters[x, y].UpdateCluster();
        }
    }
    void UpdateClusterNeighboursAt(int x, int y)
    {
        if (x >= 0 && x < clusterMapSize && y >= 0 && y < clusterMapSize)
        {
            clusters[x, y].UpdateRegionsNeighbours();
        }
    }
    public void UpdateMaps()
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

    public void UpdateRegionDict(Region region, ItemTypes itemType, int amount)
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
    public int CheckIfRegionContains(Region region, ItemTypes itemType)
    {
        if (itemType != null)
        {
            return region.Contains(itemType);

        }

        return 0;
    }
}
