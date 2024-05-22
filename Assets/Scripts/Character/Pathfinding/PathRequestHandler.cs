using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class PathRequestHandler : MonoBehaviour
{
    static Queue<PathRequest> requests = new Queue<PathRequest>();

    static object requestCompleteLock = new object();

    static bool isHandlingRequest = false;

    public static void RequestPath(CharacterController character, Tile destination)
    {
        PathRequest request = new PathRequest(character, destination);  
        
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
                request = null;
                isHandlingRequest = false;
                return;
            } 

            CharacterController character = request.character;

            Path_AStar path = new Path_AStar();
            bool isValid = path.TilePathfind(character.currentTile, request.destination, true);

            if(isValid == false || (path.Length() == 0 && character.currentTile.IsNeighbour(request.destination) == false && character.currentTile != request.destination))
            {
                NoValidPath(character);
                request = null;
                path = null;
                return;
            }

            character.SetDestination(path.destination);
            character.pathFinder = path;
            character.requestedPath = false;

            isHandlingRequest = false;
            request = null;
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
public class PathRequest
{
    public CharacterController character;
    public Tile destination;

    public PathRequest(CharacterController _character, Tile _tile)
    {
        character = _character;
        destination = _tile;
    }
}