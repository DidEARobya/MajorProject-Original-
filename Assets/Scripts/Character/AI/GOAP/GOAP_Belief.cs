using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeliefFactory
{
    readonly CharacterController agent;
    readonly Dictionary<string, GOAP_Belief> beliefs;

    public BeliefFactory(CharacterController _agent, Dictionary<string, GOAP_Belief> _beliefs)
    {
        agent = _agent;
        beliefs = _beliefs;
    }

    public void AddBelief(string name, Func<bool> condition)
    {
        beliefs.Add(name, new GOAP_Belief.Builder(name).WithCondition(condition).Build());
    }

    public void AddLocationBelief(string name, Tile locationCondition)
    {
        beliefs.Add(name, new GOAP_Belief.Builder(name).WithCondition(() => IsNeighbour(locationCondition)).WithLocation(locationCondition).Build());
    }

    bool IsNeighbour(Tile tile) => tile.IsNeighbour(agent.currentTile);
}
public class GOAP_Belief
{
    public string name;

    Func<bool> condition = () => false;
    Tile location = null;

    GOAP_Belief(string name)
    {

    }

    public bool Evaluate()
    {
        return condition();
    }

    public class Builder
    {
        readonly GOAP_Belief belief;

        public Builder(string name)
        {
            belief = new GOAP_Belief(name);
        }

        public Builder WithCondition(Func<bool> condition)
        {
            belief.condition = condition;
            return this;
        }
        public Builder WithLocation(Tile location)
        {
            belief.location = location;
            return this;
        }
        public GOAP_Belief Build()
        {
            return belief;
        }
    }
}
