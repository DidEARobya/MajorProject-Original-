using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorRegion : Region
{
    public DoorRegion(Cluster _inCluster) : base(_inCluster)
    {

    }
    protected override void FindEdges(Tile tile)
    {
        borderTiles.Add(tile);
        GameManager.GetRegionManager().regions.Add(this);

        if(tile.North != null && tile.North.region != null)
        {
            neighbours.Add(tile.North.region);
            tile.North.region.neighbours.Add(this);
        }
        if (tile.East != null && tile.East.region != null)
        {
            neighbours.Add(tile.East.region);
            tile.East.region.neighbours.Add(this);
        }
        if (tile.South != null && tile.South.region != null)
        {
            neighbours.Add(tile.South.region);
            tile.South.region.neighbours.Add(this);
        }
        if (tile.West != null && tile.West.region != null)
        {
            neighbours.Add(tile.West.region);
            tile.West.region.neighbours.Add(this);
        }
    }
}
