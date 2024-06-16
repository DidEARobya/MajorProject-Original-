using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorRegion : Region
{
    public DoorRegion(Cluster _inCluster) : base(_inCluster)
    {
        isDoor = true;
    }
    protected override void FindEdges(Tile tile)
    {
        GameManager.GetRegionManager().regions.Add(this);
    }
    public override void SetNeighbours()
    {
        Tile tile = tiles.First();

        foreach (Tile t in tile.GetAdjacentNeigbours())
        {
            if(t.region == null)
            {
                continue;
            }

            neighbours.Add(t.region);
            t.region.neighbours.Add(this);
        }
    }
}
