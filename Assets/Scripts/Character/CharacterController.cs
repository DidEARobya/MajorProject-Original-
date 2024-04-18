using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController
{
    public float x
    {
        get { return Mathf.Lerp(currentTile.x, nextTile.x, movementPercentage);}
    }
    public float y
    {
        get { return Mathf.Lerp(currentTile.y, nextTile.y, movementPercentage); }
    }

    public GameObject characterObj;

    public Tile currentTile;
    public Tile nextTile;
    public Tile destinationTile;

    Path_AStar pathFinder;

    float movementPercentage = 0f;
    float movementSpeed = 2f;

    Task activeTask;

    Action<CharacterController> characterUpdateCallback;

    public CharacterController(Tile tile)
    {
        currentTile = tile;
        nextTile = currentTile;
        destinationTile = currentTile;
    }

    public void SetCharacterObj(GameObject obj)
    {
        characterObj = obj;
    }

    void Work(float deltaTime)
    {
        if (activeTask == null)
        {
            activeTask = TaskManager.GetTask(TaskType.CONSTRUCTION);

            if (activeTask == null)
            {
                return;
            }

            activeTask.worker = this;
            activeTask.AddTaskCompleteCallback(EndTask);
            activeTask.AddTaskCancelledCallback(EndTask);

            destinationTile = activeTask.tile;
        }

        if (currentTile == destinationTile)
        {
            activeTask.DoWork(deltaTime);
        }
    }
    void Move(float deltaTime)
    {
        if (currentTile == destinationTile)
        {
            pathFinder = null;
            return;
        }

        if(nextTile == null || nextTile == currentTile)
        {
            if(pathFinder == null || pathFinder.Length() == 0)
            {
                pathFinder = new Path_AStar(GameManager.GetWorldController().worldGrid, currentTile, destinationTile);

                if(pathFinder.Length() == 0)
                {
                    Debug.Log("Return no path");
                    CancelTask();
                    pathFinder = null;
                    return;
                }
            }

            nextTile = pathFinder.DequeueNextTile();

            if(nextTile == null)
            {
                return;
            }
        }

        float distToTravel = Mathf.Sqrt(Mathf.Pow(currentTile.x - nextTile.x, 2) + Mathf.Pow(currentTile.y - nextTile.y, 2));

        float distThisFrame = movementSpeed * deltaTime;

        float percentageThisFrame = distThisFrame / distToTravel;

        movementPercentage += percentageThisFrame;

        if (movementPercentage >= 1)
        {
            currentTile = nextTile;
            movementPercentage = 0;
        }

        if (characterUpdateCallback != null)
        {
            characterUpdateCallback(this);
        }
    }
    public void Update(float deltaTime)
    {
        Work(deltaTime);
        Move(deltaTime);
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

        Debug.Log("Completing wrong task");
    }
}
