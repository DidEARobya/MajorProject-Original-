using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionTask : Task
{
    Action _taskCancelledCallback;

    public DestructionTask(Tile _tile, Action<Task> _taskCompleteCallback, TaskType _type, float _taskTime = 1) : base(_tile, _taskCompleteCallback, _type,  _taskTime)
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
        Debug.Log("DESTRUCTION CANCELLED");
        if (_taskCancelledCallback != null)
        {
            _taskCancelledCallback();
        }

        base.CancelTask(isCancelled, toIgnore);
    }
}
