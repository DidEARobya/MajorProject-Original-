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
    public int x;
    public int y;

    public float gCost;
    public float hCost;
    public float fCost;

    public HashSet<Region> neighbours = new HashSet<Region>();

    public HashSet<Tile> tiles = new HashSet<Tile>();
    public HashSet<Tile> impassableTiles = new HashSet<Tile>();
    protected HashSet<Tile> borderTiles = new HashSet<Tile>();

    List<Tile> northPairs = new List<Tile>();
    List<Tile> eastPairs = new List<Tile>();
    List<Tile> southPairs = new List<Tile>();
    List<Tile> westPairs = new List<Tile>();

    HashSet<int> spans = new HashSet<int>();

    Dictionary<ItemTypes, int> itemsInRegion = new Dictionary<ItemTypes, int>();
    Dictionary<FurnitureTypes, int> furnitureInRegion = new Dictionary<FurnitureTypes, int>();
    Dictionary<OreTypes, int> oreInRegion = new Dictionary<OreTypes, int>();
    Dictionary<PlantTypes, int> plantsInRegion = new Dictionary<PlantTypes, int>();

    public Cluster inCluster;
    public bool isDoor;
    public Region(Cluster _inCluster)
    {
        inCluster = _inCluster;
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

        foreach(Region r in neighbours)
        {
            if(r != null)
            {
                r.HighlightTiles(UnityEngine.Color.red, true);
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

        foreach (Region r in neighbours)
        {
            if (r != null)
            {
                r.DestroyDisplayTiles(true);
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
            Dictionary<Tile, Direction> neighbours = t.GetNeighboursDict();

            foreach (Tile t2 in neighbours.Keys)
            {
                if (t2 != null && beenChecked.Contains(t2) == false && tiles.Contains(t2) == true)
                {
                    stack.Push(t2);

                    continue;
                }

                if(t2.IsObjectInstalled() == true)
                {
                    UpdateDict(t2.installedObject);

                    if(t2.IsAccessible() == Accessibility.IMPASSABLE)
                    {
                        impassableTiles.Add(t2);
                    }
                }

                CheckIfBorder(t);
            }
        }

        GameManager.GetRegionManager().regions.Add(this);

        if(tiles.Count == -1)
        {
            return;
        }

        UpdateSpans();
    }
    public void UpdateSpans()
    {
        SortSpans(northPairs, 0);
        SortSpans(eastPairs, 1);
        SortSpans(southPairs, 0);
        SortSpans(westPairs, 1);

        if (spans.Count > 0)
        {
            foreach (int i in spans)
            {
                GameManager.GetRegionManager().AddHash(i, this);
            }
        }
    }
    public virtual void SetNeighbours()
    {
        if(tiles.Count == -1)
        {
            Tile tile = tiles.First();

            foreach (Tile t in tile.GetAdjacentNeigbours())
            {
                if (t.region == null)
                {
                    continue;
                }

                neighbours.Add(t.region);
                t.region.neighbours.Add(this);
            }

            return;
        }

        if(spans.Count == 0)
        {
            return;
        }

        foreach (int i in spans)
        {
            Region neighbour = GameManager.GetRegionManager().GetNeighbour(i, this);

            if(neighbour != null && neighbours.Contains(neighbour) == false)
            {
                neighbours.Add(neighbour);
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
                spans.Add(hash);

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
        spans.Add(hash);
    }
    List<Tile> SortSpanList(List<Tile> toSort, int isVertical)
    {
        List<Tile> sortedTiles = new List<Tile>();
        List<Tile> doorTiles = new List<Tile>();

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

        if(doorTiles.Count != 0)
        {
            foreach(Tile tile in doorTiles)
            {
                sortedTiles.Add(tile);
            }
        }

        return sortedTiles;
    }
    public void Delete()
    {
        if(spans.Count > 0)
        {
            foreach(int i in spans)
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

        GameManager.GetRegionManager().regions.Remove(this);

        neighbours.Clear();
        tiles.Clear();
        spans.Clear();
        borderTiles.Clear();
        itemsInRegion.Clear();
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

        if (toCheck != null && toCheck.IsAccessible() != Accessibility.IMPASSABLE && toCheck.region != this && (toCheck.region as DoorRegion) == null)
        {
            if (northPairs.Contains(toCheck) == false)
            {
                northPairs.Add(toCheck);
                borderTiles.Add(toCheck);
            }
        }

        toCheck = t.East;

        if (toCheck != null && toCheck.IsAccessible() != Accessibility.IMPASSABLE && toCheck.region != this && (toCheck.region as DoorRegion) == null)
        {
            if (eastPairs.Contains(toCheck) == false)
            {
                eastPairs.Add(toCheck);
                borderTiles.Add(toCheck);
            }
        }

        toCheck = t.South;

        if (toCheck != null && toCheck.IsAccessible() != Accessibility.IMPASSABLE && toCheck.region != this && (toCheck.region as DoorRegion) == null)
        {
            if (southPairs.Contains(t) == false)
            {
                southPairs.Add(t);
                borderTiles.Add(t);
            }
        }

        toCheck = t.West;

        if (toCheck != null && toCheck.IsAccessible() != Accessibility.IMPASSABLE && toCheck.region != this && (toCheck.region as DoorRegion) == null)
        {
            if (westPairs.Contains(t) == false)
            {
                westPairs.Add(t);
                borderTiles.Add(t);
            }
        }
    }
    public void UpdateRegion()
    {
        FindEdges(tiles.First());

        itemsInRegion.Clear();

        int _x = 0;
        int _y = 0;

        foreach(Tile tile in tiles)
        {
            _x += tile.x;
            _y += tile.y;

            if(tile.inventory.item != null)
            {
                UpdateDict(tile.inventory.item, tile.inventory.stackSize);
            }

            if(tile.IsObjectInstalled() == true)
            {
                UpdateDict(tile.installedObject);
            }
        }

        x = Mathf.RoundToInt(_x / tiles.Count);
        y = Mathf.RoundToInt(_y / tiles.Count);
    }
    public void UpdateDict(ItemTypes type, int amount)
    {
        if (itemsInRegion.ContainsKey(type) == false)
        {
            if (amount < 0)
            {
                Debug.Log("Invalid Update Request");
                return;
            }

            itemsInRegion.Add(type, amount);
            return;
        }

        itemsInRegion[type] += amount;
       
        if (itemsInRegion[type] <= 0)
        {
            itemsInRegion.Remove(type);
        }
    }
    public void UpdateDict(InstalledObject obj)
    {
        switch(obj.type)
        {
            case InstalledObjectType.FURNITURE:

                if (furnitureInRegion.ContainsKey((obj as Furniture).furnitureType) == false)
                {
                    furnitureInRegion.Add((obj as Furniture).furnitureType, 1);
                    return;
                }

                furnitureInRegion[(obj as Furniture).furnitureType] += 1;

                if (furnitureInRegion[(obj as Furniture).furnitureType] <= 0)
                {
                    furnitureInRegion.Remove((obj as Furniture).furnitureType);
                }

                break;

            case InstalledObjectType.ORE:

                if (oreInRegion.ContainsKey((obj as Ore).oreType) == false)
                {
                    oreInRegion.Add((obj as Ore).oreType, 1);
                    return;
                }

                oreInRegion[(obj as Ore).oreType] += 1;

                if (oreInRegion[(obj as Ore).oreType] <= 0)
                {
                    oreInRegion.Remove((obj as Ore).oreType);
                }

                break;

            case InstalledObjectType.PLANT:

                if (plantsInRegion.ContainsKey((obj as Plant).plantType) == false)
                {
                    plantsInRegion.Add((obj as Plant).plantType, 1);
                    return;
                }

                plantsInRegion[(obj as Plant).plantType] += 1;

                if (plantsInRegion[(obj as Plant).plantType] <= 0)
                {
                    plantsInRegion.Remove((obj as Plant).plantType);
                }

                break;
        }
    }
    public int Contains(ItemTypes type)
    {
        if (itemsInRegion.ContainsKey(type) == false)
        {
            return 0;
        }

        return itemsInRegion[type];
    }
    public bool Contains(Tile tile)
    {
        return tiles.Contains(tile);
    }
    public int GetItemDictSize()
    {
        return itemsInRegion.Count;
    }
    public bool ContainsUnstoredItem(ItemTypes type = null)
    {
        if(itemsInRegion.Count == 0)
        {
            return false;
        }
        if(type != null)
        {
            if(itemsInRegion.ContainsKey(type) == false)
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
        HashSet<Tile> toCheck = new HashSet<Tile>(tiles);

        foreach(Tile t in impassableTiles)
        {
            toCheck.Add(t);
        }

        foreach (Tile tile in toCheck)
        {
            if(tile.task == null || tile.task.taskType != type)
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

        foreach (Tile t in impassableTiles)
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
}
