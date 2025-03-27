using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSite
{
    protected List<Tile> siteTiles;
    protected List<Task> activeTasks;

    protected CharacterController siteWorker;
    protected bool canHaveMultipleWorkers;

    protected Action siteCompleteCallback;

    public void RemoveTask(Task task)
    {
        if (activeTasks.Contains(task) == false)
        {
            Debug.Log("TRYING TO REMOVE INVALID TASK FROM SITE");
            return;
        }

        activeTasks.Remove(task);
    }
    public virtual Task GetTask(CharacterController worker)
    {
        return null;
    }
    public virtual bool IsWorkable()
    {
        if(canHaveMultipleWorkers == true || siteWorker == null)
        {
            return true;
        }

        return false;
    }
    protected virtual void CompleteTaskSite()
    {
        if(siteCompleteCallback != null)
        {
            siteCompleteCallback();
        }
    }
    public virtual TaskType GetSiteType()
    {
        return TaskType.HAULING;
    }
}
