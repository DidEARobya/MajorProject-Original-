using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TendTask : Task
{
    public TendTask(Tile _tile, Action<Task> _taskCompleteCallback, TaskType type, bool _isFloor, float tendTime) : base(_tile, _taskCompleteCallback, type, _isFloor, tendTime)
    {

    }
    public override void InitTask(CharacterController character)
    {
        base.InitTask(character);
        PathRequestHandler.RequestPath(worker, tile, true);
    }
    public override void DoWork(float workTime)
    {
        base.DoWork(workTime);
    }
    public override void CancelTask(bool isCancelled, bool toIgnore = false)
    {
        base.CancelTask(isCancelled, toIgnore);
    }
}
