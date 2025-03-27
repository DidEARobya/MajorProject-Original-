using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PathRequestHandler
{
    static Queue<PathRequest> requests = new Queue<PathRequest>();

    static object requestCompleteLock = new object();

    static bool isHandlingRequest = false;

    public static void RequestPath(CharacterController character, Tile destination, bool acceptNeighbours)
    {
        //Temporary solution to correct stuck characters
        if(character.currentTile.accessibility == Accessibility.IMPASSABLE)
        {
            Tile tile = character.currentTile.GetNearestAvailableTile();
            character.currentTile = tile;
            Debug.Log("CHARACTER STUCK");
        }

        PathRequest request = new PathRequest(character, destination, acceptNeighbours);  
        
        if (requests.Contains(request))
        {
            return;
        }

        request.character.requestedPath = true;
        requests.Enqueue(request);
        return;
    }
    public static void Update()
    {
        if (requests.Count > 0 && isHandlingRequest == false)
        {
            ThreadPool.QueueUserWorkItem(delegate { ThreadedCompleteRequest(); });
        }
    }
    static void ThreadedCompleteRequest()
    {
        lock (requestCompleteLock)
        {
            if (requests.Count == 0)
            {
                return;
            }

            isHandlingRequest = true;

            PathRequest request = requests.Dequeue();

            if (request.character == null || request.destination == null)
            {
                isHandlingRequest = false;
                return;
            } 

            CharacterController character = request.character;

            Path_AStar path = new Path_AStar();

            if(path.IsRegionPathable(character.currentTile.region, request.destination.region, request.destination, true) == false)
            {
                NoValidPath(character);
                path = null;
                return;
            }

            if (path.TilePathfind(character.currentTile, request.destination, true, request.acceptNeighbours) == false)
            {
                NoValidPath(character);
                path = null;
                return;
            }

            character.SetDestination(path.destination);
            character.pathFinder = path;
            character.requestedPath = false;

            isHandlingRequest = false;
        }
    }
    static void NoValidPath(CharacterController character)
    {
        Debug.Log("No Path");
        isHandlingRequest = false;

        if (character.activeTask != null)
        {
            if (character.activeTask.taskType == TaskType.HAULING)
            {
                character.activeTask.CancelTask(false);
            }
            else
            {
                character.activeTask.CancelTask(true);
            }
        }

        character.pathFinder = null;
        character.requestedPath = false;
    }
}
public struct PathRequest
{
    public CharacterController character;
    public Tile destination;
    public bool acceptNeighbours;
    public PathRequest(CharacterController _character, Tile _tile, bool _acceptNeighbours)
    {
        character = _character;
        destination = _tile;
        acceptNeighbours = _acceptNeighbours;
    }
}