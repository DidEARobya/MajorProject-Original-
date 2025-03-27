using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HaulSite : TaskSite
{
    public Dictionary<ItemData, int> _requirements;
    private Dictionary<ItemData, int> _storedRequirements;

    public HaulSite(List<Tile> tiles, Dictionary<ItemData, int> requirements, Action haulCompleteCallback)
    {
        siteTiles = tiles;

        foreach (Tile tile in siteTiles)
        {
            tile.site = this;
        }

        canHaveMultipleWorkers = true;

        siteCompleteCallback += haulCompleteCallback;

        _requirements = requirements;
        _storedRequirements = new Dictionary<ItemData, int>();
        activeTasks = new List<Task>();

        GameManager.GetTaskManager().AddTaskSite(this, TaskType.HAULING);
    }
    protected override void CompleteTaskSite()
    {
        foreach (Task t in activeTasks)
        {
            t.CancelTask(false);
        }

        foreach (Tile tile in siteTiles)
        {
            tile.site = null;
        }

        GameManager.GetTaskManager().RemoveTaskSite(this, TaskType.HAULING);

        base.CompleteTaskSite();
    }
    public void CancelTaskSite()
    {
        foreach (Task task in activeTasks)
        {
            task.CancelTask(false);
        }

        foreach (Tile tile in siteTiles)
        {
            tile.site = null;
        }

        siteTiles[0].UninstallObject();
    }
    public override bool IsWorkable()
    {
        if (base.IsWorkable() == false || IsRequirementsFulfilled() == true)
        {
            return false;
        }

        return true;
    }
    public override Task GetTask(CharacterController worker)
    {
        if (IsWorkable() == false)
        {
            Debug.Log("Not workable");
            return null;
        }

        Task task = CreateHaulTask(worker);
        return task;
    }
    Task CreateHaulTask(CharacterController worker)
    {
        HaulTask task = null;

        foreach (ItemData item in _requirements.Keys)
        {
            if (_storedRequirements.ContainsKey(item) == false)
            {
                _storedRequirements.Add(item, 0);
            }

            if (_storedRequirements[item] == _requirements[item])
            {
                continue;
            }

            int remaining = Mathf.Abs(_storedRequirements[item] - _requirements[item]);

            foreach (Tile tile in siteTiles)
            {
                task = GameManager.GetTaskManager().CreateHaulToHaulSiteTask(this, worker, item, tile, remaining);

                if (task != null)
                {
                    int adjust = task.tile.inventory.stackSize;

                    if (adjust > remaining)
                    {
                        adjust = remaining;
                    }

                    task.BindTaskCancelledCallback(() => { Debug.Log(_storedRequirements[item]); _storedRequirements[item] -= adjust; Debug.Log(adjust + " : " + _storedRequirements[item]); });
                    _storedRequirements[item] += adjust;

                    activeTasks.Add(task);
                    return task;
                }
            }
        }

        return null;
    }
    public void StoreMaterials(Task task, CharacterController worker, Inventory inventory)
    {
        InventoryManager.ClearInventory(inventory);

        if (IsRequirementsFulfilled() == true && activeTasks.Count == 0)
        {
            CompleteTaskSite();
        }
    }
    public bool IsRequirementsFulfilled()
    {
        if(_storedRequirements.Count != _requirements.Count)
        {
            return false;
        }

        foreach(ItemData item  in _storedRequirements.Keys)
        {
            if (_storedRequirements[item] != _requirements[item])
            {
                return false;
            }
        }

        return true;
    }
}
