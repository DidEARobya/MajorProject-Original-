using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Inventory
{
    public ItemTypes item;
    public InventoryOwner owner;
    public GameObject inventoryObject;

    public int stackSize;
    public int queriedAmount = 0;

    public bool isQueried = false;
    public bool isStored = false;

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
            RegionManager.UpdateRegionDict(RegionManager.GetRegionAtTile(owner.ownerTile), item, -stackSize);
        }
        if(inventory.owner.ownerTile != null)
        {
            RegionManager.UpdateRegionDict(RegionManager.GetRegionAtTile(inventory.owner.ownerTile), inventory.item, -inventory.stackSize);
        }

        item = inventory.item;
        stackSize = inventory.stackSize;
        queriedAmount = 0;

        if (owner.ownerTile != null)
        {
            RegionManager.UpdateRegionDict(RegionManager.GetRegionAtTile(owner.ownerTile), item, stackSize);
        }
        if (inventory.owner.ownerTile != null)
        {
            RegionManager.UpdateRegionDict(RegionManager.GetRegionAtTile(inventory.owner.ownerTile), inventory.item, inventory.stackSize);
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

        int toTake = ItemTypes.GetMaxStackSize(item) - stackSize;

        if(toTake >= inventory.stackSize)
        {
            stackSize += inventory.stackSize;

            if(owner.ownerTile != null)
            {
                RegionManager.UpdateRegionDict(RegionManager.GetRegionAtTile(owner.ownerTile), item, inventory.stackSize);
            }

            inventory.ClearInventory();
        }
        else
        {
            stackSize += toTake;
            inventory.stackSize -= toTake;

            if (owner.ownerTile != null)
            {
                RegionManager.UpdateRegionDict(RegionManager.GetRegionAtTile(owner.ownerTile), item, toTake);
            }
            if (inventory.owner.ownerTile != null)
            {
                RegionManager.UpdateRegionDict(RegionManager.GetRegionAtTile(inventory.owner.ownerTile), item, -toTake);
            }
        }
    }
    public void StoreItem(Inventory inventory, int amount)
    {
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
                RegionManager.UpdateRegionDict(RegionManager.GetRegionAtTile(owner.ownerTile), item, inventory.stackSize);
            }

            inventory.ClearInventory();
        }
        else
        {
            inventory.stackSize -= amount;
            stackSize += amount;

            if (owner.ownerTile != null)
            {
                RegionManager.UpdateRegionDict(RegionManager.GetRegionAtTile(owner.ownerTile), item, amount);
            }
            if (inventory.owner.ownerTile != null)
            {
                RegionManager.UpdateRegionDict(RegionManager.GetRegionAtTile(inventory.owner.ownerTile), item, -amount);
            }
        }
    }
    public void ClearInventory()
    {
        if(owner.ownerTile != null)
        {
            RegionManager.UpdateRegionDict(RegionManager.GetRegionAtTile(owner.ownerTile), item, -stackSize);
        }

        item = null;
        stackSize = 0;
        queriedAmount = 0;
    }
    public int StoreItem(ItemTypes _item, int amount)
    {
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

        if (owner.ownerTile != null)
        {
            RegionManager.UpdateRegionDict(RegionManager.GetRegionAtTile(owner.ownerTile), item, toStore);
        }

        return toReturn;
    }
    public int CanBeStored(ItemTypes _item, int amount)
    {
        if(_item == null)
        {
            return 0;
        }

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
