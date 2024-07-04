using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IGoapPlanner
{
    GOAP_Plan Plan(CharacterController agent, HashSet<GOAP_Goal> goals, GOAP_Goal lastGoal = null);
}

public class GOAP_Planner : IGoapPlanner
{
    public GOAP_Plan Plan(CharacterController agent, HashSet<GOAP_Goal> goals, GOAP_Goal lastGoal = null)
    {
        List<GOAP_Goal> orderedGoals = goals
        .Where(g => g.desiredEffects.Any(b => !b.Evaluate()))
        .OrderByDescending(g => g == lastGoal ? g.priority - 1 : g.priority)
        .ToList();

        foreach(GOAP_Goal goal in orderedGoals)
        {
            GOAP_Node goalNode = new GOAP_Node(null, null, goal.desiredEffects, 0);

            if(FindPath(goalNode, agent.actions))
            {
                if(goalNode.IsDead == true)
                {
                    continue;
                }

                Stack<GOAP_Action> actionStack = new Stack<GOAP_Action>();

                while(goalNode.leaves.Count > 0)
                {
                    GOAP_Node cheapestLeaf = goalNode.leaves.OrderBy(leaf => leaf.cost).First();
                    goalNode = cheapestLeaf;
                    actionStack.Push(cheapestLeaf.action);
                }

                return new GOAP_Plan(goal, actionStack, goalNode.cost);
            }
        }

        Debug.Log("No Plan");
        return null;
    }

    bool FindPath(GOAP_Node parent, HashSet<GOAP_Action> actions)
    {
        foreach(GOAP_Action action in actions)
        {
            HashSet<GOAP_Belief> requiredEffects = parent.requiredEffects;

            requiredEffects.RemoveWhere(b => b.Evaluate());

            if(requiredEffects.Count == 0)
            {
                return true;
            }

            if(action.effects.Any(requiredEffects.Contains))
            {
                HashSet<GOAP_Belief> newRequiredEffects = new HashSet<GOAP_Belief>(requiredEffects);
                newRequiredEffects.ExceptWith(action.effects);
                newRequiredEffects.UnionWith(action.preconditions);

                HashSet<GOAP_Action> newAvailableActions = new HashSet<GOAP_Action>(actions);
                newAvailableActions.Remove(action);

                GOAP_Node newNode = new GOAP_Node(parent, action, newRequiredEffects, parent.cost + action.cost);

                if(FindPath(newNode, newAvailableActions))
                {
                    parent.leaves.Add(newNode);
                    newRequiredEffects.ExceptWith(newNode.action.preconditions);
                }

                if(newRequiredEffects.Count == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
public class GOAP_Node
{
    public GOAP_Node parent;
    public GOAP_Action action;
    public HashSet<GOAP_Belief> requiredEffects;
    public List<GOAP_Node> leaves;
    public int cost;

    public bool IsDead => leaves.Count == 0 && action == null;

    public GOAP_Node(GOAP_Node _parent, GOAP_Action _action, HashSet<GOAP_Belief> effects, int _cost)
    {
        parent = _parent;
        action = _action;
        requiredEffects = effects;
        leaves = new List<GOAP_Node>();
        cost = _cost;
    }
}
public class GOAP_Plan
{
    public GOAP_Goal goal;
    public Stack<GOAP_Action> actions;
    public int totalCost;

    public GOAP_Plan(GOAP_Goal _goal, Stack<GOAP_Action> _actions, int cost)
    {
        goal = _goal;
        actions = _actions;
        totalCost = cost;
    }
}
