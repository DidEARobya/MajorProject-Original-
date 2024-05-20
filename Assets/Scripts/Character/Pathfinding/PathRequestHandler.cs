using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PathRequestHandler : MonoBehaviour
{
    static Queue<PathRequest> requests = new Queue<PathRequest>();

    static Object requestCompleteLock = new Object();

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
                isHandlingRequest = false;
                return;
            } 

            CharacterController character = request.character;

            Path_AStar path = new Path_AStar(character.currentTile, request.destination, true);

            Debug.Log(path.Length());
            character.pathFinder = path;
            request.character.requestedPath = false;

            isHandlingRequest = false;
        }
    }
}
public struct PathRequest
{
    public CharacterController character;
    public Tile destination;

    public PathRequest(CharacterController _character, Tile _tile)
    {
        character = _character;
        destination = _tile;
    }
}