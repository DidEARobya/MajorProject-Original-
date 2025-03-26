using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConstructionTask : Task
{
    Action _taskCancelledCallback;

    public ConstructionTask(Tile _tile, Action<Task> _taskCompleteCallback, TaskType _type, bool _isFloor = false, float _taskTime = 1) : base(_tile, _taskCompleteCallback, _type, _isFloor, _taskTime)
    {
    }
    public void BindTaskCancelledCallback(Action taskCancelled)
    {
        _taskCancelledCallback += taskCancelled;
    }
    public override void InitTask(CharacterController character)
    {
        base.InitTask(character);

        if (character.inventory.item != null)
        {
            InventoryManager.DropInventory(character.inventory, character.currentTile);
        }

        PathRequestHandler.RequestPath(worker, tile, true);
    }
    public override void CancelTask(bool isCancelled, bool toIgnore = false)
    {
        Debug.Log("CONSTRUCTION CANCELLED");
        if (_taskCancelledCallback != null)
        {
            _taskCancelledCallback();
        }

        if (isCancelled == false)
        {
            if (tile.installedObject != null)
            {
                tile.UninstallObject();
            }
        }

        base.CancelTask(isCancelled, toIgnore);
    }
}
