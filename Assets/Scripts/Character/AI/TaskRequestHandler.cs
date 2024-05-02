using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class TaskRequestHandler
{
    static Queue<TaskRequest> requests = new Queue<TaskRequest>();

    static Thread requestCompleteThread;
    static Object requestCompleteLock = new Object();

    static bool isHandlingRequest = false;

    public static void Init()
    {
        requestCompleteThread = new Thread(ThreadedCompleteRequest);
    }
    public static void RequestTask(TaskRequest request)
    {
        if(requests.Contains(request))
        {
            return;
        }

        requests.Enqueue(request);
    }
    public static void Update()
    {
        if (requests.Count > 0 && isHandlingRequest == false)
        {
            ThreadPool.QueueUserWorkItem(delegate { ThreadedCompleteRequest(); });
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

            TaskRequest request = requests.Dequeue();

            if (request.character == null)
            {
                isHandlingRequest = false;
                return;
            }

            request.character.requestedTask = false;
            Task task = TaskManager.GetTask(request.taskType, request.character);

            if (task != null)
            {
                request.character.taskList.Add(task);
            }

            isHandlingRequest = false;
        }
    }
}
public struct TaskRequest
{
    public CharacterController character;
    public TaskType taskType;

    public TaskRequest(CharacterController _character, TaskType _taskType)
    {
        character = _character;
        taskType = _taskType;
    }
}