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