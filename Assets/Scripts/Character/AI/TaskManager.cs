using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TaskManager
{
    Dictionary<TaskType, List<Task>> taskLists = new Dictionary<TaskType, List<Task>>();

    Action<Task> taskCreatedCallback;

    public void Init()
    {
        taskLists.Add(TaskType.CONSTRUCTION, new List<Task>());
        taskLists.Add(TaskType.MINING, new List<Task>());
        taskLists.Add(TaskType.AGRICULTURE, new List<Task>());
    }
    public void AddTask(Task task, TaskType type)
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
    public void RemoveTask(Task task, TaskType type)
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
    public void AddTaskCallback(Action<Task> task)
    {
        taskCreatedCallback += task;
    }
    public void RemoveTaskCallback(Action<Task> task)
    {
        taskCreatedCallback -= task;
    }
    public Task GetTask(TaskType type, CharacterController character)
    {
        if (taskLists[type].Count == 0)
        {
            return null;
        }

        bool taskAvailable = false;

        foreach (Task t in taskLists[type])
        {
            if(character.ignoredTasks.Contains(t) == false)
            {
                taskAvailable = true;
            }
        }

        if(taskAvailable == false)
        {
            return null;    
        }

        Task task = GetClosestValidTask(character.currentTile, type);

        if (task == null)
        {
            foreach(Task t in taskLists[type])
            {
                character.ignoredTasks.Add(t);
            }

            return null;
        }

        taskLists[type].Remove(task);

        return task;
    }

    Task GetClosestValidTask(Tile start, TaskType type)
    {
        BFS_Search search = new BFS_Search();
        Region toCheck = search.GetClosestRegionWithTask(GameManager.GetRegionManager().GetRegionAtTile(start), true, type);

        search = null;

        if (toCheck == null)
        {
            return null;
        }

        float lowestDist = Mathf.Infinity;
        Tile closest = null;

        foreach (Tile tile in toCheck.searchTiles)
        {
            if (tile.task == null || tile.task.taskType != type || tile.task.worker != null)
            {
                continue;
            }

            int distX = Mathf.Abs(start.x - tile.x);
            int distY = Mathf.Abs(start.y - tile.y);

            if (lowestDist > (distX + distY))
            {
                closest = tile;
                lowestDist = distX + distY;
            }
        }

        if (closest == null)
        {
            return null;
        }

        return closest.task;
    }
    public int GetQueueSize(TaskType type)
    {
        return taskLists[type].Count;
    }

    public Task CreateHaulToStorageTask(CharacterController character)
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
        toStoreAt.inventory.toBeStored += amount;

        Task task = new HaulTask(tile, (t) => { InventoryManager.DropInventory(character.inventory, toStoreAt); }, toStoreAt, (t) => { InventoryManager.PickUp(character, tile, amount); }, TaskType.HAULING);

        return task;
    }
    public HaulTask CreateHaulToJobSiteTask(RequirementTask jobSite, CharacterController character, ItemData type, Tile toStoreAt, int amount = 0)
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
