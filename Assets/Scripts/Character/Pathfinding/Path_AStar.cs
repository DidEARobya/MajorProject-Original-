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
    Queue<INodeData> path = new Queue<INodeData>();

    public Tile destination;
    bool isPlayer;

    public Path_AStar()
    {

    }
    public bool TilePathfind(Tile start, Tile end, bool _isPlayer, bool acceptNeighbours)
    {
        if (start == null || end == null)
        {
            return false;
        }

        isPlayer = _isPlayer;

        WorldGrid world = GameManager.GetWorldGrid();

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

            if(acceptNeighbours == false)
            {
                if (current.data.GetTile() == end)
                {
                    path = RetraceTilePath(startNode, current);
                    return true;
                }
            }
            else
            {
                if (current.data.GetTile().IsNeighbour(end) == true)
                {
                    path = RetraceTilePath(startNode, current);
                    return true;
                }
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

        return false;
    }
    public bool IsRegionPathable(Region start, Region end, Tile target, bool _isPlayer)
    {
        if (start == null || (end == null && target == null))
        {
            return false;
        }

        isPlayer = _isPlayer;

        HashSet<Region> closedSet = new HashSet<Region>();

        SimplePriorityQueue<Region> openSet = new SimplePriorityQueue<Region>();
        openSet.Enqueue(start, 0);

        while (openSet.Count > 0)
        {
            Region current = openSet.Dequeue();
            closedSet.Add(current);

            if (current == end || current.searchTiles.Contains(target))
            {
                return true;
            }

            foreach (Region neighbour in current.GetNeighbours())
            {
                if (closedSet.Contains(neighbour) == true)
                {
                    continue;
                }

                float tentativeGScore = 0;

                if (tentativeGScore < neighbour.gCost || openSet.Contains(neighbour) == false)
                {
                    neighbour.gCost = tentativeGScore;
                    neighbour.hCost = 0;

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

        return false;
    }
    Queue<INodeData> RetraceTilePath(Node start, Node end)
    {
        destination = end.data.GetTile();

        Queue<INodeData> totalPath = new Queue<INodeData>();
        Node current = end;

        while (current != start)
        {
            totalPath.Enqueue(current.data);
            current = current.cameFrom;
        }

        return new Queue<INodeData>(totalPath.Reverse());
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