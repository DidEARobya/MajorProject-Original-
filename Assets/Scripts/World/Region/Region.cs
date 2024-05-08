using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Region : INodeData
{
    public HashSet<Region> neighbours = new HashSet<Region>();

    public HashSet<Tile> tiles = new HashSet<Tile>();

    List<Tile> northPairs = new List<Tile>();
    List<Tile> eastPairs = new List<Tile>();
    List<Tile> southPairs = new List<Tile>();
    List<Tile> westPairs = new List<Tile>();

    public HashSet<int> spans = new HashSet<int>();
    Dictionary<ItemTypes, int> itemsInRegion = new Dictionary<ItemTypes, int>();

    public Region()
    {
        RegionManager.regions.Add(this);
    }
    public void SetTiles(TerrainTypes type, bool isNeighbour)
    {
        if(tiles.Count == 0 || tiles.First().terrainType == type)
        {
            return;
        }

        foreach(Tile tile in tiles)
        {
            tile.SetTerrainType(type);
        }

        if(isNeighbour == true)
        {
            return;
        }

        foreach(Region r in neighbours)
        {
            if(r != null)
            {
                r.SetTiles(TerrainTypes.POOR_SOIL, true);
            }
        }
    }
    void FindEdges(Tile tile)
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

                CheckIfBorder(t);
            }
        }

        SortSpans(northPairs, 0);
        SortSpans(eastPairs, 1);
        SortSpans(southPairs, 0);
        SortSpans(westPairs, 1);

        if(spans.Count > 0)
        {
            foreach(int i in spans)
            {
                RegionManager.AddHash(i, this);
            }
        }
    }
    public void SetNeighbours()
    {
        if(spans.Count == 0)
        {
            return;
        }

        foreach (int i in spans)
        {
            Region neighbour = RegionManager.GetNeighbour(i, this);

            if(neighbour != null && neighbours.Contains(neighbour) == false)
            {
                neighbours.Add(neighbour);
            }
        }

        Debug.Log(neighbours.Count);
    }
    void SortSpans(List<Tile> spanPairs, int isVertical)
    {
        if(spanPairs.Count == 0)
        {
            return;
        }

        List<Tile> spanTiles = new List<Tile>(spanPairs);
        List<Tile> sortedTiles = new List<Tile>();

        while(spanTiles.Count > 0)
        {
            Tile first = null;
            int posCheck = 9999;

            for(int i = 0; i < spanTiles.Count; i++)
            {
                if(isVertical == 1)
                {
                    if (spanTiles[i].y < posCheck)
                    {
                        posCheck = spanTiles[i].y;
                        first = spanTiles[i];
                    }
                }
                else
                {
                    if (spanTiles[i].x < posCheck)
                    {
                        posCheck = spanTiles[i].x;
                        first = spanTiles[i];
                    }
                }
            }

            sortedTiles.Add(first);
            spanTiles.Remove(first);
        }

        int spanCount = 0;
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
            if (sortedTiles[i - 1].IsNeighbour(sortedTiles[i]) == false)
            {
                index++;
                continue;
            }

            if(tempIndex != index)
            {
                hashX = Mathf.FloorToInt(x / length);
                hashY = Mathf.FloorToInt(y / length);

                hash = GenerateLinkHash(hashX, hashY, isVertical, length);
                spans.Add(hash);

                tempIndex = index;
                x = 0;
                y = 0;
                length = 1;

                spanCount++;
            }

            x += sortedTiles[i - 1].x;
            y += sortedTiles[i - 1].y;
            length++;
        }

        hashX = Mathf.FloorToInt(x / length);
        hashY = Mathf.FloorToInt(y / length);

        hash = GenerateLinkHash(hashX, hashY, isVertical, length);
        spans.Add(hash);
    }
    public void Delete()
    {
        if(spans.Count > 0)
        {
            foreach(int i in spans)
            {
                RegionManager.RemoveHash(i, this);
            }
        }

        if(tiles.Count > 0)
        {
            foreach (Tile tile in tiles)
            {
                tile.region = null;
            }
        }

        RegionManager.regions.Remove(this);

        neighbours = null;
        tiles.Clear();
        spans.Clear();
        itemsInRegion = null;
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

        if (toCheck != null && toCheck.IsObjectInstalled() == false && toCheck.IsAccessible() != Accessibility.IMPASSABLE && toCheck.region != this)
        {
            if (northPairs.Contains(toCheck) == false)
            {
                northPairs.Add(toCheck);
            }
        }

        toCheck = t.East;

        if (toCheck != null && toCheck.IsObjectInstalled() == false && toCheck.IsAccessible() != Accessibility.IMPASSABLE && toCheck.region != this)
        {
            if (eastPairs.Contains(toCheck) == false)
            {
                eastPairs.Add(toCheck);
            }
        }

        toCheck = t.South;

        if (toCheck != null && toCheck.IsObjectInstalled() == false && toCheck.IsAccessible() != Accessibility.IMPASSABLE && toCheck.region != this)
        {
            if (southPairs.Contains(t) == false)
            {
                southPairs.Add(t);
            }
        }

        toCheck = t.West;

        if (toCheck != null && toCheck.IsObjectInstalled() == false && toCheck.IsAccessible() != Accessibility.IMPASSABLE && toCheck.region != this)
        {
            if (westPairs.Contains(t) == false)
            {
                westPairs.Add(t);
            }
        }
    }
    public void UpdateRegion()
    {
        itemsInRegion.Clear();

        foreach(Tile tile in tiles)
        {
            if(tile.inventory.item != null)
            {
                if (itemsInRegion.ContainsKey(tile.inventory.item) == false)
                {
                    itemsInRegion.Add(tile.inventory.item, tile.inventory.stackSize);
                }
                else
                {
                    itemsInRegion[tile.inventory.item] += tile.inventory.stackSize;
                }
            }
        }

        FindEdges(tiles.First());
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

    public Accessibility IsAccessible()
    {
        return Accessibility.ACCESSIBLE;
    }
    public int GetCost(bool isPlayer)
    {
        return 0;
    }
    public void SetNode(Node node)
    {

    }

    public Region GetRegion()
    {
        return this;
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
