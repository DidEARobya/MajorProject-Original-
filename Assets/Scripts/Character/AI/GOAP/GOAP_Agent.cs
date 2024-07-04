using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GOAP_Agent
{
    CharacterController character;
    public Tile currentTile => character.currentTile;
    Path_AStar pathFinder => character.pathFinder;
    List<Task> taskList => character.taskList;

    GOAP_Goal lastGoal;
    public GOAP_Goal currentGoal;
    public GOAP_Plan actionPlan;
    public GOAP_Action currentAction;

    public Dictionary<string, GOAP_Belief> beliefs;
    public HashSet<GOAP_Action> actions;
    public HashSet<GOAP_Goal> goals;

    IGoapPlanner goalPlanner;

    public GOAP_Agent(CharacterController _character)
    {
        character = _character;

        SetupBeliefs();
        SetupActions();
        SetupGoals();

        goalPlanner = new GOAP_Planner();
    }
    void SetupBeliefs()
    {
        beliefs = new Dictionary<string, GOAP_Belief>();

        BeliefFactory factory = new BeliefFactory(this, beliefs);

        factory.AddBelief("Nothing", () => false);
        factory.AddBelief("Idle", () => pathFinder == null);
        factory.AddBelief("Moving", () => pathFinder != null);
        factory.AddBelief("Working", () => taskList.Count != 0);
    }

    void SetupActions()
    {
        actions = new HashSet<GOAP_Action>();

        actions.Add(new GOAP_Action.Builder("Relax").WithStrategy(new IdleStrategy(5)).AddEffect(beliefs["Nothing"]).Build());
        actions.Add(new GOAP_Action.Builder("Work").WithStrategy(new WorkStrategy(character)).AddEffect(beliefs["Working"]).Build());
    }

    void SetupGoals()
    {
        goals = new HashSet<GOAP_Goal>();

        goals.Add(new GOAP_Goal.Builder("Nothing").WithPriority(1).AddDesiredEffect(beliefs["Nothing"]).Build());
        goals.Add(new GOAP_Goal.Builder("Work").WithPriority(1).AddDesiredEffect(beliefs["Working"]).Build());
    }
    public void Update(float deltaTime)
    {
        if (currentAction == null)
        {
            CalculatePlan();

            if (actionPlan != null && actionPlan.actions.Count > 0)
            {
                character.pathFinder = null;

                currentGoal = actionPlan.goal;
                currentAction = actionPlan.actions.Pop();
                currentAction.Start();
            }
        }

        if (actionPlan != null && currentAction != null)
        {
            currentAction.Update(deltaTime);

            if (currentAction.Complete)
            {
                currentAction.Stop();
                currentAction = null;

                if (actionPlan.actions.Count == 0)
                {
                    lastGoal = currentGoal;
                    currentGoal = null;
                }
            }
        }
    }
    void CalculatePlan()
    {
        int priorityLevel = currentGoal?.priority ?? 0;

        HashSet<GOAP_Goal> goalsToCheck = goals;

        if (currentGoal != null)
        {
            goalsToCheck = new HashSet<GOAP_Goal>(goals.Where(g => g.priority > priorityLevel));
        }

        GOAP_Plan potentialPlan = goalPlanner.Plan(this, goalsToCheck, lastGoal);

        Debug.Log(potentialPlan.goal.name);

        if (potentialPlan != null)
        {
            actionPlan = potentialPlan;
        }
    }
}
