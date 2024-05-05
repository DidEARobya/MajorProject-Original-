using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : INodeData
{
    int size;

    public RegionNeighbour[] neighbours = new RegionNeighbour[4];
    Tile[,] tiles;
    HashSet<Tile> edges = new HashSet<Tile>();

    Dictionary<FurnitureTypes, int> furnitureInRegion = new Dictionary<FurnitureTypes, int>();
    Dictionary<OreTypes, int> oreInRegion = new Dictionary<OreTypes, int>();
    Dictionary<ItemTypes, int> itemsInRegion = new Dictionary<ItemTypes, int>();

    public int x;
    public int y;

    public Region(WorldGrid grid, int _x, int _y)
    {
        size = RegionManager.regionSize;
        tiles = new Tile[size, size];

        for(int x = 0; x < size; x++)
        {
            for(int y = 0; y < size; y++)
            {
                tiles[x, y] = grid.GetTile(x + (size * _x), y + (size * _y));
            }
        }

        x = _x;
        y = _y;

        SetEdges();
    }
    public void UpdateRegion()
    {
        furnitureInRegion.Clear();
        oreInRegion.Clear();
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

            if (tile.installedObject == null || tile.installedObject.isInstalled == false)
            {
                continue;
            }

            if (tile.installedObject.type == InstalledObjectType.FURNITURE)
            {
                if (furnitureInRegion.ContainsKey((tile.installedObject as Furniture).furnitureType) == false)
                {
                    furnitureInRegion.Add((tile.installedObject as Furniture).furnitureType, 1);
                }
                else
                {
                    furnitureInRegion[(tile.installedObject as Furniture).furnitureType] += 1;
                }
            }
            else
            {
                if (oreInRegion.ContainsKey((tile.installedObject as Ore).oreType) == false)
                {
                    oreInRegion.Add((tile.installedObject as Ore).oreType, 1);
                }
                else
                {
                    oreInRegion[(tile.installedObject as Ore).oreType] += 1;
                }
            }
        }
    }
    public void UpdateDict(FurnitureTypes type, int amount)
    {
        if (furnitureInRegion.ContainsKey(type) == false)
        {
            if (amount < 0)
            {
                Debug.Log("Invalid Update Request");
                return;
            }

            furnitureInRegion.Add(type, amount);
            return;
        }

        furnitureInRegion[type] += amount;

        if (furnitureInRegion[type] <= 0)
        {
            furnitureInRegion.Remove(type);
        }
    }
    public void UpdateDict(OreTypes type, int amount)
    {
        if (oreInRegion.ContainsKey(type) == false)
        {
            if (amount < 0)
            {
                Debug.Log("Invalid Update Request");
                return;
            }

            oreInRegion.Add(type, amount);
            return;
        }

        oreInRegion[type] += amount;

        if (oreInRegion[type] <= 0)
        {
            oreInRegion.Remove(type);
        }
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
    public int Contains(OreTypes type)
    {
        if(oreInRegion.ContainsKey(type) == false)
        {
            return 0;
        }

        return oreInRegion[type];
    }
    public int Contains(FurnitureTypes type)
    {
        if (furnitureInRegion.ContainsKey(type) == false)
        {
            return 0;
        }

        return furnitureInRegion[type];
    }
    public int Contains(ItemTypes type)
    {
        if (itemsInRegion.ContainsKey(type) == false)
        {
            return 0;
        }

        return itemsInRegion[type];
    }
    public void SetEdges()
    {
        if(edges.Count > 0)
        {
            edges.Clear();
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if(x == 0 || y == 0 || x == size - 1 || y == size - 1)
                {
                    if (tiles[x, y].IsAccessible() != Accessibility.IMPASSABLE)
                    {
                        edges.Add(tiles[x, y]);
                    }
                }
            }
        }
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
