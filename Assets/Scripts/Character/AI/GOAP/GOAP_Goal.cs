using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_Goal
{
    public string name;
    public int priority;

    public HashSet<GOAP_Belief> desiredEffects = new HashSet<GOAP_Belief>();

    GOAP_Goal(string _name)
    {
        name = _name;
    }

    public class Builder
    {
        readonly GOAP_Goal goal;

        public Builder(string name)
        {
            goal = new GOAP_Goal(name);
        }

        public Builder WithPriority(int priority)
        {
            goal.priority = priority;
            return this;
        }
        public Builder AddDesiredEffect(GOAP_Belief effect)
        {
            goal.desiredEffects.Add(effect);
            return this;
        }
        public GOAP_Goal Build()
        {
            return goal;
        }
    }
}
