using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConstructionSite
{
    private List<Tile> _tiles;

    private BuildingData _data;

    private Dictionary<ItemData, int> _requirements;
    private Dictionary<ItemData, int> _expectedRequirements;
    private Dictionary<ItemData, int> _storedRequirements;

    private Action<Task> _constructionCompleteCallback;

    private List<Task> _activeTasks;
    public ConstructionSite(List<Tile> tiles, BuildingData data, Action<Task> constructionCompleteCallback)
    { 
        _tiles = tiles;

        foreach (Tile tile in _tiles)
        {
            tile.site = this;
        }

        _data = data;
        _constructionCompleteCallback = constructionCompleteCallback;
        _requirements = _data.GetRequirements();
        _expectedRequirements = new Dictionary<ItemData, int>();
        _storedRequirements = new Dictionary<ItemData, int>();
        _activeTasks = new List<Task>();
    }
    public void CancelConstruction()
    {
        foreach(Task task in _activeTasks)
        {
            task.CancelTask(false);
        }

        foreach (Tile tile in _tiles)
        {
            tile.site = null;
        }

        _tiles[0].UninstallObject();
    }
    public bool IsWorkable()
    {
        if (NeedsMaterials() == false)
        {
            Debug.Log("MAT");
            return false;
        }

        //Need building logic
        if (IsRequirementsFulfilled() == true)
        {
            Debug.Log("REQ");
            return false;
        }

        return true;
    }
    public bool NeedsMaterials()
    {
        return !(_expectedRequirements.Keys.Count == _requirements.Keys.Count && _expectedRequirements.Keys.All(k => _requirements.ContainsKey(k) && object.Equals(_requirements[k], _expectedRequirements[k])));
    }
    public Task GetTask(CharacterController worker)
    {
        if(IsWorkable() == false)
        {
            Debug.Log("Not workable");
            return null;
        }
        else
        {
            Debug.Log("workable");
        }

        Task task = CreateHaulTask(worker);

        return task;
    }
    Task CreateHaulTask(CharacterController worker)
    {
        HaulTask task = null;

        foreach (ItemData item in _requirements.Keys)
        {
            if (_expectedRequirements.ContainsKey(item) == false)
            {
                _expectedRequirements.Add(item, 0);
            }

            if (_expectedRequirements[item] != _requirements[item])
            {
                int remaining = Mathf.Abs(_expectedRequirements[item] - _requirements[item]);

                task = GameManager.GetTaskManager().CreateHaulToJobSiteTask(this, worker, item, _tiles[0], remaining);

                if (task != null)
                {
                    int adjust = remaining;

                    if (remaining > task.tile.inventory.stackSize)
                    {
                        adjust = task.tile.inventory.stackSize;
                    }

                    task.BindTaskCancelledCallback(() => { _expectedRequirements[item] -= adjust; });
                    _expectedRequirements[item] += adjust;
                    break;
                }
            }
        }

        if(task == null)
        {                
            return null;
        }

        _activeTasks.Add(task);
        return task;
    }
    public void StoreMaterials(Task task, CharacterController worker, Inventory inventory)
    {
        if (_activeTasks.Contains(task) == false)
        {
            Debug.LogError("Construction site recieved task complete from unverified task");
        }
        else
        {
            _activeTasks.Remove(task);
        }

        if (inventory.item == null)
        {
            return;
        }

        int amount = inventory.stackSize;

        if (_requirements.ContainsKey(inventory.item) == false)
        {
            InventoryManager.DropInventory(worker.inventory, worker.currentTile);
            return;
        }

        if (_storedRequirements.ContainsKey(inventory.item) == false)
        {
            _storedRequirements.Add(inventory.item, amount);
            InventoryManager.ClearInventory(inventory);

            if(NeedsMaterials() == false)
            {
                Debug.Log("COMPLETE");
                if(_constructionCompleteCallback != null)
                {
                    _constructionCompleteCallback(null);
                }

                foreach (Task t in _activeTasks)
                {
                    t.CancelTask(false);
                }

                foreach (Tile tile in _tiles)
                {
                    tile.site = null;
                }

                GameManager.GetTaskManager().RemoveTaskSite(this, TaskType.CONSTRUCTION);
            }

            return;
        }

        if (_storedRequirements[inventory.item] + amount > _requirements[inventory.item])
        {
            int excess = (_storedRequirements[inventory.item] + amount);

            inventory.stackSize -= amount;
            _storedRequirements[inventory.item] = _requirements[inventory.item];

            InventoryManager.AddToTileInventory(inventory.item, worker.currentTile, excess);
        }
        else
        {
            _storedRequirements[inventory.item] += amount;
            InventoryManager.ClearInventory(inventory);
        }

        if (NeedsMaterials() == false)
        {
            Debug.Log("COMPLETE");

            if (_constructionCompleteCallback != null)
            {
                _constructionCompleteCallback(null);
            }

            foreach (Task t in _activeTasks)
            {
                t.CancelTask(false);
            }

            foreach (Tile tile in _tiles)
            {
                tile.site = null;
            }

            GameManager.GetTaskManager().RemoveTaskSite(this, TaskType.CONSTRUCTION);
        }
    }
    bool IsRequirementsFulfilled()
    {
        return _storedRequirements.Keys.Count == _requirements.Keys.Count && _storedRequirements.Keys.All(k => _requirements.ContainsKey(k) && object.Equals(_requirements[k], _storedRequirements[k]));
    }
}
