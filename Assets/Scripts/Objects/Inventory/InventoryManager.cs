using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public static class InventoryManager
{
    static List<Inventory> inventories = new List<Inventory>();
    static Action<Inventory> inventoryUpdateCallback;
    public static void CreateNewInventory(InventoryOwnerType type, Tile tile = null, CharacterController character = null)
    {
        Inventory inventory = new Inventory(); ;

        switch (type)
        {
            case InventoryOwnerType.TILE:

                tile.inventory = inventory;
                tile.ownerTile = tile;
                tile.inventory.owner = tile;
                break;

            case InventoryOwnerType.CHARACTER:

                character.inventory = inventory;
                character.ownerCharacter = character;
                character.inventory.owner = character;
                break;
        }

        inventories.Add(inventory);
    }
    public static void AddToTileInventory(ItemTypes type, Tile tile, int amount)
    {
        int excess = tile.inventory.StoreItem(type, amount);

        if(excess != 0)
        {
            Tile temp = tile.GetNearestAvailableInventory(type, excess);

            if (temp != null)
            {
                temp.inventory.StoreItem(type, excess);

                if (inventoryUpdateCallback != null)
                {
                    inventoryUpdateCallback(temp.inventory);
                }
            }
        }

        if (inventoryUpdateCallback != null)
        {
            inventoryUpdateCallback(tile.inventory);
        }

        CharacterManager.ResetCharacterTaskIgnores();
    }
    public static void AddToTileInventory(Tile tile, Dictionary<ItemTypes, int> toDrop)
    {
        if(toDrop == null)
        {
            return;
        }

        foreach (ItemTypes type in toDrop.Keys)
        {
            AddToTileInventory(type, tile, toDrop[type]);
        }
    }
    public static void PickUp(CharacterController character, Tile tile, int amount)
    {
        character.inventory.StoreItem(tile.inventory, amount);

        if (inventoryUpdateCallback != null)
        {
            inventoryUpdateCallback(character.inventory);
            inventoryUpdateCallback(tile.inventory);
        }
    }
    public static void PickUp(CharacterController character, Tile tile)
    {
        if(character.inventory.item != tile.inventory.item)
        {
            SwitchInventories(character.inventory, tile.inventory);
            return;
        }
        else
        {
            character.inventory.StoreItem(tile.inventory);

            if (inventoryUpdateCallback != null)
            {
                inventoryUpdateCallback(character.inventory);
                inventoryUpdateCallback(tile.inventory);
            }
        }
    }
    public static void SwitchInventories(Inventory inventory1, Inventory inventory2)
    {
        Inventory temp1 = inventory1.CloneInventory();
        Inventory temp2 = inventory2.CloneInventory();

        inventory1.ReplaceInventory(temp2);
        inventory2.ReplaceInventory(temp1);

        if (inventoryUpdateCallback != null)
        {
            inventoryUpdateCallback(inventory1);
            inventoryUpdateCallback(inventory2);
        }
    }
    public static void ClearInventory(Inventory inventory)
    {
        inventory.ClearInventory();

        if (inventoryUpdateCallback != null)
        {
            inventoryUpdateCallback(inventory);
        }
    }
    public static TilePathPair GetClosestValidItem(Tile start, ItemTypes itemType)
    {
        float lowestDist = Mathf.Infinity;
        Path_AStar path = null;
        Tile goal = null;

        for (int i = 0; i < inventories.Count; i++)
        {
            if (inventories[i].item != itemType)
            {
                continue;
            }

            Tile temp = GameManager.GetWorldGrid().GetTile(inventories[i].owner.x, inventories[i].owner.y);

            int distX = Mathf.Abs(start.x - temp.x);
            int distY = Mathf.Abs(start.y - temp.y);

            if (lowestDist > (distX + distY))
            {
                path = Utility.CheckIfTaskValid(start, temp, false);

                if (path != null)
                {
                    lowestDist = distX + distY;
                    goal = temp;
                }
            }
        }

        return new TilePathPair(goal, path);
    }
    public static void SetInventoryUpdateCallback(Action<Inventory> callback)
    {
        inventoryUpdateCallback += callback;
    }
    public static void RemoveInventoryUpdateCallback(Action<Inventory> callback)
    {
        inventoryUpdateCallback -= callback;
    }
}

public struct TilePathPair
{
    public Tile tile;
    public Path_AStar path;

    public TilePathPair(Tile _tile, Path_AStar _path)
    {
        tile = _tile;
        path = _path;
    }
}