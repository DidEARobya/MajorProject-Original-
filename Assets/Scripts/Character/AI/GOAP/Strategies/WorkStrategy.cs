using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkStrategy : IActionStrategy
{
    GOAP_Agent agent;
    CharacterController character;

    public bool CanPerform => !Complete;
    public bool Complete
    {
        get { return character.requestedTask == false && character.activeTask == null && character.taskList.Count == 0; }
        private set { }
    }

    public WorkStrategy(GOAP_Agent _agent)
    {
        agent = _agent;
        character = agent.character;
    }

    public void Start()
    {

    }
    public void Update(float deltaTime)
    {
        if (character.activeTask == null)
        {
            if (character.taskList.Count == 0)
            {
                return;
            }

            character.SetActiveTask(character.taskList[0], false);
        }

        if (DoWork(deltaTime) == true)
        {
            return;
        }

        if (character.activeTask != null && character.activeTask.IsWorkable() == false)
        {
            Debug.Log("WORK CANCEL");
            if (character.activeTask.taskType == TaskType.HAULING)
            {
                character.activeTask.CancelTask(false);
            }
            else
            {
                character.activeTask.CancelTask(true, true);
            }
        }
    }
    public void Stop()
    {
        if (character.activeTask != null)
        {
            Debug.Log("WORK CANCEL STOP");
            character.activeTask.CancelTask(true);
        }

        if (character.taskList.Count > 0)
        {
            foreach (Task task in character.taskList)
            {
                task.CancelTask(true);
            }

            character.taskList.Clear();
        }

        Complete = false;
    }

    public bool DoWork(float deltaTime)
    {
        if (character.activeTask == null)
        {
            Debug.Log("NO TASK WORKING");
            return false;
        }

        if (character.destinationTile == character.currentTile)
        {
            agent.stamina -= 2 * deltaTime;
            character.activeTask.DoWork(deltaTime);
            return true;
        }
        else
        {
            //Debug.Log("DESTINATION: " + character.destinationTile.x + "," + character.destinationTile.y);
            //Debug.Log("CURRENT: " + character.currentTile.x + "," + character.currentTile.y);
        }

        return false;
    }
}
