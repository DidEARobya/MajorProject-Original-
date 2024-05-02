using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UIElements;

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

    public bool requestedTask;
    public bool isWorking;
    public CharacterController(Tile tile) : base (InventoryOwnerType.CHARACTER)
    {
        currentTile = tile;
        nextTile = currentTile;
        destinationTile = currentTile;

        InventoryManager.CreateNewInventory(ownerType, null, this);

        priorityList.Add(TaskType.CONSTRUCTION);
        priorityList.Add(TaskType.MINING);
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

            if (workDelay >= 0.5f && requestedTask == false)
            {
                TaskRequestHandler.RequestTask(this);

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

        DoWork(deltaTime);
    }
    public void SetActiveTask(Task task, bool requeue)
    {
        if (taskList.Contains(task))
        {
            taskList.Remove(task);
        }

        if(requeue == true)
        {
            taskList.Add(activeTask);
        }

        activeTask = task;

        activeTask.AddTaskCompleteCallback(EndTask);
        activeTask.AddTaskCancelledCallback(EndTask);

        activeTask.InitTask(this);

        if(activeTask == null)
        {
            return;
        }

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
