using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
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

    public float workSpeed = 20f;

    public Task activeTask;
    public List<Task> taskList = new List<Task>();

    float workDelay = 0f;
    public HashSet<Task> ignoredTasks = new HashSet<Task>(); 

    Action<CharacterController> characterUpdateCallback;

    public bool requestedTask;
    public bool isWorking;
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
        if (pathFinder != null || currentTile != nextTile)
        {
            TraversePath(deltaTime);
        }

        if (activeTask == null && taskList.Count == 0)
        {
            workDelay += deltaTime;

            if(workDelay >= 0.5f)
            {
                TaskRequestHandler.RequestTask(new TaskRequest(this, TaskType.CONSTRUCTION));
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

            SetActiveTask(taskList[0]);
        }

        DoWork(deltaTime);
    }
    public void ForcePrioritiseTask(Task task)
    {
        taskList.Add(activeTask);
        SetActiveTask(task);
    }
    void SetActiveTask(Task task)
    {
        if(taskList.Contains(task))
        {
            taskList.Remove(task);
        }

        activeTask = task;

        activeTask.AddTaskCompleteCallback(EndTask);
        activeTask.AddTaskCancelledCallback(EndTask);

        activeTask.InitTask(this);
        SetDestination(activeTask.tile);
    }
    void DoWork(float deltaTime)
    {
        if(activeTask == null)
        {
            return;
        }

        if(destinationTile.IsNeighbour(currentTile) == true || destinationTile == currentTile)
        {
            activeTask.DoWork(deltaTime);
        }
    }
    void TraversePath(float deltaTime)
    {
        if (pathFinder == null)
        {
            if(currentTile != nextTile)
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

        if (nextTile.IsAccessible() == Accessibility.DELAYED)
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
    public void DropInventory()
    {
        InventoryManager.DropInventory(inventory, currentTile);
    }
    public void SetDestination(Tile tile)
    {
        destinationTile.reservedBy = null;
        destinationTile = tile;
        destinationTile.reservedBy = this;
    }
    public void UnStuck()
    {
        pathFinder = null;

        nextTile = currentTile.GetNearestAvailableTile();
        destinationTile = nextTile;
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
    }
}
