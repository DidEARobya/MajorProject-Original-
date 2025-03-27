using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Jobs;
using UnityEngine;

public class TaskRequestHandler
{
    static Queue<CharacterController> requests = new Queue<CharacterController>();

    static object requestCompleteLock = new object();

    static bool isHandlingRequest = false;

    public static void RequestTask(CharacterController request)
    {
        if (requests.Contains(request))
        {
            return;
        }

        request.requestedTask = true;
        requests.Enqueue(request);
        return;
    }
    public static void Update()
    {
        if (requests.Count > 0 && isHandlingRequest == false)
        {
            ThreadedCompleteRequest();
        }
    }
    static void ThreadedCompleteRequest()
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

            Task task = request.priorityDict.GetTask();

            if (task != null)
            {
                task.worker = request;
                request.taskList.Add(task);
            }

            request.requestedTask = false;
            isHandlingRequest = false;
        }


    }
}