using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum TaskType
{
    CONSTRUCTION,
    MINING,
    HAULING
}
public static class TaskManager
{
    static Dictionary<TaskType, List<Task>> taskLists = new Dictionary<TaskType, List<Task>>();

    static Action<Task> taskCreatedCallback;

    public static void Init()
    {
        taskLists.Add(TaskType.CONSTRUCTION, new List<Task>());
        taskLists.Add(TaskType.MINING, new List<Task>());
        taskLists.Add(TaskType.HAULING, new List<Task>());
    }
    public static void AddTask(Task task, TaskType type)
    {
        if(task == null)
        {
            return;
        }

        if (taskLists[type].Contains(task) == true)
        {
            return;
        }

        taskLists[type].Add(task);

        if(taskCreatedCallback != null)
        {
            taskCreatedCallback(task);
        }
    }
    public static void RemoveTask(Task task, TaskType type)
    {
        if (task == null)
        {
            return;
        }

        if (taskLists[type].Contains(task) == true)
        {
            taskLists[type].Remove(task);
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
    public static Task GetTask(TaskType type, CharacterController character)
    {
        if (taskLists[type].Count == 0)
        {
            return null;
        }

        Task task = GetClosestValidTask(taskLists[type], character.currentTile, character);

        if (task == null || task.path == null)
        {
            return null;
        }

        taskLists[type].Remove(task);

        return task;
    }

    static Task GetClosestValidTask(List<Task> list, Tile start, CharacterController character)
    {
        bool refreshPathGraph = false;
        float lowestDist = Mathf.Infinity;
        Task closestTask = null;
        Path_AStar path = null;

        for (int i = 0; i < list.Count; i++)
        {
            if (character.ignoredTasks.Contains(list[i]))
            {
                continue;
            }

            Tile goal = list[i].tile;

            int distX = Mathf.Abs(start.x - goal.x);
            int distY = Mathf.Abs(start.y - goal.y);

            if (lowestDist > (distX + distY))
            {
                path = Utility.CheckIfTaskValid(start, goal, refreshPathGraph);

                if (path != null)
                {
                    lowestDist = distX + distY;
                    closestTask = list[i];
                }
                else
                {
                    character.ignoredTasks.Add(list[i]);
                }
            }
        }

        if(closestTask == null || path == null)
        {
            return null;
        }    

        closestTask.path = path;

        return closestTask;
    }
    public static int GetQueueSize(TaskType type)
    {
        return taskLists[type].Count;
    }

    public static void CreateHaulToStorageTask(CharacterController character, ItemTypes type, Tile toStoreAt, int amount = 0)
    {
        Tile tile = InventoryManager.GetClosestValidItem(character.currentTile, type);

        Task task = new HaulTask(tile, (t) => { InventoryManager.DropInventory(character.inventory, toStoreAt); }, toStoreAt, (t) => { InventoryManager.PickUp(character, tile, amount); }, TaskType.CONSTRUCTION);
        TaskManager.AddTask(task, TaskType.CONSTRUCTION);
    }
    public static HaulTask CreateHaulToJobSiteTask(RequirementTask jobSite, CharacterController character, ItemTypes type, Tile toStoreAt, int amount = 0)
    {
        Tile tile = InventoryManager.GetClosestValidItem(character.currentTile, type);

        HaulTask task = new HaulTask(tile, (t) => { jobSite.StoreComponent(character.inventory, amount); }, toStoreAt, (t) => { InventoryManager.PickUp(character, tile, amount); }, TaskType.CONSTRUCTION);
        task.path = new Path_AStar(character.currentTile, tile, true, false);

        if(task.path == null)
        {
            Debug.Log("No Path");
        }

        return task;
    }
}
public struct TaskPair
{
    public Task task;
    public Path_AStar path;

    public TaskPair(Task _task,  Path_AStar _path)
    {
        task = _task;
        path = _path;
    }
}
