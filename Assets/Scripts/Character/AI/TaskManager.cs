using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TaskManager
{
    static Queue<Task> constructionTasks = new Queue<Task>();
    static Action<Task> taskCreatedCallback;

    public static void AddTask(Task task, TaskType type)
    {
        if(task == null)
        {
            Debug.Log("Tried to add a null task");
            return;
        }

        switch(type)
        {
            case TaskType.CONSTRUCTION:

                if (constructionTasks.Contains(task) == true)
                {
                    Debug.Log("Task already exists");
                    return;
                }
                constructionTasks.Enqueue(task);
                break;

        }

        if(taskCreatedCallback != null)
        {
            taskCreatedCallback(task);
        }
    }

    public static void AddTaskCallback(Action<Task> task)
    {
        taskCreatedCallback += task;
    }
    public static void RemoveTaskCallback(Action<Task> task)
    {
        taskCreatedCallback -= task;
    }
    public static Task GetTask(TaskType type)
    {

        switch (type)
        {
            case TaskType.CONSTRUCTION:

                if (constructionTasks.Count == 0)
                {
                    return null;
                }

                return constructionTasks.Dequeue();

        }

        return null;
    }

    public static int GetQueueSize(TaskType type)
    {
        switch (type)
        {
            case TaskType.CONSTRUCTION:
            return constructionTasks.Count;
        }

        return 0;
    }
}
