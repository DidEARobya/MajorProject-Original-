using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class CharacterController : InventoryOwner
{
    public new float x
    {
        get { return Mathf.Lerp(currentTile.x, nextTile.x, movementPercentage);}
    }
    public new float y
    {
        get { return Mathf.Lerp(currentTile.y, nextTile.y, movementPercentage); }
    }

    public GameObject characterObj;
    public Inventory inventory;

    public Tile currentTile;
    public Tile nextTile;
    public Tile destinationTile;

    public Path_AStar pathFinder;

    float movementPercentage = 0f;
    float movementSpeed = 2f;

    public float workSpeed = 20f;

    public Task activeTask;
    public List<Task> taskList = new List<Task>();

    float workDelay = 0f;
    public HashSet<Task> ignoredTasks = new HashSet<Task>(); 

    Action<CharacterController> characterUpdateCallback;
    
    public List<TaskType> priorityList = new List<TaskType>();
    public TaskPriorityDict priorityDict;
    public GameObject priorityDisplay;

    public bool requestedTask;
    public bool requestedPath;

    GOAP_Goal lastGoal;
    public GOAP_Goal currentGoal;
    public GOAP_Plan actionPlan;
    public GOAP_Action currentAction;

    public Dictionary<string, GOAP_Belief> beliefs;
    public HashSet<GOAP_Action> actions;
    public HashSet<GOAP_Goal> goals;

    IGoapPlanner goalPlanner;

    public CharacterController(Tile tile) : base (InventoryOwnerType.CHARACTER)
    {
        currentTile = tile;
        nextTile = currentTile;
        destinationTile = currentTile;

        InventoryManager.CreateNewInventory(ownerType, null, this);

        SetupBeliefs();
        SetupActions();
        SetupGoals();

        goalPlanner = new GOAP_Planner();

        priorityList.Add(TaskType.CONSTRUCTION);
        priorityList.Add(TaskType.MINING);
        priorityList.Add(TaskType.AGRICULTURE);
        priorityList.Add(TaskType.HAULING);

        priorityDict = new TaskPriorityDict();
        priorityDict.Init(this);
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
        actions.Add(new GOAP_Action.Builder("Work").WithStrategy(new WorkStrategy(this)).AddEffect(beliefs["Working"]).Build());
    }

    void SetupGoals()
    {
        goals = new HashSet<GOAP_Goal>();

        goals.Add(new GOAP_Goal.Builder("Nothing").WithPriority(1).AddDesiredEffect(beliefs["Nothing"]).Build());
        goals.Add(new GOAP_Goal.Builder("Work").WithPriority(1).AddDesiredEffect(beliefs["Working"]).Build());
    }
    public void SetCharacterObj(GameObject obj)
    {
        characterObj = obj;
    }
    void CalculatePlan()
    {
        int priorityLevel = currentGoal?.priority ?? 0;

        HashSet<GOAP_Goal> goalsToCheck = goals;

        if(currentGoal != null)
        {
            goalsToCheck = new HashSet<GOAP_Goal>(goals.Where(g => g.priority > priorityLevel));
        }

        GOAP_Plan potentialPlan = goalPlanner.Plan(this, goalsToCheck, lastGoal);

        Debug.Log(potentialPlan.goal.name);

        if(potentialPlan != null)
        {
            actionPlan = potentialPlan;
        }
    }
    public void Update(float deltaTime)
    {
        if(currentAction == null)
        {
            CalculatePlan();

            if(actionPlan != null && actionPlan.actions.Count > 0) 
            { 
                pathFinder = null;

                currentGoal = actionPlan.goal;
                currentAction = actionPlan.actions.Pop();
                currentAction.Start();
            }
        }

        if(actionPlan != null && currentAction != null)
        {
            currentAction.Update(deltaTime);

            if(currentAction.Complete)
            {
                currentAction.Stop();
                currentAction = null;

                if(actionPlan.actions.Count == 0)
                {
                    lastGoal = currentGoal;
                    currentGoal = null;
                }
            }
        }

        if (requestedPath == true)
        {
            return;
        }

        if (pathFinder != null || currentTile != nextTile)
        {
            TraversePath(deltaTime);
        }

        if (activeTask == null)
        {
            if (taskList.Count == 0)
            {
                return;
            }

            SetActiveTask(taskList[0], false);
        }

        if (DoWork(deltaTime) == true)
        {
            return;
        }

        if (requestedPath == false && pathFinder == null && activeTask != null)
        {
            if (activeTask.taskType == TaskType.HAULING)
            {
                activeTask.CancelTask(false);
            }
            else
            {
                activeTask.CancelTask(true, true);
            }
        }
        return;

        if (activeTask == null && taskList.Count == 0)
        {
            workDelay += deltaTime;

            if (workDelay >= 0.2f && requestedTask == false)
            {
                GameManager.GetTaskRequestHandler().RequestTask(this);

                workDelay = 0f;
            }

            return;
        }

        if(activeTask == null)
        {
            if (taskList.Count == 0)
            {
                return;
            }

            SetActiveTask(taskList[0], false);
        }

        if(DoWork(deltaTime) == true)
        {
            return;
        }

        if (requestedPath == false && pathFinder == null && activeTask != null)
        {
            if (activeTask.taskType == TaskType.HAULING)
            {
                activeTask.CancelTask(false);
            }
            else
            {
                activeTask.CancelTask(true, true);
            }
        }
    }
    public void SetActiveTask(Task task, bool requeue)
    {
        if (taskList.Contains(task))
        {
            taskList.Remove(task);
        }

        if(requeue == true)
        {
            activeTask.RemoveTaskCompleteCallback(EndTask);
            taskList.Add(activeTask);
        }

        activeTask = task;
        activeTask.AddTaskCompleteCallback(EndTask);

        activeTask.InitTask(this);
    }
    bool DoWork(float deltaTime)
    {
        if(activeTask == null)
        {
            return false;
        }

        if(destinationTile == currentTile)
        {
            activeTask.DoWork(deltaTime);
            return true;
        }

        return false;
    }
    void TraversePath(float deltaTime)
    {
        if (pathFinder == null)
        {
            if (currentTile != nextTile)
            {
                Move(deltaTime);
            }

            return;
        }

        if (nextTile == null || nextTile == currentTile)
        {
            nextTile = pathFinder.DequeueNextTile();

            if (nextTile == null)
            {
                return;
            }
        }

        if (nextTile.IsAccessible() == Accessibility.IMPASSABLE)
        {
            nextTile = currentTile;
            pathFinder = null;
            PathRequestHandler.RequestPath(this, destinationTile, true);
            return;
        }

        if (nextTile.IsAccessible() == Accessibility.DELAYED)
        {
            return;
        }

        Move(deltaTime);
    }

    void Move(float deltaTime)
    {
        if(currentTile == null || nextTile == null)
        {
            return;
        }

        float distToTravel = Mathf.Sqrt(Mathf.Pow(currentTile.x - nextTile.x, 2) + Mathf.Pow(currentTile.y - nextTile.y, 2));

        float distThisFrame = movementSpeed * deltaTime;

        float percentageThisFrame = distThisFrame / distToTravel;

        movementPercentage += percentageThisFrame;

        if (movementPercentage >= 1)
        {
            currentTile = nextTile;
            movementPercentage = 0;

            if (destinationTile == currentTile)
            {
                currentTile.reservedBy = this;
                pathFinder = null;
                return;
            }
        }

        if (characterUpdateCallback != null)
        {
            characterUpdateCallback(this);
        }
    }
    public void DropInventory()
    {
        InventoryManager.DropInventory(inventory, currentTile);
    }
    public void SetDestination(Tile tile)
    {
        currentTile.reservedBy = null;
        destinationTile = tile;
    }
    public void AddCharacterUpdate(Action<CharacterController> callback)
    {
        characterUpdateCallback += callback;
    }
    public void RemoveCharacterUpdate(Action<CharacterController> callback)
    {
        characterUpdateCallback -= callback;
    }

    public void CancelTask(Task task)
    {
        activeTask = null;
        pathFinder = null;
        
        if(taskList.Contains(task))
        {
            taskList.Remove(task);
        }
    }
    public void EndTask(Task task)
    {
        if(activeTask == task)
        {
            activeTask = null;
            return;
        }

        Debug.Log("Ending Wrong Task");
    }
}
