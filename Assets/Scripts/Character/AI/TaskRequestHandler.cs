using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class TaskRequestHandler
{
    static Queue<CharacterController> requests = new Queue<CharacterController>();
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
            CompleteRequest();
        }
    }
    static void CompleteRequest()
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
                task = TaskManager.CreateHaulToStorageTask(request);
            }
            else
            {
                task = TaskManager.GetTask(type, request);
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