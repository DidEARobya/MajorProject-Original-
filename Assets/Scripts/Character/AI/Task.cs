using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Task
{
    public Tile tile;
    public Path_AStar path;
    protected float taskTime = 1;

    public TaskType taskType;
    public CharacterController worker;

    protected Action<Task> taskCompleteCallback;
    protected Action<Task> taskCancelledCallback;

    protected bool isFloor;

    public Task (Tile _tile, Action<Task> _taskCompleteCallback, TaskType type, bool _isFloor, float _taskTime = 1)
    {
        tile = _tile;
        tile.task = this;
        taskTime = _taskTime;
        taskType = type;
        isFloor = _isFloor;

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

    public virtual void DoWork(float workTime)
    {
        taskTime -= workTime * worker.workSpeed;

        if (taskTime <= 0)
        {
            tile.isPendingTask = false;

            if (taskCompleteCallback != null)
            {
                taskCompleteCallback(this);
            }
        }
    }
    public virtual void CancelTask(bool isCancelled)
    {
        if(isCancelled == true)
        {
            TaskManager.AddTask(this, taskType);
        }
        else
        {
            TaskManager.RemoveTask(this, taskType);
            tile.isPendingTask = false;
            tile.task = null;
        }

        if (worker != null)
        {
            worker.pathFinder = null;
            worker = null;
        }

        if (taskCancelledCallback != null)
        {
            taskCancelledCallback(this);
        }
    }
}
