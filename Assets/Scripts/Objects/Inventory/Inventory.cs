using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory
{
    public ItemTypes item;
    public InventoryOwner owner;
    public GameObject inventoryObject;

    public int stackSize;

    public Inventory()
    {

    }
    protected Inventory(Inventory toClone)
    {
        item = toClone.item;
        owner = toClone.owner;
        stackSize = toClone.stackSize;
        inventoryObject = toClone.inventoryObject;
    }
    public Inventory CloneInventory()
    {
        return new Inventory(this);
    }
    public void ReplaceInventory(Inventory inventory)
    {
        item = inventory.item;
        stackSize = inventory.stackSize;
    }
    public void StoreItem(Inventory inventory)
    {
        if(CanBeStored(inventory) == false)
        {
            return;
        }

        if(item == null)
        {
            stackSize = 0;
            item = inventory.item;
        }

        int toTake = ItemTypes.GetMaxStackSize(item) - stackSize;

        if(toTake >= inventory.stackSize)
        {
            stackSize += inventory.stackSize;
            inventory.item = null;
        }
        else
        {
            stackSize += toTake;
            inventory.stackSize -= toTake;
        }
    }
    public void StoreItem(Inventory inventory, int amount)
    {
        if (CanBeStored(inventory) == false)
        {
            return;
        }

        if (item == null)
        {
            stackSize = 0;
            item = inventory.item;
        }

        if (amount >= inventory.stackSize)
        {
            stackSize += inventory.stackSize;
            inventory.item = null;
        }
        else
        {
            stackSize += amount;
            inventory.stackSize -= amount;
        }
    }
    public void ClearInventory()
    {
        item = null;
        stackSize = 0;
    }
    public int StoreItem(ItemTypes _item, int amount)
    {
        if(amount == 60)
        {

        }

        int toStore = CanBeStored(_item, amount);

        if(toStore == 0)
        {
            return amount;
        }

        int toReturn = amount - toStore;

        if (item == null)
        {
            item = _item;
        }

        stackSize += toStore;
        return toReturn;
    }
    public bool CanBeStored(Inventory inventory)
    {
        if (item == null)
        {
            return true;
        }

        if (item != inventory.item)
        {
            return false;
        }

        if(stackSize + inventory.stackSize > ItemTypes.GetMaxStackSize(inventory.item))
        {
            return false;
        }

        return true;
    }
    public int CanBeStored(ItemTypes _item, int amount)
    {
        if (item != null && item != _item)
        {
            return 0;
        }

        if (stackSize + amount >= ItemTypes.GetMaxStackSize(_item))
        {
            return ItemTypes.GetMaxStackSize(_item) - stackSize;
        }

        return amount;
    }
}
