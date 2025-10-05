using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionSite : TaskSite
{
    private TaskType _taskType;
    private bool _isFloor;

    public DestructionSite(Tile tile, TaskType taskType, System.Action destructionCompleteCallback, bool isFloor = false)
    {
        siteTiles = new List<Tile>();
        siteTiles.Add(tile);

        foreach (Tile t in siteTiles)
        {
            t.site = this;
        }

        canHaveMultipleWorkers = false;
        _isFloor = isFloor;

        siteCompleteCallback += destructionCompleteCallback;

        activeTasks = new List<Task>();
        _taskType = taskType;
        GameManager.GetTaskManager().AddTaskSite(this, _taskType);
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

        GameManager.GetTaskManager().RemoveTaskSite(this, _taskType);

        base.CompleteTaskSite();
    }

    public override void CancelTaskSite() 
    {
        foreach (Task task in activeTasks)
        {
            task.CancelTask(false);
        }

        foreach (Tile tile in siteTiles)
        {
            tile.site = null;
        }
    }
    public override Task GetTask(CharacterController worker)
    {
        if (IsWorkable() == false)
        {
            Debug.Log("Not workable");
            return null;
        }

        float taskTime = _isFloor ? ThingsDataHandler.GetFloorData(siteTiles[0].floorType).constructionTime : siteTiles[0].installedObject.durability;

        Task task = new DestructionTask(siteTiles[0], (t) => { activeTasks.Remove(t); CompleteTaskSite(); }, _taskType, taskTime);
        task.AddTaskCancelledCallback((t) => { activeTasks.Remove(t); if (siteWorker == worker) { siteWorker = null; } else { Debug.Log("TRIED REMOVING INVALID WORKER FROM SITE"); } });
        activeTasks.Add(task);
        siteWorker = worker;
        return task;
    }

    public override TaskType GetSiteType()
    {
        return _taskType;
    }
}
