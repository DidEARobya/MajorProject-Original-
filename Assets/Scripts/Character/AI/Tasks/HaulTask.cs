using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaulTask : Task
{
    Tile storageTile;

    Action<Task> gatherCompleteCallback;

    Path_AStar storagePath;

    RequirementTask parentTask;
    bool isGathering = true;
    public HaulTask(Tile _tile, Action<Task> _taskCompleteCallback, Tile _storageTile, Action<Task> _gatherCompleteCallback, TaskType _type, bool _isFloor = false, float _taskTime = 1, RequirementTask _task = null) : base(_tile, _taskCompleteCallback, _type, _isFloor, _taskTime)
    {
        storageTile = _storageTile;
        gatherCompleteCallback = _gatherCompleteCallback;
    }
    public override void InitTask(CharacterController character)
    {
        base.InitTask(character);
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
                    isGathering = false;
                    taskTime = 1;
                    worker.pathFinder = storagePath;

                    storagePath = new Path_AStar(worker.currentTile, storageTile, true);

                    if (storagePath == null)
                    {
                        Debug.Log("Cannot Haul");
                        worker.ignoredTasks.Add(this);
                        worker.CancelTask(false, this);

                        return;
                    }

                    worker.destinationTile = storageTile;
                    worker.pathFinder = storagePath;
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
            }
        }
    }
    public override void CancelTask(bool isCancelled)
    {
        if (isCancelled == false)
        {
            storageTile.isPendingTask = false;
            storageTile.task = null;
        }

        base.CancelTask(isCancelled);
    }
}
