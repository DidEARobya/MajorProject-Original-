using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public ItemTypes item;
    public InventoryOwner owner;
    public GameObject inventoryObject;

    public int maxStackSize
    {
        get { return ItemTypes.GetMaxStackSize(item); }
    }

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

        int toTake = maxStackSize - stackSize;

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
    public void StoreItem(ItemTypes _item)
    {
        if (CanBeStored(_item, 1) == false)
        {
            return;
        }

        if (item == null)
        {
            item = _item;
        }

        stackSize += 1;
    }
    public void StoreItem(ItemTypes _item, int amount)
    {
        if (CanBeStored(_item, amount) == false)
        {
            return;
        }

        if (item == null)
        {
            item = _item;
        }

        stackSize += amount;
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

        if(stackSize + inventory.stackSize > maxStackSize)
        {
            return false;
        }

        return true;
    }
    public bool CanBeStored(ItemTypes _item, int amount)
    {
        if(item == null)
        {
            return true;
        }

        if (item != _item)
        {
            return false;
        }

        if (stackSize + amount >= maxStackSize)
        {
            return false;
        }

        return true;
    }
}
