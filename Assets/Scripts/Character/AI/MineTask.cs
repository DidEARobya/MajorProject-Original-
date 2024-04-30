using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineTask : Task
{
    public MineTask(Tile _tile, Action<Task> _taskCompleteCallback, TaskType type, bool _isFloor, float targetDurability) : base(_tile, _taskCompleteCallback, type, _isFloor, targetDurability)
    {
    }
    public override void DoWork(float workTime)
    {
        base.DoWork(workTime);
    }
    public override void CancelTask(bool isCancelled)
    {
        base.CancelTask(isCancelled);
    }
}

