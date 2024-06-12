using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public enum TaskType
{
    CONSTRUCTION,
    MINING,
    AGRICULTURE,
    HAULING
}
public enum PriorityLevel
{
    ONE, 
    TWO, 
    THREE, 
    FOUR
}
public class TaskPriorityDict
{
    CharacterController owner;
    List<PriorityLevel> levelList = new List<PriorityLevel>();

    public void Init(CharacterController character)
    {
        owner = character;

        levelList.Add(PriorityLevel.ONE);
        levelList.Add(PriorityLevel.ONE);
        levelList.Add(PriorityLevel.ONE);
        levelList.Add(PriorityLevel.ONE);
    }

    public Task GetTask()
    {
        var length = Enum.GetNames(typeof(TaskType)).Length;

        for (int i = 0; i < length; i++)
        {
            Task task = CheckForTask((PriorityLevel)i);

            if (task != null)
            {
                return task;
            }
        }

        return null;
    }

    Task CheckForTask(PriorityLevel level)
    {
        for(int i = 0; i < levelList.Count; i++)
        {
            if (levelList[i] != level)
            {
                continue;
            }

            TaskType type = (TaskType)i;

            Task task = GameManager.GetTaskManager().GetTask(type, owner);

            if(task != null)
            {
                return task;
            }
        }

        return null;
    }
}
