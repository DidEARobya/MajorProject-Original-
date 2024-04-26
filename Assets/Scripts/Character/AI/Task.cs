using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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
        if(storedRequirements != requirements)
        {
            ItemTypes type = null;

            if (requirements.ContainsKey(worker.inventory.item))
            {
                storedRequirements.Add(worker.inventory.item, worker.inventory.stackSize);
            }

            if (storedRequirements[worker.inventory.item] < requirements[worker.inventory.item])
            {
                type = worker.inventory.item;
            }
            else
            {
                for (int i = 0; i < requirements.Count; i++)
                {
                    if (storedRequirements.ElementAt(i).Value != requirements.ElementAt(i).Value)
                    {
                        type = requirements.ElementAt(i).Key;
                        break;
                    }
                }
            }

            if (type != null)
            {
                Path_AStar path = InventoryManager.GetClosestValidItem(tile, type);
                Task task = new Task(tile, (t) => { InventoryManager.PickUp(worker, tile); }, TaskType.CONSTRUCTION);

                Task currentTask = worker.activeTask;
                worker.taskQueue.Enqueue(currentTask);
                worker.activeTask = task;

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
}
