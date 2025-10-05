using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
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
    public List<PriorityLevel> levelList = new List<PriorityLevel>();

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
        //Task t = CheckForTask(PriorityLevel.ONE);
        //return t;

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
        for (int i = 0; i < levelList.Count; i++)
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