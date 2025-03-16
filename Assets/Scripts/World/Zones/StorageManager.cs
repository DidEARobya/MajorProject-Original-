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

    public static Tile GetClosestAcceptableInventory(Tile itemTile, ItemData itemType, int amount)
    {
        BFS_Search search = new BFS_Search();
        Region toCheck = search.GetClosestRegionWithStorage(GameManager.GetRegionManager().GetRegionAtTile(itemTile), true, itemType, amount);

        search = null;

        if (toCheck == null)
        {
            return null;
        }

        float lowestDist = Mathf.Infinity;
        Tile closest = null;

        foreach (Tile tile in toCheck.searchTiles)
        {
            if (tile.inventory.isStored == false || tile.inventory.CanBeStored(itemType, amount) == 0)
            {
                continue;
            }

            int distX = Mathf.Abs(itemTile.x - tile.x);
            int distY = Mathf.Abs(itemTile.y - tile.y);

            if (lowestDist > (distX + distY))
            {
                closest = tile;
                lowestDist = distX + distY;
            }
        }

        if (closest == null)
        {
            return null;
        }

        return closest;
    }
}
