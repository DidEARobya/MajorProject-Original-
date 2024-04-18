using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum TaskType
{
    CONSTRUCTION,
}
public class Task
{
    public Tile tile;
    protected float taskTime = 1;

    public TaskType taskType;
    public CharacterController worker;

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
        taskTime -= workTime;

        if(taskTime <= 0)
        {
            if(taskCompleteCallback != null)
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

        if(taskCancelledCallback != null)
        {
            taskCancelledCallback(this);
        }
    }
}
