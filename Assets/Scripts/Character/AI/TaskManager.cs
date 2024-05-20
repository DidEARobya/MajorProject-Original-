using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum TaskType
{
    CONSTRUCTION,
    MINING,
    HAULING,
    AGRICULTURE
}
public static class TaskManager
{
    static Dictionary<TaskType, List<Task>> taskLists = new Dictionary<TaskType, List<Task>>();

    static Action<Task> taskCreatedCallback;

    public static void Init()
    {
        taskLists.Add(TaskType.CONSTRUCTION, new List<Task>());
        taskLists.Add(TaskType.MINING, new List<Task>());
        taskLists.Add(TaskType.AGRICULTURE, new List<Task>());
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
        if (task == null || taskLists.ContainsKey(type) == false)
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

        if (task == null)
        {
            return null;
        }

        taskLists[type].Remove(task);

        return task;
    }

    static Task GetClosestValidTask(List<Task> list, Tile start, CharacterController character)
    {
        float lowestDist = Mathf.Infinity;
        Stack<Task> taskStack = new Stack<Task>();

        for (int i = 0; i < list.Count; i++)
        {
            if (character.ignoredTasks.Contains(list[i]))
            {
                Debug.Log("Ignored");
                continue;
            }

            Tile goal = list[i].tile;

            int distX = Mathf.Abs(start.x - goal.x);
            int distY = Mathf.Abs(start.y - goal.y);

            if (lowestDist > (distX + distY))
            {
                taskStack.Push(list[i]);
                lowestDist = distX + distY;
            }
        }

        if(taskStack.Count == 0)
        {
            return null;
        }

        while(taskStack.Count > 0)
        {
            Task task = taskStack.Pop();

            Path_AStar path = new Path_AStar(start, task.tile, true);

            if (path == null || (path.Length() == 0 && start.IsNeighbour(task.tile) == false))
            {
                Debug.Log("No Path");
                character.ignoredTasks.Add(task);
                continue;
            }

            return task;
        }

        return null;
    }
    public static int GetQueueSize(TaskType type)
    {
        return taskLists[type].Count;
    }

    public static Task CreateHaulToStorageTask(CharacterController character)
    {
        if(StorageManager.storageTiles.Count == 0 || InventoryManager.inventories.Count == 0)
        {
            return null;
        }

        Tile tile = InventoryManager.GetClosestValidItem(character.currentTile, false);

        if(tile == null)
        {
            return null;
        }

        Tile toStoreAt = StorageManager.GetClosestAcceptableInventory(tile, tile.inventory.item, tile.inventory.stackSize);

        if(toStoreAt == null)
        {
            tile.inventory.isQueried = false;
            return null;
        }

        int amount = toStoreAt.inventory.CanBeStored(tile.inventory.item, tile.inventory.stackSize);
        Task task = new HaulTask(tile, (t) => { InventoryManager.DropInventory(character.inventory, toStoreAt); }, toStoreAt, (t) => { InventoryManager.PickUp(character, tile, amount); }, TaskType.HAULING);

        return task;
    }
    public static HaulTask CreateHaulToJobSiteTask(RequirementTask jobSite, CharacterController character, ItemTypes type, Tile toStoreAt, int amount = 0)
    {
        if (InventoryManager.inventories.Count == 0)
        {
            return null;
        }

        Tile tile = InventoryManager.GetClosestValidItem(character.currentTile, type, amount);

        if (tile == null)
        {
            return null;
        }

        HaulTask task = new HaulTask(tile, (t) => { jobSite.StoreComponent(character.inventory); }, toStoreAt, (t) => { InventoryManager.PickUp(character, tile, amount); }, TaskType.HAULING);

        return task;
    }
}
