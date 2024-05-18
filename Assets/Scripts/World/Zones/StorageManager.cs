using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class StorageManager
{
    public static HashSet<Tile> storageTiles = new HashSet<Tile>();

    public static void AddTile(Tile tile)
    {
        if (storageTiles.Contains(tile))
        {
            return;
        }

        tile.inventory.isStored = true;
        storageTiles.Add(tile);
    }

    public static void RemoveTile(Tile tile)
    {
        if(storageTiles.Contains(tile) == false)
        {
            return;
        }

        tile.inventory.isStored = false;
        storageTiles.Remove(tile);
    }

    public static Tile GetClosestAcceptableInventory(Tile itemTile, ItemTypes itemType, int amount)
    {
        Stack<Tile> stack = new Stack<Tile>();
        float lowestDist = Mathf.Infinity;

        foreach (Tile tile in storageTiles)
        {
            if(tile == null)
            {
                continue;
            }

            int distX = Mathf.Abs(itemTile.x - tile.x);
            int distY = Mathf.Abs(itemTile.y - tile.y);

            if (lowestDist > (distX + distY))
            {
                if (tile.inventory.CanBeStored(itemType, amount) > 0)
                {
                    stack.Push(tile);
                    lowestDist = distX + distY;
                }
            }
        }

        if (stack.Count == 0)
        {
            return null;
        }

        while (stack.Count > 0)
        {
            Tile temp = stack.Pop();

            Path_AStar path = new Path_AStar(itemTile, temp, true);

            if (path == null)
            {
                continue;
            }

            return temp;
        }

        return null;
    }
}
