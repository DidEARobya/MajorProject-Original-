using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ConstructionSite : TaskSite
{
    private ConstructionData _data;

    public ConstructionSite(Tile tile, ConstructionData data, Action constructionCompleteCallback)
    {
        siteTiles = new List<Tile>();
        siteTiles.Add(tile);

        foreach (Tile t in siteTiles)
        {
            t.site = this;
        }

        canHaveMultipleWorkers = false;

        _data = data;
        siteCompleteCallback += constructionCompleteCallback;

        activeTasks = new List<Task>();

        GameManager.GetTaskManager().AddTaskSite(this, TaskType.CONSTRUCTION);
    }
    public ConstructionSite(List<Tile> tiles, ConstructionData data, Action constructionCompleteCallback)
    { 
        siteTiles = tiles;

        foreach (Tile tile in siteTiles)
        {
            tile.site = this;
        }

        canHaveMultipleWorkers = false;

        _data = data;
        siteCompleteCallback += constructionCompleteCallback;

        activeTasks = new List<Task>();

        GameManager.GetTaskManager().AddTaskSite(this, TaskType.CONSTRUCTION);
    }
    protected override void CompleteTaskSite()
    {
        foreach (Task task in activeTasks)
        {
            task.CancelTask(false);
        }

        foreach (Tile tile in siteTiles)
        {
            tile.site = null;
        }

        GameManager.GetTaskManager().RemoveTaskSite(this, TaskType.CONSTRUCTION);

        base.CompleteTaskSite();
    }
    public override void CancelTaskSite()
    {
        foreach(Task task in activeTasks)
        {
            task.CancelTask(false);
        }

        foreach (Tile tile in siteTiles)
        {
            tile.site = null;
        }

        if(_data.constructionType == ConstructionType.BUILDING)
        {
            siteTiles[0].UninstallObject();
        }
        else
        {
            
        }
    }
    public override Task GetTask(CharacterController worker)
    {
        if(IsWorkable() == false)
        {
            Debug.Log("Not workable");
            return null;
        }

        foreach(Tile tile in siteTiles)
        {
            if (tile.inventory.item != null)
            {
                Tile toStoreAt = tile.GetNearestAvailableInventory(tile.inventory.item, tile.inventory.stackSize, false);
                Task haulTask = new HaulTask(tile, (t) => { InventoryManager.DropInventory(worker.inventory, toStoreAt); }, toStoreAt, (t) => { InventoryManager.PickUp(worker, tile); }, TaskType.HAULING);
                return haulTask;
            }
        }

        Task task = new ConstructionTask(siteTiles[0], (t) => { activeTasks.Remove(t); CompleteTaskSite(); }, TaskType.CONSTRUCTION, _data.constructionTime);
        task.AddTaskCancelledCallback((t) => { activeTasks.Remove(t); if (siteWorker == worker) { siteWorker = null; } else { Debug.Log("TRIED REMOVING INVALID WORKER FROM SITE"); } });
        activeTasks.Add(task);
        siteWorker = worker;
        return task;
    }

    public override TaskType GetSiteType()
    {
        return TaskType.CONSTRUCTION;
    }
}
