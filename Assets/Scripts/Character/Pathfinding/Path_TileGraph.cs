using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_TileGraph
{
    public Node[,] nodes;
    public Path_TileGraph(WorldGrid grid)
    {
        nodes = new Node[grid.mapWidth, grid.mapHeight];

        for(int x = 0; x < grid.mapWidth; x++)
        {
            for (int y = 0; y < grid.mapHeight; y++)
            {
                nodes[x, y] = new Node(grid.GetTile(x, y), x, y);
            }
        }

        foreach (Node node in nodes)
        {
            List<Node> neighbours = GetNeighbourNodes(node);
            List<Edge> edges = new List<Edge>();

            for(int i = 0; i < neighbours.Count; i++)
            {
                Edge edge = new Edge(neighbours[i], neighbours[i].data.GetCost());
                edges.Add(edge);
            }

            node.edges = edges.ToArray();
        }
    }

    
    List<Node> GetNeighbourNodes(Node node)
    {
        List<Node> neighbours = new List<Node>();

        int x = node.x;
        int y = node.y;

        int length = nodes.GetLength(0);

        if((x + 1 < length) && nodes[x + 1, y] != null)
        {
            neighbours.Add(nodes[x + 1, y]);
        }

        if ((x + 1 < length) && (y + 1 < length) && nodes[x + 1, y + 1] != null)
        {
            neighbours.Add(nodes[x + 1, y + 1]);
        }

        if ((y + 1 < length) && nodes[x, y + 1] != null)
        {
            neighbours.Add(nodes[x, y + 1]);
        }

        if ((x - 1 >= 0) && (y + 1 < length) && nodes[x - 1, y + 1] != null)
        {
            neighbours.Add(nodes[x - 1, y + 1]);
        }

        if ((x - 1 >= 0) && nodes[x - 1, y] != null)
        {
            neighbours.Add(nodes[x - 1, y]);
        }

        if ((x - 1 >= 0) && (y - 1 >= 0) && nodes[x - 1, y - 1] != null)
        {
            neighbours.Add(nodes[x - 1, y - 1]);
        }

        if ((y - 1 >= 0) && nodes[x, y - 1] != null)
        {
            neighbours.Add(nodes[x, y - 1]);
        }

        if ((x + 1 < length) && (y - 1 >= 0) && nodes[x + 1, y - 1] != null)
        {
            neighbours.Add(nodes[x + 1, y - 1]);
        }

        return neighbours;
    }
}
public class Node
{
    public int x;
    public int y;

    public INodeData data;
    public Edge[] edges;

    public Node(INodeData _data, int _x, int _y)
    {
        data = _data;
        x = _x;
        y = _y;

        data.SetNode(this);
    }
}
public class Edge
{
    public Node node;
    public float cost;

    public Edge(Node _node,  float _cost)
    {
        node = _node;
        cost = _cost;
    }
}
