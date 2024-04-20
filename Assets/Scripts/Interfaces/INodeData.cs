using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INodeData
{
    public Tile GetTile();
    public int GetCost(bool isPlayer);
    public void SetNode(Node node);
}
