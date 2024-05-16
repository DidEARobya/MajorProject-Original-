using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowZone : Zone
{
    public GrowZone()
    {
        zoneType = ZoneType.GROW; 
        zoneColour.a = 0.1f;
    }

    protected override void UpdateZoneTasks()
    {
        foreach(Tile tile in tiles)
        {
            if(tile.installedObject == null && tile.isPendingTask == false)
            {
                Task task = new TendTask(tile, (t) => { ObjectManager.SpawnPlant(PlantTypes.OAK_TREE, tile, PlantState.SEED); }, TaskType.AGRICULTURE, false, 5f);
                TaskManager.AddTask(task, TaskType.AGRICULTURE);
            }
        }
    }
}
