using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSpriteController : MonoBehaviour
{
    // Start is called before the first frame update
    public void Init()
    {
        TaskManager.AddTaskCallback(OnTaskCreated);
    }
    void OnTaskCreated(Task task)
    {
        if(task == null)
        {
            return;
        }

        task.AddTaskCompleteCallback(OnTaskEnded);
        task.AddTaskCancelledCallback(OnTaskEnded);
    }

    void OnTaskEnded(Task task)
    {
        
    }
}
