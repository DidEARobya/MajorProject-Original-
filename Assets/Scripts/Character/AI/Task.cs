using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum TaskType
{
    CONSTRUCTION,
}
public class Task
{
    public Tile tile;
    public Path_AStar path;
    protected float taskTime = 1;

    public TaskType taskType;
    public CharacterController worker;

    public Dictionary<ItemTypes, int> requirements;
    public Dictionary<ItemTypes, int> storedRequirements;

    protected Action<Task> taskCompleteCallback;
    protected Action<Task> taskCancelledCallback;

    public Task (Tile _tile, Action<Task> _taskCompleteCallback, TaskType type, float _taskTime = 1)
    {
        tile = _tile;
        taskTime = _taskTime;
        taskType = type;

        if(tile != null)
        {
            tile.isPendingTask = true;
        }

        taskCompleteCallback += _taskCompleteCallback;
    }
    public Task(Tile _tile, Action<Task> _taskCompleteCallback, TaskType type, ItemTypes requiredType, int requiredAmount, float _taskTime = 1)
    {
        tile = _tile;
        taskTime = _taskTime;
        taskType = type;

        requirements = new Dictionary<ItemTypes, int>();
        requirements.Add(requiredType, requiredAmount);

        storedRequirements = new Dictionary<ItemTypes, int>();

        if (tile != null)
        {
            tile.isPendingTask = true;
        }

        taskCompleteCallback += _taskCompleteCallback;
    }
    public void AddTaskCompleteCallback(Action<Task> _taskCompleteCallback)
    {
        taskCompleteCallback += _taskCompleteCallback;
    }

    public void AddTaskCancelledCallback(Action<Task> _taskCancelledCallback)
    {
        taskCancelledCallback += _taskCancelledCallback;
    }

    public void DoWork(float workTime)
    {
        if(requirements != null && CheckIfRequirementsFulfilled() == false)
        {
            if(CheckIfWorkable() == false)
            {
                return;
            }
        }

        taskTime -= workTime;

        if(taskTime <= 0)
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

            if (storedRequirements.Count != 0 && storedRequirements.Count >= i)
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
    public void CancelTask(bool isCancelled)
    {
        if(isCancelled == true)
        {
            TaskManager.AddTask(this, taskType);
        }

        tile.isPendingTask = false;
        worker = null;

        if(taskCancelledCallback != null)
        {
            taskCancelledCallback(this);
        }
    }

    bool CheckIfRequirementsFulfilled()
    {
        return storedRequirements.Keys.Count == requirements.Keys.Count && storedRequirements.Keys.All(k => requirements.ContainsKey(k) && object.Equals(requirements[k], storedRequirements[k]));
    }
}
