using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionStrategy
{
    public bool CanPerform { get; }

    public bool Complete { get; }

    void Start()
    {

    }
    void Update(float deltaTime)
    {

    }
    void Stop()
    {

    }
}
public class WorkStrategy : IActionStrategy
{
    CharacterController agent;

    public bool CanPerform => !Complete;
    public bool Complete
    {
        get { return agent.requestedTask == false && agent.activeTask == null && agent.taskList.Count == 0; }
        private set { }
    }

    public WorkStrategy(CharacterController _agent)
    {
        agent = _agent;
    }

    public void Start()
    {
        GameManager.GetTaskRequestHandler().RequestTask(agent);
    }

    public void Stop()
    {
        Complete = false;
    }
}
public class IdleStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Complete
    {
        get; private set;
    }

    float timer;
    float delay;

    public IdleStrategy(float _delay)
    {
        delay = _delay;
    }

    public void Start()
    {
        timer = 0;
    }

    public void Update(float deltaTime)
    {
        timer += deltaTime;

        if (timer >= delay)
        {
            Complete = true;
        }
    }

    public void Stop()
    {
        timer = 0;
        Complete = false;
    }
}
