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
    protected float taskTime = 1;

    public TaskType taskType;
    public CharacterController worker;

    protected Action<Task> taskCompleteCallback;

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
    public virtual void InitTask(CharacterController character)
    {
        worker = character;
    }
    public void AddTaskCompleteCallback(Action<Task> _taskCompleteCallback)
    {
        taskCompleteCallback += _taskCompleteCallback;
    }
    public void RemoveTaskCompleteCallback(Action<Task> _taskCompleteCallback)
    {
        taskCompleteCallback -= _taskCompleteCallback;
    }

    public virtual Task CheckTaskRequirements()
    {
        return null;
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
                tile.task = null;
            }
        }
    }
    public virtual void CancelTask(bool isCancelled, bool toIgnore = false)
    {
        if (worker != null)
        {
            if(isCancelled == true && toIgnore == true)
            {
                worker.ignoredTasks.Add(this);
            }

            worker.CancelTask(this);
            worker = null;
        }

        if (isCancelled == true)
        {
            GameManager.GetTaskManager().AddTask(this, taskType);
            return;
        }

        GameManager.GetTaskManager().RemoveTask(this, taskType);

        tile.isPendingTask = false;
        tile.task = null;
    }
}
