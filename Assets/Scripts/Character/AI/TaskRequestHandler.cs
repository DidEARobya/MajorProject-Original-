using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TaskRequestHandler
{
    Queue<CharacterController> requests = new Queue<CharacterController>();

    static object requestCompleteLock = new object();

    bool isHandlingRequest = false;

    public void RequestTask(CharacterController request)
    {
        if (requests.Contains(request))
        {
            return;
        }

        request.requestedTask = true;
        requests.Enqueue(request);
        return;
    }
    public void Update()
    {
        if (requests.Count > 0 && isHandlingRequest == false)
        {
            ThreadPool.QueueUserWorkItem(delegate { ThreadedCompleteRequest(); });
        }
    }
    void ThreadedCompleteRequest()
    {
        lock (requestCompleteLock)
        {
            if (requests.Count == 0)
            {
                return;
            }

            isHandlingRequest = true;

            CharacterController request = requests.Dequeue();

            if (request == null)
            {
                isHandlingRequest = false;
                return;
            }

            Task task;

            foreach (TaskType type in request.priorityList)
            {
                if (type == TaskType.HAULING)
                {
                    task = GameManager.GetTaskManager().CreateHaulToStorageTask(request);
                }
                else
                {
                    task = GameManager.GetTaskManager().GetTask(type, request);
                }

                if (task != null)
                {
                    request.taskList.Add(task);
                    break;
                }
            }

            request.requestedTask = false;
            isHandlingRequest = false;
        }
    }
}