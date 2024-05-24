using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowZone : Zone
{
    PlantTypes toGrow;

    public GrowZone()
    {
        zoneType = ZoneType.GROW; 
        zoneColour = Color.green;
        zoneColour.a = 0.1f;
    }

    public void SetToGrow(PlantTypes type)
    {
        if(type == null || type == toGrow)
        {
            return;
        }

        toGrow = type;
        UpdateZoneTasks();
    }
    public override void UpdateZoneTasks()
    {
        if(toGrow == null)
        {
            return;
        }

        foreach(Tile tile in tiles)
        {
            if(tile.installedObject == null && tile.isPendingTask == false)
            {
                Task task = new TendTask(tile, (t) => { ObjectManager.SpawnPlant(PlantTypes.OAK_TREE, tile, PlantState.SEED); }, TaskType.AGRICULTURE, false, 5f);
                GameManager.GetTaskManager().AddTask(task, TaskType.AGRICULTURE);
            }
            else
            {
                if(tile.isPendingTask == false && tile.installedObject.type == InstalledObjectType.PLANT && (tile.installedObject as Plant).plantType != toGrow)
                {
                    Task task = new DestroyTask(tile, (t) => { tile.UninstallObject(); }, TaskType.AGRICULTURE, false, tile.installedObject.durability);
                    GameManager.GetTaskManager().AddTask(task, TaskType.AGRICULTURE);
                }
                else if(tile.isPendingTask == false && tile.installedObject.type == InstalledObjectType.PLANT && (tile.installedObject as Plant).plantState == PlantState.GROWN)
                {
                    Task task = new DestroyTask(tile, (t) => { tile.UninstallObject(); }, TaskType.AGRICULTURE, false, tile.installedObject.durability);
                    GameManager.GetTaskManager().AddTask(task, TaskType.AGRICULTURE);
                }
            }
        }
    }
}
