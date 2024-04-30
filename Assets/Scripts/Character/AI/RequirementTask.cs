using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class RequirementTask : Task
{
    public Dictionary<ItemTypes, int> requirements;
    public Dictionary<ItemTypes, int> storedRequirements;

    FloorTypes floorType;

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
    public override void DoWork(float workTime)
    {
        if (requirements != null && CheckIfRequirementsFulfilled() == false)
        {
            if (CheckIfWorkable() == false)
            {
                return;
            }
        }

        base.DoWork(workTime);
    }
    bool CheckIfWorkable()
    {
        if (worker.inventory.item != null)
        {
            if(requirements.ContainsKey(worker.inventory.item))
            {
                if (storedRequirements.ContainsKey(worker.inventory.item))
                {
                    storedRequirements[worker.inventory.item] += worker.inventory.stackSize;
                }
                else
                {
                    storedRequirements.Add(worker.inventory.item, worker.inventory.stackSize);
                }

                if (storedRequirements[worker.inventory.item] > requirements[worker.inventory.item])
                {
                    int diff = storedRequirements[worker.inventory.item] - requirements[worker.inventory.item];
                    storedRequirements[worker.inventory.item] -= diff;

                    InventoryManager.AddToTileInventory(worker.inventory.item, worker.currentTile, diff);
                }

                InventoryManager.ClearInventory(worker.inventory);
            }
            else
            {
                worker.DropInventory();
            }
        }

        if (CheckIfRequirementsFulfilled() == true)
        {
            return true;
        }

        QueueHaulTask();

        return false;
    }
    void QueueHaulTask()
    {
        ItemTypes type = null;
        int toTake = 0;

        for (int i = 0; i < requirements.Count; i++)
        {
            int stored = 0;

            if (storedRequirements.Count != 0 && storedRequirements.Count > i)
            {
                stored = storedRequirements.ElementAt(i).Value;
            }

            int required = requirements.ElementAt(i).Value;

            if (stored < required)
            {
                type = requirements.ElementAt(i).Key;
                toTake = required - stored;

                break;
            }
        }

        if (type != null)
        {
            TilePathPair pair = InventoryManager.GetClosestValidItem(worker.currentTile, type, toTake);
            Path_AStar path = pair.path;

            if (path == null)
            {
                Debug.Log("No Item Available");
                worker.ignoredTasks.Add(this);
                worker.CancelTask(true, this);
                return;
            }

            Task task = new Task(pair.tile, (t) => { InventoryManager.PickUp(worker, pair.tile, toTake); }, TaskType.CONSTRUCTION, false);
            task.path = path;

            worker.ForcePrioritiseTask(task);
        }
    }
    bool CheckIfRequirementsFulfilled()
    {
        return storedRequirements.Keys.Count == requirements.Keys.Count && storedRequirements.Keys.All(k => requirements.ContainsKey(k) && object.Equals(requirements[k], storedRequirements[k]));
    }
    public override void CancelTask(bool isCancelled)
    {
        base.CancelTask(isCancelled);

        if (storedRequirements.Count > 0)
        {
            InventoryManager.AddToTileInventory(tile, storedRequirements);
            storedRequirements.Clear();
        }

        if (isCancelled == true)
        {
            return;
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
}
