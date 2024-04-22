using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public DroppedObject item;
    public int maxStackSize
    {
        get { return DroppedObjectTypes.GetMaxStackSize(item.type); }
    }

    public int stackSize;

    public Inventory()
    {

    }
    public void StoreItem(Inventory inventory)
    {
        if(CanBeStored(inventory) == false)
        {
            return;
        }

        if(item == null)
        {
            item = inventory.item;
        }

        stackSize += inventory.stackSize;
        inventory.item.Destroy();
        inventory.item = null;
    }
    public void StoreItem(DroppedObject _item)
    {
        if (CanBeStored(_item) == false)
        {
            return;
        }

        if (item == null)
        {
            item = _item;
        }
        else
        {
            _item.Destroy();
        }

        stackSize += 1;
    }
    public bool CanBeStored(Inventory inventory)
    {
        if (item == null)
        {
            return true;
        }

        if (item.type != inventory.item.type)
        {
            return false;   
        }

        if(stackSize + inventory.stackSize > maxStackSize)
        {
            return false;
        }

        return true;
    }
    public bool CanBeStored(DroppedObject _item)
    {
        if(item == null)
        {
            return true;
        }

        if (item.type != _item.type)
        {
            return false;
        }

        if (stackSize >= maxStackSize)
        {
            return false;
        }

        return true;
    }
}
