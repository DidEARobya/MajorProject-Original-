using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
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
    }
    public static void DropInventory(Inventory inventory, Tile tile)
    {
        AddToTileInventory(inventory.item, tile, inventory.stackSize);

        inventory.ClearInventory();

        UpdateCallback(inventory);
    }
    public static void AddToTileInventory(ItemTypes type, Tile tile, int amount)
    {
        int excess = tile.inventory.StoreItem(type, amount);

        if(excess != 0)
        {
            Tile temp = tile.GetNearestAvailableInventory(type, excess);

            if (temp != null)
            {
                AddToTileInventory(type, temp, excess);
                UpdateCallback(temp.inventory);
            }
        }

        UpdateCallback(tile.inventory);
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
    public static void PickUp(CharacterController character, Tile tile, int amount = 0)
    {
        if(amount == 0)
        {
            PickUp(character, tile);
            return;
        }

        character.inventory.StoreItem(tile.inventory, amount);

        UpdateCallback(character.inventory);
        UpdateCallback(tile.inventory);
    }
    static void PickUp(CharacterController character, Tile tile)
    {
        if(character.inventory.item != tile.inventory.item)
        {
            SwitchInventories(character.inventory, tile.inventory);
            return;
        }
        else
        {
            character.inventory.StoreItem(tile.inventory);

            UpdateCallback(character.inventory);
            UpdateCallback(tile.inventory);
        }
    }
    public static void SwitchInventories(Inventory inventory1, Inventory inventory2)
    {
        Inventory temp1 = inventory1.CloneInventory();
        Inventory temp2 = inventory2.CloneInventory();

        inventory1.ReplaceInventory(temp2);
        inventory2.ReplaceInventory(temp1);

        UpdateCallback(inventory1);
        UpdateCallback(inventory2);
    }
    public static void ClearInventory(Inventory inventory)
    {
        inventory.ClearInventory();

        UpdateCallback(inventory);
    }
    static void UpdateCallback(Inventory inventory)
    {
        if (inventoryUpdateCallback != null)
        {
            inventoryUpdateCallback(inventory);
        }

        if(inventories.Contains(inventory) == false)
        {
            if(inventory.item != null)
            {
                inventories.Add(inventory);
            }
        }
        else
        {
            if(inventory.item == null || inventory.stackSize == 0)
            {
                inventories.Remove(inventory);
            }
        }

        inventory.isQueried = false;
    }
    public static TilePathPair GetClosestValidItem(Tile start, ItemTypes itemType, int amount = 0)
    {
        if(inventories.Count == 0)
        {
            return new TilePathPair(null, null);
        }

        float lowestDist = Mathf.Infinity;
        Stack<Tile> tileStack = new Stack<Tile>();

        for (int i = 0; i < inventories.Count; i++)
        {
            if (inventories[i].item != itemType)
            {
                continue;
            }

            Tile temp = GameManager.GetWorldGrid().GetTile(inventories[i].owner.x, inventories[i].owner.y);

            if(temp == null || temp.inventory.isQueried == true)
            {
                continue;
            }

            int distX = Mathf.Abs(start.x - temp.x);
            int distY = Mathf.Abs(start.y - temp.y);

            if (lowestDist > (distX + distY))
            {
                tileStack.Push(temp);
                lowestDist = distX + distY;
            }
        }

        if(tileStack.Count == 0)
        {
            return new TilePathPair(null, null);
        }

        while(tileStack.Count > 0)
        {
            Tile temp = tileStack.Pop();

            Path_AStar path = new Path_AStar(start, temp, true);

            if (path == null)
            {
                continue;
            }

            Inventory inventory = temp.inventory;

            if(amount == 0)
            {
                inventory.isQueried = true;
            }
            else
            {
                inventory.queriedAmount += amount;
                
                if (inventory.queriedAmount > inventory.stackSize)
                {
                    inventory.isQueried = true;

                    int remaining = inventory.queriedAmount -  inventory.stackSize;

                    if(remaining == 0)
                    {
                        continue;
                    }
                }
            }

            return new TilePathPair(temp, path);
        }

        return new TilePathPair(null, null);
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