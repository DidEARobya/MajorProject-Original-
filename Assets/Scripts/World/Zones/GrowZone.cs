using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowZone
{
    public Color zoneColour = Color.green;
    HashSet<Tile> tiles = new HashSet<Tile>();

    public GrowZone()
    {
        zoneColour.a = 0.1f;
    }

    void UpdateZone()
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
    public void AddTile(Tile tile)
    {
        if(tiles.Contains(tile) || tile.IsAccessible() == Accessibility.IMPASSABLE)
        {
            return;
        }

        tiles.Add(tile);
        tile.zone = this;
        tile.UpdateVisual();
        //UpdateZone();
    }

    public void RemoveTile(Tile tile)
    {
        if (tiles.Contains(tile) == false)
        {
            return;
        }

        tiles.Remove(tile);
        tile.zone = null;
        tile.UpdateVisual();
    }
}
