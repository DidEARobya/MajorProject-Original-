using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TaskManager
{
    Dictionary<TaskType, List<ConstructionSite>> taskLists = new Dictionary<TaskType, List<ConstructionSite>>();

    Action<Task> taskCreatedCallback;

    public void Init()
    {
        taskLists.Add(TaskType.CONSTRUCTION, new List<ConstructionSite>());
        //taskLists.Add(TaskType.MINING, new List<Task>());
        //taskLists.Add(TaskType.AGRICULTURE, new List<Task>());
    }
    public void AddTaskSite(ConstructionSite site, TaskType type)
    {
        if(site == null)
        {
            return;
        }

        if (taskLists[type].Contains(site) == true)
        {
            return;
        }

        taskLists[type].Add(site);

        if(taskCreatedCallback != null)
        {
            //taskCreatedCallback(site);
        }
    }
    public void RemoveTaskSite(ConstructionSite site, TaskType type)
    {
        if (site == null || taskLists.ContainsKey(type) == false)
        {
            return;
        }

        if (taskLists[type].Contains(site) == true)
        {
            taskLists[type].Remove(site);
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
        if (taskLists.ContainsKey(type) == false || taskLists[type].Count == 0)
        {
            return null;
        }

        bool taskAvailable = false;

        foreach (ConstructionSite s in taskLists[type])
        {
            if(character.ignoredTaskSites.Contains(s) == false)
            {
                taskAvailable = true;
            }
        }

        if(taskAvailable == false)
        {
            return null;    
        }

        ConstructionSite site = GetClosestValidSite(character.currentTile, character, type);

        if (site == null)
        {
            foreach(ConstructionSite t in taskLists[type])
            {
                character.ignoredTaskSites.Add(t);
            }

            return null;
        }

        Task task = site.GetTask(character);

        if (task == null)
        {
            character.ignoredTaskSites.Add(site);
            return null;
        }

        return task;
    }
    ConstructionSite GetClosestValidSite(Tile start, CharacterController character, TaskType type)
    {
        Tile closest = null;

        BFS_Search search = new BFS_Search();
        Region toCheck = search.GetClosestRegionWithTask(out closest, GameManager.GetRegionManager().GetRegionAtTile(start), character, true, type);

        search = null;

        if (toCheck == null)
        {
            Debug.Log("toCheck is null");
            return null;
        }

        if(closest == null)
        {
            Debug.Log("closest is null");
            return null;
        }

        return closest.site;
    }
    /*Task GetClosestValidTask(Tile start, TaskType type)
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
    }*/
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
    public HaulTask CreateHaulToJobSiteTask(ConstructionSite jobSite, CharacterController character, ItemData type, Tile toStoreAt, int amount = 0)
    {
        if (InventoryManager.inventories.Count == 0)
        {
            return null;
        }

        Tile tile = InventoryManager.GetClosestValidItem(character.currentTile, type, amount);

        if (tile == null)
        {
            Debug.Log("NO TILE");
            return null;
        }

        HaulTask task = new HaulTask(tile, (task) => { jobSite.StoreMaterials(task, character, character.inventory); }, toStoreAt, (task) => { InventoryManager.PickUp(character, tile, amount); }, TaskType.HAULING);

        return task;
    }
}
