using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Task activeTask;
    public Stack<Task> taskStack = new Stack<Task>();

    float workDelay = 0f;
    public HashSet<Task> ignoredTasks = new HashSet<Task>(); 

    Action<CharacterController> characterUpdateCallback;

    public CharacterController(Tile tile) : base (InventoryOwnerType.CHARACTER)
    {
        currentTile = tile;
        nextTile = currentTile;
        destinationTile = currentTile;

        InventoryManager.CreateNewInventory(ownerType, null, this);
    }

    public void SetCharacterObj(GameObject obj)
    {
        characterObj = obj;
    }
    public void Update(float deltaTime)
    {
        FindWork(deltaTime);
        DoWork(deltaTime);
        TraversePath(deltaTime);
    }

    void FindWork(float deltaTime)
    {
        workDelay += deltaTime;

        if (activeTask == null)
        {
            if(workDelay < 0.5f)
            {
                return;
            }

            if(taskStack.Count > 0)
            {
                activeTask = taskStack.Pop();

                pathFinder = new Path_AStar(currentTile, activeTask.tile, true);

                if(pathFinder == null)
                {
                    activeTask.CancelTask(true);
                }
            }
            else
            {
                activeTask = TaskManager.GetTask(TaskType.CONSTRUCTION, this);
            }

            if (activeTask == null)
            {
                return;
            }

            if(pathFinder == null)
            {
                pathFinder = activeTask.path;
            }

            workDelay = 0f;

            activeTask.worker = this;
            activeTask.AddTaskCompleteCallback(EndTask);
            activeTask.AddTaskCancelledCallback(EndTask);

            destinationTile = activeTask.tile;
        }
    }
    public void UpdateTask(Task task)
    {
        activeTask = task;

        pathFinder = activeTask.path;
        workDelay = 0f;

        activeTask.worker = this;
        activeTask.AddTaskCompleteCallback(EndTask);
        activeTask.AddTaskCancelledCallback(EndTask);

        destinationTile = activeTask.tile;
    }
    void DoWork(float deltaTime)
    {
        if(activeTask == null)
        {
            return;
        }

        if (destinationTile.IsNeighbour(currentTile) == true || destinationTile == currentTile)
        {
            activeTask.DoWork(deltaTime);
        }
    }
    void TraversePath(float deltaTime)
    {
        if(pathFinder == null)
        {
            return;
        }

        if (nextTile == null || nextTile == currentTile)
        {
            if (pathFinder == null || pathFinder.Length() == 0)
            {
                if (pathFinder.Length() == 0)
                {
                    CancelTask();
                    pathFinder = null;
                    return;
                }
            }

            nextTile = pathFinder.DequeueNextTile();

            if (nextTile == null)
            {
                return;
            }
        }

        if(nextTile.IsAccessible() == Accessibility.DELAYED)
        {
            return;
        }

        Move(deltaTime);
    }

    void Move(float deltaTime)
    {
        float distToTravel = Mathf.Sqrt(Mathf.Pow(currentTile.x - nextTile.x, 2) + Mathf.Pow(currentTile.y - nextTile.y, 2));

        float distThisFrame = movementSpeed * deltaTime;

        float percentageThisFrame = distThisFrame / distToTravel;

        movementPercentage += percentageThisFrame;

        if (movementPercentage >= 1)
        {
            currentTile = nextTile;
            movementPercentage = 0;

            if (destinationTile.IsNeighbour(currentTile) == true || destinationTile == currentTile)
            {
                pathFinder = null;
                return;
            }
        }

        if (characterUpdateCallback != null)
        {
            characterUpdateCallback(this);
        }
    }
    public void SetDestination(Tile tile)
    {
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

    public void CancelTask()
    {
        if(activeTask == null)
        {
            return;
        }

        activeTask.CancelTask(true);
        activeTask = null;
    }
    public void EndTask(Task task)
    {
        if(activeTask == task)
        {
            activeTask = null;
            return;
        }
    }
}
