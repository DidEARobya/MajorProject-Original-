using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path_AStar
{
    Queue<INodeData> path = new Queue<INodeData>();

    public Path_AStar(WorldGrid world, Tile start, Tile end)
    {
        if(start == null || end == null)
        {
            return;
        }

        if(world.pathGraph == null)
        {
            world.pathGraph = new Path_TileGraph(world);
        }

        Node startNode = start.pathNode;
        Node endNode = end.pathNode;

        Node[,] nodes = world.pathGraph.nodes;

        List<Node> closedSet = new List<Node>();

        SimplePriorityQueue<Node> openSet = new SimplePriorityQueue<Node>();
        openSet.Enqueue(startNode, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        Dictionary<Node, float> gScore = new Dictionary<Node, float>();

        foreach(Node node in nodes)
        {
            gScore[node] = Mathf.Infinity;
        }

        Dictionary<Node, float> fScore = new Dictionary<Node, float>();

        foreach (Node node in nodes)
        {
            fScore[node] = Mathf.Infinity;
        }

        fScore[nodes[startNode.x, startNode.y]] = HeuristicCostEstimate(nodes[startNode.x, startNode.y], nodes[endNode.x, endNode.y]);

        while(openSet.Count > 0)
        {
            Node current = openSet.Dequeue();

            if(current == endNode)
            {
                ReconstructPath(cameFrom, current);
                return;
            }

            closedSet.Add(current);

            foreach(Edge neighbour in current.edges)
            {
                if(closedSet.Contains(neighbour.node) == true)
                {
                    continue;
                }

                float tentativeGScore = gScore[current] + DistanceBetween(current, neighbour.node, neighbour);

                if(openSet.Contains(neighbour.node) == true && tentativeGScore >= gScore[neighbour.node])
                {
                    continue;
                }

                cameFrom[neighbour.node] = current;
                gScore[neighbour.node] = tentativeGScore;
                fScore[neighbour.node] = gScore[neighbour.node] + HeuristicCostEstimate(neighbour.node, endNode);

                if(openSet.Contains(neighbour.node) == false)
                {
                    openSet.Enqueue(neighbour.node, fScore[neighbour.node]);
                }
            }
        }

        Debug.Log("No Path Found");
    }

    void ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        Queue<INodeData> totalPath = new Queue<INodeData>();
        totalPath.Enqueue(current.data);

        while(cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Enqueue(current.data);
        }

        path = new Queue<INodeData>(totalPath.Reverse());
    }
    float HeuristicCostEstimate(Node start, Node goal)
    {
        return Mathf.Sqrt(Mathf.Pow(goal.x - start.x, 2) + Mathf.Pow(goal.y - start.y, 2));
    }
    float DistanceBetween(Node start, Node goal, Edge edge)
    {
        int x1 = start.x;
        int y1 = start.y;

        int x2 = goal.x;
        int y2 = goal.y;

        float val = 0;

        if (Mathf.Abs(x2 - x1) + Mathf.Abs(y2 - y1) == 1)
        {
            val += 1;
        }
        else if (Mathf.Abs(x2 - x1) == 1 && Mathf.Abs(y2 - y1) == 1)
        {
            val += 1.41421356237f;
        }
        else
        {
            val += Mathf.Pow(goal.x - start.x, 2) + Mathf.Pow(goal.y - start.y, 2);
        }

        return val * edge.cost;
        //x1 == x2 + 1 || x1 == x2 - 1 && Mathf.Abs(x2 - x1) == 1 && Mathf.Abs(y2 - y1) == 1
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
