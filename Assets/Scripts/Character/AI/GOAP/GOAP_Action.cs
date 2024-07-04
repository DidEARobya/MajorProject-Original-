using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class GOAP_Action
{
    public string name;
    public int cost;

    public HashSet<GOAP_Belief> preconditions = new HashSet<GOAP_Belief>();
    public HashSet<GOAP_Belief> effects = new HashSet<GOAP_Belief>();

    IActionStrategy strategy;
    public bool Complete => strategy.Complete;

    GOAP_Action(string _name)
    {
        name = _name;
    }
    public void Start()
    {
        strategy.Start();
    }

    public void Update(float deltaTime)
    {
        if(strategy.CanPerform == true)
        {
            strategy.Update(deltaTime);
        }

        if(strategy.Complete == false)
        {
            return;
        }

        foreach(GOAP_Belief belief in effects)
        {
            belief.Evaluate();
        }
    }
    public void Stop()
    {
        strategy.Stop();
    }

    public class Builder
    {
        readonly GOAP_Action action;

        public Builder(string name)
        {
            action = new GOAP_Action(name);
        }

        public Builder WithCost(int cost)
        {
            action.cost = cost;
            return this;
        }
        public Builder WithStrategy(IActionStrategy strategy)
        {
            action.strategy = strategy;
            return this;
        }
        public Builder AddPrecondition(GOAP_Belief precondition)
        {
            action.preconditions.Add(precondition);
            return this;
        }
        public Builder AddEffect(GOAP_Belief effect)
        {
            action.effects.Add(effect);
            return this;
        }
        public GOAP_Action Build()
        {
            return action;
        }
    }
}

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
        return;

        Debug.Log("Start");
        foreach(Tile tile in agent.currentTile.GetNeighboursDict().Keys)
        {
            if(tile.IsAccessible() == Accessibility.IMPASSABLE)
            {
                continue;
            }

            PathRequestHandler.RequestPath(agent, tile, false);
            return;
        }
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
        
        if(timer >= delay)
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