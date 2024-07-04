using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaulTask : Task
{
    Tile storageTile;

    Action<Task> gatherCompleteCallback;

    bool isGathering = true;
    bool pathRequested = false;

    public HaulTask(Tile _tile, Action<Task> _taskCompleteCallback, Tile _storageTile, Action<Task> _gatherCompleteCallback, TaskType _type, bool _isFloor = false, float _taskTime = 1, RequirementTask _task = null) : base(_tile, _taskCompleteCallback, _type, _isFloor, _taskTime)
    {
        storageTile = _storageTile;
        gatherCompleteCallback = _gatherCompleteCallback;
    }
    public override void InitTask(CharacterController character)
    {
        base.InitTask(character);
        PathRequestHandler.RequestPath(worker, tile, true);
    }
    public override void DoWork(float workTime)
    {
        taskTime -= workTime * worker.workSpeed;

        if (taskTime <= 0)
        {
            if (isGathering == true)
            {
                tile.isPendingTask = false;

                if (gatherCompleteCallback != null)
                {
                    if (pathRequested == false)
                    {
                        PathRequestHandler.RequestPath(worker, storageTile, true);
                        pathRequested = true;
                    }

                    isGathering = false;
                    taskTime = 1;

                    gatherCompleteCallback(this);
                }
            }
            else
            {
                storageTile.isPendingTask = false;

                if (taskCompleteCallback != null)
                {
                    taskCompleteCallback(this);
                }

                storageTile.task = null;
            }
        }
    }
    public override void CancelTask(bool isCancelled, bool toIgnore = false)
    {
        if (isCancelled == false)
        {
            storageTile.isPendingTask = false;
            storageTile.task = null;
        }

        base.CancelTask(isCancelled, toIgnore);
    }
}
