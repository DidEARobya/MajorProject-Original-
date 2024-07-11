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
    public static List<Inventory> inventories = new List<Inventory>();
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
        tile.inventory.queriedAmount -= amount;

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
            tile.inventory.queriedAmount -= tile.inventory.stackSize;

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

        if(inventory.owner.GetOwnerType() == InventoryOwnerType.CHARACTER)
        {
            return;
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

        if(inventory.stackSize > inventory.queriedAmount)
        {
            inventory.isQueried = false;
        }
    }
    public static Tile GetClosestValidItem(Tile start, ItemTypes itemType, int amount = 0)
    {
        if(inventories.Count == 0)
        {
            return null;
        }

        BFS_Search search = new BFS_Search();
        Region toCheck = search.GetClosestRegionWithItem(GameManager.GetRegionManager().GetRegionAtTile(start), true, false, itemType);

        search = null;

        if(toCheck == null)
        {
            return null;
        }

        float lowestDist = Mathf.Infinity;

        Tile closest = null;

        foreach(Tile tile in toCheck.tiles)
        {
            if (tile.inventory.item != itemType || tile.inventory.isQueried == true)
            {
                continue;
            }

            int distX = Mathf.Abs(start.x - tile.x);
            int distY = Mathf.Abs(start.y - tile.y);

            if (lowestDist > (distX + distY))
            {
                closest = tile;
                lowestDist = distX + distY;
            }
        }

        if(closest == null)
        {
            return null;
        }

        Inventory inventory = closest.inventory;

        if (amount == 0)
        {
            inventory.isQueried = true;
        }
        else
        {
            inventory.queriedAmount += amount;

            if (inventory.queriedAmount >= inventory.stackSize)
            {
                inventory.isQueried = true;
            }
        }

        return closest;
    }
    public static Tile GetClosestValidItem(Tile start, bool checkStored)
    {
        if (inventories.Count == 0)
        {
            return null;
        }

        BFS_Search search = new BFS_Search();
        Region toCheck;

        if (checkStored == false)
        {
            toCheck = search.GetClosestRegionWithItem(GameManager.GetRegionManager().GetRegionAtTile(start), true, true);
        }
        else
        {
            toCheck = search.GetClosestRegionWithItem(GameManager.GetRegionManager().GetRegionAtTile(start), true);
        }
        
        search = null;

        if (toCheck == null)
        {
            return null;
        }

        float lowestDist = Mathf.Infinity;
        Tile closest = null;

        foreach (Tile tile in toCheck.tiles)
        {
            if (tile.inventory.item == null || tile.inventory.isQueried == true)
            {
                continue;
            }
            if (checkStored == false && tile.inventory.isStored == true)
            {
                continue;
            }

            int distX = Mathf.Abs(start.x - tile.x);
            int distY = Mathf.Abs(start.y - tile.y);

            if (lowestDist > (distX + distY))
            {
                closest = tile;
                lowestDist = distX + distY;
            }
        }

        if(closest == null)
        {
            return null;
        }

        Inventory inventory = closest.inventory;
        inventory.isQueried = true;

        return closest;
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