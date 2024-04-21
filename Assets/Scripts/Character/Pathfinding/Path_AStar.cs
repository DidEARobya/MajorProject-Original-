using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Animations;

public class Path_AStar
{
    Queue<INodeData> path = new Queue<INodeData>();
    bool isPlayer;
    public Path_AStar(Tile start, Tile end, bool _isPlayer)
    {
        if (start == null || end == null)
        {
            return;
        }

        isPlayer = _isPlayer;

        WorldGrid world = GameManager.GetWorldGrid();

        if (world.pathGraph == null)
        {
            world.pathGraph = new Path_TileGraph(world);
        }

        Node startNode = start.pathNode;
        Node endNode = end.pathNode;

        List<Node> closedSet = new List<Node>();

        SimplePriorityQueue<Node> openSet = new SimplePriorityQueue<Node>();
        openSet.Enqueue(startNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();
            closedSet.Add(current);

            if (current == endNode)
            {
                RetracePath(cameFrom, current);
                return;
            }

            foreach (Node neighbour in current.neighbours)
            {
                if (closedSet.Contains(neighbour) == true)
                {
                    continue;
                }

                float tentativeGScore = current.gCost + DistanceBetween(current, neighbour);

                if (tentativeGScore < neighbour.gCost || openSet.Contains(neighbour) == false)
                {
                    neighbour.gCost = tentativeGScore;
                    neighbour.hCost = DistanceBetween(neighbour, endNode);
                    cameFrom[neighbour] = current;

                    if (openSet.Contains(neighbour) == false)
                    {
                        openSet.Enqueue(neighbour, neighbour.fCost);
                    }
                    else
                    {
                        openSet.UpdatePriority(neighbour, neighbour.fCost);
                    }
                }
            }
        }
    }

    void RetracePath(Dictionary<Node, Node> cameFrom, Node current)
    {
        Queue<INodeData> totalPath = new Queue<INodeData>();
        totalPath.Enqueue(current.data);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Enqueue(current.data);
        }

        path = new Queue<INodeData>(totalPath.Reverse());
    }

    float DistanceBetween(Node start, Node goal)
    {
        int distX = Mathf.Abs(start.x - goal.x);
        int distY = Mathf.Abs(start.y - goal.y);

        float val = 0;

        if(distX > distY)
        {
            val += 14 * distY + 10 * (distX -  distY);
        }
        else
        {
            val += 14 * distX + 10 * (distY - distX);
        }

        return val * goal.GetCost(isPlayer);
    }
    public Tile DequeueNextTile()
    {
        if(path.Count == 0)
        {
            return null;
        }

        return path.Dequeue().GetTile();
    }
    public int Length()
    {
        if(path == null)
        {
            return 0;
        }

        return path.Count;
    }
}

/*Queue<INodeData> SimplifyPath(List<INodeData> path)
{
    Queue<INodeData> waypoints = new Queue<INodeData>();
    Vector2 oldDir = Vector2.zero;

    for(int i = 1; i < path.Count; i++)
    {
        Vector2 newDir = new Vector2(path[i - 1].GetTile().x - path[i].GetTile().x, path[i - 1].GetTile().y - path[i].GetTile().y);

        if(newDir != oldDir)
        {
            waypoints.Enqueue(path[i - 1]);
        }

        oldDir = newDir;
    }

    return waypoints;
}*/