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
            node.neighbours = GetNeighbourNodes(node).ToArray();
        }
    }

    List<Node> GetNeighbourNodes(Node node)
    {
        List<Node> neighbours = new List<Node>();

        int length = nodes.GetLength(0);

        for(int x = -1; x <= 1;  x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.x + x;
                int checkY = node.y + y;

                if(checkX >= 0 && checkX < length && checkY >= 0 && checkY < length)
                {
                    if(nodes[checkX, checkY].data.GetTile().IsAccessible() != Accessibility.IMPASSABLE && isClippingCorner(node, nodes[checkX, checkY]) == false)
                    {
                        neighbours.Add(nodes[checkX, checkY]);
                    }
                }
            }
        }

        return neighbours;
    }

    bool isClippingCorner(Node current, Node neighbour)
    {
        int dirX = current.x - neighbour.x;
        int dirY = current.y - neighbour.y;

        if(Mathf.Abs(dirX) + Mathf.Abs(dirY) == 2)
        {
            if(nodes[current.x - dirX, current.y].data.GetTile().IsAccessible() == Accessibility.IMPASSABLE)
            {
                return true;
            }

            if (nodes[current.x, current.y - dirY].data.GetTile().IsAccessible() == Accessibility.IMPASSABLE)
            {
                return true;
            }
        }

        return false;
    }
}
public class Node
{
    public int x;
    public int y;

    public float gCost;
    public float hCost;
    public float fCost
    {
        get { return gCost + hCost; }
    }

    public INodeData data;
    public Node[] neighbours;

    public Node(INodeData _data, int _x, int _y)
    {
        data = _data;
        x = _x;
        y = _y;

        data.SetNode(this);
    }
    public int GetCost(bool isPlayer)
    {
        return Mathf.FloorToInt(data.GetCost(isPlayer));
    }
}
