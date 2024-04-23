using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryManager
{
    static Action<Inventory> inventoryUpdateCallback;
    public static void CreateNewInventory(InventoryOwnerType type, Tile tile = null, CharacterController character = null)
    {
        Inventory inventory = new Inventory(); ;

        switch (type)
        {
            case InventoryOwnerType.TILE:

                if(tile == null)
                {
                    Debug.Log("Invalid Tile Inventory creation");
                    return;
                }

                tile.inventory = inventory;
                tile.ownerTile = tile;
                tile.inventory.owner = tile;
                break;

            case InventoryOwnerType.CHARACTER:

                if (character == null)
                {
                    Debug.Log("Invalid Character Inventory creation");
                    return;
                }

                character.inventory = inventory;
                character.ownerCharacter = character;
                character.inventory.owner = character;
                break;
        }
    }
    public static void AddToTileInventory(ItemTypes type, Tile tile)
    {
        tile.inventory.StoreItem(type);

        CharacterController character = CharacterManager.characters[0];
        Task task = new Task(tile, (t) => { PickUp(character, tile); }, TaskType.CONSTRUCTION);

        TaskManager.AddTask(task, task.taskType);

        if (inventoryUpdateCallback != null)
        {
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
    public static void SetInventoryUpdateCallback(Action<Inventory> callback)
    {
        inventoryUpdateCallback += callback;
    }
    public static void RemoveInventoryUpdateCallback(Action<Inventory> callback)
    {
        inventoryUpdateCallback -= callback;
    }
}
