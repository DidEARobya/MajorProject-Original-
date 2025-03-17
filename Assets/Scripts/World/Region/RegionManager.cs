using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegionManager
{
    public const int clusterSize = 10;

    private int _clusterMapSize;
    private Cluster[,] _clusters;
    private Dictionary<int, HashSet<Region>> _regionHashMap;

    private bool _hasGenerated = false;

    public void Init(WorldGrid grid, int size)
    {
        _clusterMapSize = size;
        _clusters = new Cluster[size, size];
        _regionHashMap = new Dictionary<int, HashSet<Region>>();

        for(int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                _clusters[x, y] = new Cluster(grid, x, y);
            }
        }
    }
    public Region GetNeighbour(int hash, Region region)
    {
        if(_regionHashMap.ContainsKey(hash) == false)
        {
            return null;
        }

        foreach(Region r in _regionHashMap[hash])
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
        if(_regionHashMap.ContainsKey(hash))
        {
            _regionHashMap[hash].Add(region);
            return;
        }

        _regionHashMap.Add(hash, new HashSet<Region>());
        _regionHashMap[hash].Add(region);
    }
    public void RemoveHash(int hash, Region region)
    {
        if (_regionHashMap.ContainsKey(hash) == false)
        {
            Debug.Log("Invalid removal");
            return;
        }

        _regionHashMap[hash].Remove(region);

        if (_regionHashMap[hash].Count == 0)
        {
            _regionHashMap.Remove(hash);
        }
    }
    public Region GetRegionAtTile(Tile tile, bool checkImpassable = false)
    {
        Cluster cluster = GetClusterAtTile(tile);

        foreach(Region region in cluster.regions)
        {
            if(region.Contains(tile) || (checkImpassable == true && region.searchTiles.Contains(tile)))
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

        return _clusters[x, y];
    }
    public void UpdateCluster(Cluster cluster, Tile updatedTile)
    {
        if (_hasGenerated == false)
        {
            return;
        }

        bool updateSurrounding = false;

        if(updatedTile != null)
        {
            foreach(Tile tile in updatedTile.GetAdjacentNeigbours())
            {
                if(tile.region == null || tile.region._inCluster == cluster)
                {
                    continue;
                }

                updateSurrounding = true;
            }
        }
        else
        {
            updateSurrounding = true;
        }

        int x = cluster.x;
        int y = cluster.y;

        cluster.UpdateCluster();

        if (updateSurrounding == false)
        {
            return;
        }

        UpdateClusterAt(x, y + 1);
        UpdateClusterAt(x - 1, y);
        UpdateClusterAt(x + 1, y);
        UpdateClusterAt(x, y - 1);
    }
    void UpdateClusterAt(int x, int y)
    {
        if(x >= 0 && x < _clusterMapSize && y >= 0 && y < _clusterMapSize)
        {
            _clusters[x, y].UpdateCluster();
        }
    }
    public void UpdateMaps()
    {
        foreach(Cluster cluster in _clusters)
        {
            cluster.UpdateCluster();
        }

        _hasGenerated = true;
    }

    public void UpdateRegionDict(Region region, ItemData itemType, int amount)
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
    public int CheckIfRegionContains(Region region, ItemData itemType)
    {
        if (itemType != null)
        {
            return region.Contains(itemType);

        }

        return 0;
    }
}
