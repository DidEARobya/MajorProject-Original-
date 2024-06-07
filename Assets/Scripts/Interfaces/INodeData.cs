using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INodeData
{
    public Accessibility IsAccessible();
    public int GetCost(bool isPlayer);
    public void SetNode(Node node);
    public Tile GetTile();
}