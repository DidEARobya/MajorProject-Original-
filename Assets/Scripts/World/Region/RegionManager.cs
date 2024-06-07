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
    public Region GetRegionAtTile(Tile tile, bool checkImpassable = false)
    {
        Cluster cluster = GetClusterAtTile(tile);

        foreach(Region region in cluster.regions)
        {
            if(region.Contains(tile) || (checkImpassable == true && region.impassableTiles.Contains(tile)))
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

        List<Cluster> toUpdateNeighbours = new List<Cluster>();

        cluster.UpdateCluster();
        toUpdateNeighbours.Add(cluster);

        UpdateClusterAt(x, y + 1, toUpdateNeighbours);
        UpdateClusterAt(x - 1, y, toUpdateNeighbours);
        UpdateClusterAt(x + 1, y, toUpdateNeighbours);
        UpdateClusterAt(x, y - 1, toUpdateNeighbours);

        AddClusterAt(x, y + 2, toUpdateNeighbours);
        AddClusterAt(x - 1, y + 1, toUpdateNeighbours);
        AddClusterAt(x + 1, y + 1, toUpdateNeighbours);
        AddClusterAt(x - 2, y, toUpdateNeighbours);
        AddClusterAt(x + 2, y, toUpdateNeighbours);
        AddClusterAt(x - 1, y - 1, toUpdateNeighbours);
        AddClusterAt(x + 1, y - 1, toUpdateNeighbours);
        AddClusterAt(x, y - 2, toUpdateNeighbours);

        foreach (Cluster c in toUpdateNeighbours)
        {
            c.UpdateRegionsNeighbours();
        }
    }
    void AddClusterAt(int x, int y, List<Cluster> list)
    {
        if (x >= 0 && x < clusterMapSize && y >= 0 && y < clusterMapSize)
        {
            list.Add(clusters[x, y]);
        }
    }
    void UpdateClusterAt(int x, int y, List<Cluster> list = null)
    {
        if(x >= 0 && x < clusterMapSize && y >= 0 && y < clusterMapSize)
        {
            if(list != null)
            {
                list.Add(clusters[x, y]);
            }

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
