using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Region
{
    public float gCost;
    public float hCost;
    public float fCost;

    public HashSet<Tile> tiles = new HashSet<Tile>();
    public HashSet<Tile> searchTiles = new HashSet<Tile>();

    private List<Tile> _northPairs = new List<Tile>();
    private List<Tile> _eastPairs = new List<Tile>();
    private List<Tile> _southPairs = new List<Tile>();
    private List<Tile> _westPairs = new List<Tile>();

    private HashSet<int> _spans = new HashSet<int>();

    private Dictionary<ItemData, int> _itemsInRegion = new Dictionary<ItemData, int>();
    private Dictionary<BuildingData, int> _buildingsInRegion = new Dictionary<BuildingData, int>();
    private Dictionary<OreData, int> _oreInRegion = new Dictionary<OreData, int>();
    private Dictionary<PlantData, int> _plantsInRegion = new Dictionary<PlantData, int>();

    public Cluster _inCluster;
    public bool isDoor;
    public Region(Cluster inCluster)
    {
        _inCluster = inCluster;
        isDoor = false;
    }
    public void HighlightTiles(UnityEngine.Color colour, bool isNeighbour)
    {
        foreach (Tile tile in tiles)
        {
            if(tile.displayRegion == true)
            {
                continue;
            }

            tile.displayRegion = true;
            tile.regionColour = colour;
            tile.UpdateVisual();
        }

        if(isNeighbour == true)
        {
            return;
        }

        foreach(int i in _spans)
        {
            Region region = GameManager.GetRegionManager().GetNeighbour(i, this);

            if(region != null)
            {
                region.HighlightTiles(UnityEngine.Color.red, true);
            }
        }
    }
    public void DestroyDisplayTiles(bool isNeighbour)
    {
        foreach (Tile tile in tiles)
        {
            tile.displayRegion = false;
            tile.UpdateVisual();
        }

        if(isNeighbour == true) 
        { 
            return; 
        }

        foreach (int i in _spans)
        {
            Region region = GameManager.GetRegionManager().GetNeighbour(i, this);
            if(region != null)
            {
                region.DestroyDisplayTiles(true);
            }
        }
    }
    protected virtual void FindEdges(Tile tile)
    {
        Stack<Tile> stack = new Stack<Tile>();
        HashSet<Tile> beenChecked = new HashSet<Tile>();

        stack.Push(tile);

        while (stack.Count > 0)
        {
            Tile t = stack.Pop();

            beenChecked.Add(t);
            List<Tile> neighbours = t.GetAdjacentNeigbours();
            
            foreach (Tile t2 in neighbours)
            {
                if (t2 != null && beenChecked.Contains(t2) == false && tiles.Contains(t2) == true)
                {
                    stack.Push(t2);

                    continue;
                }

                float temp = Time.realtimeSinceStartup;
                if (t2.IsObjectInstalled() == true)
                {
                    //UpdateDict(t2.installedObject);

                    if(t2.IsAccessible() == Accessibility.IMPASSABLE)
                    {
                        searchTiles.Add(t2);
                    }
                }

                CheckIfBorder(t);
            }
        }

        UpdateSpans();
    }
    public void UpdateSpans()
    {
        SortSpans(_northPairs, 0);
        SortSpans(_eastPairs, 1);
        SortSpans(_southPairs, 0);
        SortSpans(_westPairs, 1);

        if (_spans.Count > 0)
        {
            foreach (int i in _spans)
            {
                GameManager.GetRegionManager().AddHash(i, this);
            }
        }
    }
    void SortSpans(List<Tile> spanPairs, int isVertical)
    {
        if (spanPairs.Count == 0)
        {
            return;
        }

        List<Tile> sortedTiles = SortSpanList(new List<Tile>(spanPairs), isVertical);

        int index = 0;
        int tempIndex = 0;

        int length = 1;
        int x = 0;
        int y = 0;
        int hashX = 0;
        int hashY = 0;
        int hash = 0;

        for (int i = 1; i < sortedTiles.Count; i++)
        {
            if (sortedTiles[i - 1].IsAdjacent(sortedTiles[i]) == false)
            {
                index++;
            }

            if (tempIndex != index)
            {
                if (length == 1 && x == 0 && y == 0)
                {
                    x = sortedTiles[i - 1].x;
                    y = sortedTiles[i - 1].y;
                }

                hashX = Mathf.FloorToInt(x / length);
                hashY = Mathf.FloorToInt(y / length);

                hash = GenerateLinkHash(hashX, hashY, isVertical, length);
                _spans.Add(hash);

                tempIndex = index;
                x = 0;
                y = 0;
                length = 1;
            }
            else
            {
                x += sortedTiles[i - 1].x;
                y += sortedTiles[i - 1].y;
                length++;
            }
        }

        if(length == 1 && x == 0 && y == 0)
        {
            if(sortedTiles.Count == 1)
            {
                x = sortedTiles[0].x;
                y = sortedTiles[0].y;
            }
            else
            {
                x = sortedTiles[sortedTiles.Count - 1].x;
                y = sortedTiles[sortedTiles.Count - 1].y;
            }
        }

        hashX = Mathf.FloorToInt(x / length);
        hashY = Mathf.FloorToInt(y / length);

        hash = GenerateLinkHash(hashX, hashY, isVertical, length);
        _spans.Add(hash);
    }
    List<Tile> SortSpanList(List<Tile> toSort, int isVertical)
    {
        List<Tile> sortedTiles = new List<Tile>();

        while (toSort.Count > 0)
        {
            Tile first = null;
            int posCheck = 9999;

            for (int i = 0; i < toSort.Count; i++)
            {
                if (isVertical == 1)
                {
                    if (toSort[i].y < posCheck)
                    {
                        posCheck = toSort[i].y;
                        first = toSort[i];
                    }
                }
                else
                {
                    if (toSort[i].x < posCheck)
                    {
                        posCheck = toSort[i].x;
                        first = toSort[i];
                    }
                }
            }

            if(first == null)
            {
                continue;
            }

            sortedTiles.Add(first);
            toSort.Remove(first);
        }

        return sortedTiles;
    }
    public void Delete()
    {
        if(_spans.Count > 0)
        {
            foreach(int i in _spans)
            {
                GameManager.GetRegionManager().RemoveHash(i, this);
            }
        }

        if(tiles.Count > 0)
        {
            foreach (Tile tile in tiles)
            {
                tile.region = null;

                if(tile.displayRegion == true)
                {
                    tile.displayRegion = false;
                    tile.UpdateVisual();
                }
            }
        }

        tiles.Clear();
        searchTiles.Clear();

        _northPairs.Clear();
        _eastPairs.Clear();
        _southPairs.Clear();
        _westPairs.Clear();

        _spans.Clear();

        _itemsInRegion.Clear();
        _buildingsInRegion.Clear();
        _oreInRegion.Clear();
        _plantsInRegion.Clear();
    }
    public HashSet<Region> GetNeighbours()
    {
        HashSet<Region> neighbours = new HashSet<Region>();

        foreach (int i in _spans)
        {
            Region neighbour = GameManager.GetRegionManager().GetNeighbour(i, this);

            if(neighbour != null)
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }
    public void AddTile(Tile tile)
    {
        if(tile == null || tiles.Contains(tile) == true)
        {
            return;
        }

        tile.region = this;
        tiles.Add(tile);
    }
    void CheckIfBorder(Tile t)
    {
        Tile toCheck = t.North;

        if (toCheck != null && toCheck.IsAccessible() != Accessibility.IMPASSABLE && toCheck.region != this)
        {
            _northPairs.Add(toCheck);
        }

        toCheck = t.East;

        if (toCheck != null && toCheck.IsAccessible() != Accessibility.IMPASSABLE && toCheck.region != this)
        {
            _eastPairs.Add(toCheck);
        }

        toCheck = t.South;

        if (toCheck != null && toCheck.IsAccessible() != Accessibility.IMPASSABLE && toCheck.region != this)
        {
            _southPairs.Add(t);
        }

        toCheck = t.West;

        if (toCheck != null && toCheck.IsAccessible() != Accessibility.IMPASSABLE && toCheck.region != this)
        {
            _westPairs.Add(t);
        }
    }
    public void UpdateRegion()
    {
        FindEdges(tiles.First());
        _itemsInRegion.Clear();

        foreach (Tile tile in tiles)
        {
            if (tile.inventory.item != null)
            {
                UpdateDict(tile.inventory.item, tile.inventory.stackSize);
            }

            if (tile.IsObjectInstalled() == true)
            {
                UpdateDict(tile.installedObject);
            }

            searchTiles.Add(tile);
        }
    }
    public void UpdateDict(ItemData type, int amount)
    {
        if (_itemsInRegion.ContainsKey(type) == false)
        {
            if (amount < 0)
            {
                Debug.Log("Invalid Update Request");
                return;
            }

            _itemsInRegion.Add(type, amount);
            return;
        }

        _itemsInRegion[type] += amount;
       
        if (_itemsInRegion[type] <= 0)
        {
            _itemsInRegion.Remove(type);
        }
    }
    public void UpdateDict(InstalledObject obj)
    {
        switch(obj.type)
        {
            case InstalledObjectType.FURNITURE:

                if (_buildingsInRegion.ContainsKey((obj as Building)._data) == false)
                {
                    _buildingsInRegion.Add((obj as Building)._data, 1);
                    return;
                }

                _buildingsInRegion[(obj as Building)._data] += 1;

                if (_buildingsInRegion[(obj as Building)._data] <= 0)
                {
                    _buildingsInRegion.Remove((obj as Building)._data);
                }

                break;

            case InstalledObjectType.ORE:

                if (_oreInRegion.ContainsKey((obj as Ore)._data) == false)
                {
                    _oreInRegion.Add((obj as Ore)._data, 1);
                    return;
                }

                _oreInRegion[(obj as Ore)._data] += 1;

                if (_oreInRegion[(obj as Ore)._data] <= 0)
                {
                    _oreInRegion.Remove((obj as Ore)._data);
                }

                break;

            case InstalledObjectType.PLANT:

                if (_plantsInRegion.ContainsKey((obj as Plant)._data) == false)
                {
                    _plantsInRegion.Add((obj as Plant)._data, 1);
                    return;
                }

                _plantsInRegion[(obj as Plant)._data] += 1;

                if (_plantsInRegion[(obj as Plant)._data] <= 0)
                {
                    _plantsInRegion.Remove((obj as Plant)._data);
                }

                break;
        }
    }
    public int Contains(ItemData type)
    {
        if (_itemsInRegion.ContainsKey(type) == false)
        {
            return 0;
        }

        return _itemsInRegion[type];
    }
    public bool Contains(Tile tile)
    {
        return tiles.Contains(tile);
    }
    public int GetItemDictSize()
    {
        return _itemsInRegion.Count;
    }
    public bool ContainsUnstoredItem(ItemData type = null)
    {
        if(_itemsInRegion.Count == 0)
        {
            return false;
        }

        if(type != null)
        {
            if(_itemsInRegion.ContainsKey(type) == false)
            {
                return false;
            }
        }

        foreach(Tile tile in tiles)
        {
            if(type == null)
            {
                if (tile.inventory.item != null && tile.inventory.isQueried == false && tile.inventory.isStored == false)
                {
                    return true;
                }
            }
            else
            {
                if (tile.inventory.item == type && tile.inventory.isQueried == false && tile.inventory.isStored == false)
                {
                    return true;
                }
            }
        }

        return false;
    }
    public bool ContainsTask(TaskType type)
    {
        foreach (Tile tile in searchTiles)
        {
            if(tile.task == null || tile.task.taskType != type || tile.task.worker != null)
            {
                continue;
            }

            return true;
        }

        return false;
    }
    public Task GetClosestTask(TaskType type, Tile start)
    {
        HashSet<Tile> toCheck = new HashSet<Tile>(tiles);

        foreach (Tile t in searchTiles)
        {
            toCheck.Add(t);
        }

        float lowestDist = Mathf.Infinity;
        Task closest = null;

        foreach (Tile tile in toCheck)
        {
            if (tile.task == null || tile.task.taskType != type || tile.task.worker != null)
            {
                continue;
            }

            int distX = Mathf.Abs(start.x - tile.x);
            int distY = Mathf.Abs(start.y - tile.y);

            if (lowestDist > (distX + distY))
            {
                closest = tile.task;
                lowestDist = distX + distY;
            }
        }

        return closest;
    }
    public int GenerateLinkHash(int x, int y, int direction, int length)
    {
        return (x << 17) | (y << 5) | (direction << 4) | length;
    }

    public void ExtractValuesFromHash(int hash, out int x, out int y, out int direction, out int length)
    {
        x = (hash >> 17) & 0xFFF;
        y = (hash >> 5) & 0xFFF;
        direction = (hash >> 4) & 0x1;
        length = hash & 0xF;
    }

    public bool ContainsValidStorage(ItemData type, int amount)
    {
        foreach(Tile tile in tiles)
        {
            if(tile.inventory.isStored == false)
            {
                continue;
            }

            if (tile.inventory.CanBeStored(type, amount) > 0)
            {
                return true;
            }
        }

        return false;
    }
}
