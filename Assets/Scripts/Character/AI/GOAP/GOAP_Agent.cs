using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GOAP_Agent
{
    public CharacterController character;
    public Tile currentTile => character.currentTile;
    List<Task> taskList => character.taskList;

    GOAP_Goal lastGoal;
    public GOAP_Goal currentGoal;
    public GOAP_Plan actionPlan;
    public GOAP_Action currentAction;

    public Dictionary<string, GOAP_Belief> beliefs;
    public HashSet<GOAP_Action> actions;
    public HashSet<GOAP_Goal> goals;

    IGoapPlanner goalPlanner;

    public bool isSocialising = false;

    public float stamina = 100;
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

        factory.AddBelief("Tired", () => stamina < 30);
        factory.AddBelief("Rested", () => stamina >= 30);
        factory.AddBelief("Exhausted", () => stamina <= 0);

        factory.AddBelief("Working", () => taskList.Count == 0);
        factory.AddBelief("Wandering", () => taskList.Count != 0);
    }

    void SetupActions()
    {
        actions = new HashSet<GOAP_Action>();

        actions.Add(new GOAP_Action.Builder("PassOut").WithStrategy(new RestStrategy(this)).AddPrecondition(beliefs["Exhausted"]).AddEffect(beliefs["Rested"]).Build());  
        actions.Add(new GOAP_Action.Builder("Rest").WithStrategy(new RestStrategy(this)).AddPrecondition(beliefs["Tired"]).AddEffect(beliefs["Rested"]).Build());

        actions.Add(new GOAP_Action.Builder("Work").WithStrategy(new WorkStrategy(this)).AddEffect(beliefs["Working"]).Build());
        actions.Add(new GOAP_Action.Builder("Wander").WithStrategy(new WanderStrategy(this, 5)).AddEffect(beliefs["Wandering"]).Build());
    }

    void SetupGoals()
    {
        goals = new HashSet<GOAP_Goal>();

        goals.Add(new GOAP_Goal.Builder("Rest").WithPriority(1).AddDesiredEffect(beliefs["Rested"]).Build());
        goals.Add(new GOAP_Goal.Builder("PassOut").WithPriority(4).AddDesiredEffect(beliefs["Rested"]).Build());

        goals.Add(new GOAP_Goal.Builder("Work").WithPriority(3).AddDesiredEffect(beliefs["Working"]).Build());
        goals.Add(new GOAP_Goal.Builder("Wander").WithPriority(2).AddDesiredEffect(beliefs["Wandering"]).Build());
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
            if (beliefs["Exhausted"].Evaluate() == true && currentGoal.name != "PassOut")
            {
                currentAction.Stop();
                actionPlan = null;
            }

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

        if (potentialPlan != null)
        {
            Debug.Log(potentialPlan.goal.name);
            actionPlan = potentialPlan;
        }
    }
}
