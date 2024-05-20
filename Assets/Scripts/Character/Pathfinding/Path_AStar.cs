using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;

public class Path_AStar
{
    Queue<ITileData> path = new Queue<ITileData>();

    public Tile destination;
    bool isPlayer;

    public Path_AStar(Tile start, Tile end, bool _isPlayer)
    {
        if (start == null || end == null)
        {
            return;
        }

        isPlayer = _isPlayer;

        WorldGrid world = GameManager.GetWorldGrid();

        /*if (world.pathGraph == null)
        {
            if (end.accessibility == Accessibility.IMPASSABLE)
            {
                Accessibility temp = end.accessibility;
                end.accessibility = Accessibility.ACCESSIBLE;

                world.pathGraph = new Path_TileGraph(world);

                end.accessibility = temp;
            }
        }
        else
        {
            if (end.accessibility == Accessibility.IMPASSABLE)
            {
                Accessibility temp = end.accessibility;
                end.accessibility = Accessibility.ACCESSIBLE;

                foreach (Tile tile in end.GetNeighboursDict().Keys)
                {
                    tile.pathNode.neighbours = world.pathGraph.GetNeighbourNodes(tile.pathNode).ToArray();
                }

                end.accessibility = temp;
            }
        }*/

        if (world.pathGraph == null)
        {
            world.pathGraph = new Path_TileGraph(world);
        }

        Node startNode = start.pathNode;
        Node endNode = end.pathNode;

        HashSet<Node> closedSet = new HashSet<Node>();

        SimplePriorityQueue<Node> openSet = new SimplePriorityQueue<Node>();
        openSet.Enqueue(startNode, 0);

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();
            closedSet.Add(current);

            if ((current.data as ITileData).GetTile().IsNeighbour(end) == true)
            {
                path = RetraceTilePath(startNode, current);
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
                    neighbour.cameFrom = current;

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

        return;
    }
    Queue<ITileData> RetraceTilePath(Node start, Node end)
    {
        destination = (end.data as ITileData).GetTile();

        Queue<ITileData> totalPath = new Queue<ITileData>();
        Node current = end;

        while (current != start)
        {
            totalPath.Enqueue((ITileData)current.data);
            current = current.cameFrom;
        }

        return new Queue<ITileData>(totalPath.Reverse());
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