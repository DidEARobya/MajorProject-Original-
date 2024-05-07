using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Region : INodeData
{
    public RegionNeighbour[] neighbours = new RegionNeighbour[4];

    public HashSet<Tile> tiles = new HashSet<Tile>();
    public List<Tile> borderTiles = new List<Tile>();

    Dictionary<ItemTypes, int> itemsInRegion = new Dictionary<ItemTypes, int>();

    public Region()
    {
       
    }
    public void Delete()
    {
        foreach(Tile tile in tiles)
        {
            tile.region = null;
        }

        neighbours = null;
        tiles = null;
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

        Debug.Log(itemsInRegion.Count);
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
            Debug.Log(itemsInRegion[type]);
            return;
        }

        itemsInRegion[type] += amount;
        Debug.Log(itemsInRegion[type]);
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
}
public struct RegionNeighbour
{
    public Region neighbour;
    public Direction direction;

    public RegionNeighbour(Region _neighbour, Direction _direction)
    {
        neighbour = _neighbour;
        direction = _direction;
    }
}
