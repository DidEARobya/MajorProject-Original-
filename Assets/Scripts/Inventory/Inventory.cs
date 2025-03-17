using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Inventory
{
    public ItemData item;
    public InventoryOwner owner;
    public GameObject inventoryObject;

    public int stackSize;
    public int queriedAmount = 0;

    public bool isQueried = false;
    public bool isStored = false;

    public int toBeStored = 0;

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
        if(owner.ownerTile != null)
        {
            GameManager.GetRegionManager().UpdateRegionDict(GameManager.GetRegionManager().GetRegionAtTile(owner.ownerTile), item, -stackSize);
        }
        if(inventory.owner.ownerTile != null)
        {
            GameManager.GetRegionManager().UpdateRegionDict(GameManager.GetRegionManager().GetRegionAtTile(inventory.owner.ownerTile), inventory.item, -inventory.stackSize);
        }

        item = inventory.item;
        stackSize = inventory.stackSize;
        queriedAmount = 0;

        if (owner.ownerTile != null)
        {
            GameManager.GetRegionManager().UpdateRegionDict(GameManager.GetRegionManager().GetRegionAtTile(owner.ownerTile), item, stackSize);
        }
        if (inventory.owner.ownerTile != null)
        {
            GameManager.GetRegionManager().UpdateRegionDict(GameManager.GetRegionManager().GetRegionAtTile(inventory.owner.ownerTile), inventory.item, inventory.stackSize);
        }
    }
    public void StoreItem(Inventory inventory)
    {
        if (item != null && inventory.item != item)
        {
            return;
        }

        if (item == null)
        {
            item = inventory.item;
        }

        int toTake = item.maxStackSize - stackSize;

        if(toTake >= inventory.stackSize)
        {
            stackSize += inventory.stackSize;

            if(owner.ownerTile != null)
            {
                GameManager.GetRegionManager().UpdateRegionDict(GameManager.GetRegionManager().GetRegionAtTile(owner.ownerTile), item, inventory.stackSize);
            }

            inventory.ClearInventory();
        }
        else
        {
            stackSize += toTake;
            inventory.stackSize -= toTake;

            if (owner.ownerTile != null)
            {
                GameManager.GetRegionManager().UpdateRegionDict(GameManager.GetRegionManager().GetRegionAtTile(owner.ownerTile), item, toTake);
            }
            if (inventory.owner.ownerTile != null)
            {
                GameManager.GetRegionManager().UpdateRegionDict(GameManager.GetRegionManager().GetRegionAtTile(inventory.owner.ownerTile), item, -toTake);
            }
        }
    }
    public void StoreItem(Inventory inventory, int amount)
    {
        toBeStored -= amount;

        if (item != null && inventory.item != item)
        {
            return;
        }

        if (item == null)
        {
            item = inventory.item;
        }

        if (amount >= inventory.stackSize)
        {
            stackSize += inventory.stackSize;

            if (owner.ownerTile != null)
            {
                GameManager.GetRegionManager().UpdateRegionDict(GameManager.GetRegionManager().GetRegionAtTile(owner.ownerTile), item, inventory.stackSize);
            }

            inventory.ClearInventory();
        }
        else
        {
            inventory.stackSize -= amount;
            stackSize += amount;

            if (owner.ownerTile != null)
            {
                GameManager.GetRegionManager().UpdateRegionDict(GameManager.GetRegionManager().GetRegionAtTile(owner.ownerTile), item, amount);
            }
            if (inventory.owner.ownerTile != null)
            {
                GameManager.GetRegionManager().UpdateRegionDict(GameManager.GetRegionManager().GetRegionAtTile(inventory.owner.ownerTile), item, -amount);
            }
        }
    }
    public void ClearInventory()
    {
        if(owner.ownerTile != null)
        {
            GameManager.GetRegionManager().UpdateRegionDict(GameManager.GetRegionManager().GetRegionAtTile(owner.ownerTile), item, -stackSize);
        }

        item = null;
        stackSize = 0;
        queriedAmount = 0;
    }
    public int StoreItem(ItemData _item, int amount)
    {
        toBeStored -= amount;

        int toStore = CanBeStored(_item, amount);

        if (toStore == 0)
        {
            return amount;
        }

        int toReturn = amount - toStore;

        if (item == null)
        {
            item = _item;
        }

        stackSize += toStore;

        if (owner.ownerTile != null)
        {
            GameManager.GetRegionManager().UpdateRegionDict(GameManager.GetRegionManager().GetRegionAtTile(owner.ownerTile), item, toStore);
        }

        return toReturn;
    }
    public int CanBeStored(ItemData _item, int amount)
    {
        if(_item == null)
        {
            return 0;
        }

        if (item != null && item != _item)
        {
            return 0;
        }

        if (stackSize + toBeStored + amount >= _item.maxStackSize)
        {
            return _item.maxStackSize - (stackSize + toBeStored);
        }

        return amount;
    }
}
