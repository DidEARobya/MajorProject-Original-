using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RequirementTask : Task
{
    public Dictionary<ItemTypes, int> requirements;
    public Dictionary<ItemTypes, int> storedRequirements;

    public RequirementTask(Tile _tile, Action<Task> _taskCompleteCallback, TaskType type, Dictionary<ItemTypes, int> _requirements, float _taskTime = 1) : base(_tile, _taskCompleteCallback, type, _taskTime)
    {
        tile = _tile;
        taskTime = _taskTime;
        taskType = type;

        requirements = _requirements;

        storedRequirements = new Dictionary<ItemTypes, int>();

        if (tile != null)
        {
            tile.isPendingTask = true;
        }

        taskCompleteCallback += _taskCompleteCallback;
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

        taskTime -= workTime;

        if (taskTime <= 0)
        {
            tile.isPendingTask = false;

            if (taskCompleteCallback != null)
            {
                taskCompleteCallback(this);
            }
        }
    }
    bool CheckIfWorkable()
    {
        if (worker.inventory.item != null && requirements.ContainsKey(worker.inventory.item))
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
            TilePathPair pair = InventoryManager.GetClosestValidItem(worker.currentTile, type);

            if (pair.path == null)
            {
                Debug.Log("No Item Available");
                worker.ignoredTasks.Add(this);
                worker.CancelTask();
                return;
            }

            Path_AStar path = pair.path;

            Task task = new Task(pair.tile, (t) => { InventoryManager.PickUp(worker, pair.tile); }, TaskType.CONSTRUCTION);
            task.path = path;

            Task currentTask = worker.activeTask;
            worker.taskStack.Push(currentTask);

            worker.UpdateTask(task);
        }
    }
    bool CheckIfRequirementsFulfilled()
    {
        return storedRequirements.Keys.Count == requirements.Keys.Count && storedRequirements.Keys.All(k => requirements.ContainsKey(k) && object.Equals(requirements[k], storedRequirements[k]));
    }
}