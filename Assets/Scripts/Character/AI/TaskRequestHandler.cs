using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TaskRequestHandler
{
    static Queue<TaskRequest> requests = new Queue<TaskRequest>();

    static bool handlingRequest;
    public static void RequestTask(CharacterController character, TaskType taskType)
    {
        requests.Enqueue(new TaskRequest(character, taskType));
        character.requestedTask = true;
    }
    public static void Update()
    {
        if(requests.Count == 0)
        {
            return;
        }

        if(handlingRequest == true)
        {
            return;
        }

        CompleteRequest();
    }
    static void CompleteRequest()
    {
        handlingRequest = true;

        TaskRequest request = requests.Dequeue();
        Task task = TaskManager.GetTask(request.taskType, request.character);

        if(task != null)
        {
            request.character.taskList.Add(task);
        }

        request.character.requestedTask = false;

        handlingRequest = false;
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