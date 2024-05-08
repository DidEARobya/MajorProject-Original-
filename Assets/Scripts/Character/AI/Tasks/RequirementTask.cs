using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class RequirementTask : Task
{
    public Dictionary<ItemTypes, int> requirements;
    public Dictionary<ItemTypes, int> storedRequirements;

    FloorTypes floorType;

    bool isInitialised = false;

    public RequirementTask(Tile _tile, Action<Task> _taskCompleteCallback, TaskType type, Dictionary<ItemTypes, int> _requirements, bool _isFloor, float _taskTime = 1) : base(_tile, _taskCompleteCallback, type, _isFloor, _taskTime)
    {
        requirements = _requirements;

        storedRequirements = new Dictionary<ItemTypes, int>();

        if(isFloor == true)
        {
            floorType = tile.floorType;
            tile.SetFloorType(FloorTypes.TASK);
        }
    }
    public override void InitTask(CharacterController character)
    {
        if (character.inventory.item != null)
        {
            InventoryManager.DropInventory(character.inventory, character.currentTile);
        }

        base.InitTask(character);

        if (isInitialised == true)
        {
            return;
        }

        CreateHaulTask();
        isInitialised = true;
    }
    public override void DoWork(float workTime)
    {
        if (CheckIfRequirementsFulfilled() == false)
        {
            CreateHaulTask();
            return;
        }

        base.DoWork(workTime);
    }
    public void StoreComponent(Inventory inventory)
    {
        if(inventory.item == null)
        {
            return;
        }

        int amount = inventory.stackSize;

        if(requirements.ContainsKey(inventory.item) == false)
        {
            InventoryManager.DropInventory(worker.inventory, worker.currentTile);
            return;
        }

        if (storedRequirements.ContainsKey(inventory.item) == false)
        {
            storedRequirements.Add(inventory.item, amount);
            InventoryManager.ClearInventory(inventory);
            return;
        }

        if (storedRequirements[inventory.item] + amount > requirements[inventory.item])
        {
            int excess = (storedRequirements[inventory.item] + amount);

            inventory.stackSize -= amount;
            storedRequirements[inventory.item] = requirements[inventory.item];

            InventoryManager.AddToTileInventory(inventory.item, worker.currentTile, excess);
        }
        else
        {
            storedRequirements[inventory.item] += amount;
            InventoryManager.ClearInventory(inventory);
        }
    }
    void CreateHaulTask()
    {
        HaulTask task = null;

        foreach (ItemTypes item in requirements.Keys)
        {
            if (storedRequirements.ContainsKey(item) == false)
            {
                storedRequirements.Add(item, 0);
            }

            if (storedRequirements[item] != requirements[item])
            {
                int remaining = Mathf.Abs(storedRequirements[item] - requirements[item]);

                task = TaskManager.CreateHaulToJobSiteTask(this, worker, item, tile, remaining);
                break;
            }
        }

        if (task == null)
        {
            if(CheckIfRequirementsFulfilled() == true)
            {
                PathRequestHandler.RequestPath(worker, tile);
                return;
            }

            CancelTask(true, true);
            return;
        }

        worker.SetActiveTask(task, true);
    }
    bool CheckIfRequirementsFulfilled()
    {
        return storedRequirements.Keys.Count == requirements.Keys.Count && storedRequirements.Keys.All(k => requirements.ContainsKey(k) && object.Equals(requirements[k], storedRequirements[k]));
    }
    public override void CancelTask(bool isCancelled, bool toIgnore = false)
    {
        isInitialised = false;
        tile.accessibility = Accessibility.ACCESSIBLE;

        if(isCancelled == false)
        {
            if (storedRequirements.Count > 0)
            {
                InventoryManager.AddToTileInventory(tile, storedRequirements);
                storedRequirements.Clear();
            }

            if (isFloor == true)
            {
                tile.SetFloorType(floorType);
            }
            else if (tile.installedObject != null)
            {
                tile.UninstallObject();
            }
        }

        base.CancelTask(isCancelled, toIgnore);
    }
}
