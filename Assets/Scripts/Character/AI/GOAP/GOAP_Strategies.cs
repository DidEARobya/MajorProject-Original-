using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public interface IActionStrategy
{
    public bool CanPerform { get; }
    public bool Complete { get; }

    void Start() { }
    void Update(float deltaTime) { }
    void Stop() { }
}
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

        if (character.requestedPath == false && character.pathFinder == null && character.activeTask != null)
        {
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
            return false;
        }

        if (character.destinationTile == character.currentTile)
        {
            agent.stamina -= 2 * deltaTime;
            character.activeTask.DoWork(deltaTime);
            return true;
        }

        return false;
    }
}
public class RestStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Complete
    {
        get; private set;
    }

    GOAP_Agent agent;
    public RestStrategy(GOAP_Agent _agent)
    {
        agent = _agent;
    }

    public void Start()
    {

    }

    public void Update(float deltaTime)
    {
        if(agent.stamina < 100)
        {
            agent.stamina += 2.5f * deltaTime;

            if(agent.stamina > 100)
            {
                agent.stamina = 100;
            }
        }

        if (agent.stamina >= 70)
        {
            Complete = true;
        }
    }

    public void Stop()
    {
        Complete = false;
    }
}
public class WanderStrategy : IActionStrategy
{
    GOAP_Agent agent;
    CharacterController character;

    int radius;
    public bool CanPerform => !Complete;
    public bool Complete
    {
        get { return character.taskList.Count != 0 || (character.requestedPath == false && character.pathFinder == null && agent.isSocialising == false); }
        private set { }
    }

    public WanderStrategy(GOAP_Agent _agent, int _radius)
    {
        agent = _agent;
        character = agent.character;
        radius = _radius;
    }

    public void Start()
    {
        Tile start = character.currentTile;

        Tile tile = GameManager.GetWorldGrid().GetRandomTileUsingRadius(start, radius);

        if(tile == null)
        {
            return;
        }

        PathRequestHandler.RequestPath(character, tile, false);
    }
    public void Stop()
    {
        Complete = false;
    }
}
