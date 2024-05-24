using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageZone : Zone
{
    public StorageZone()
    {
        zoneType = ZoneType.STORAGE;
        zoneColour = Color.yellow;
        zoneColour.a = 0.1f;
    }
    public override void AddTile(Tile tile)
    {
        if (tiles.Contains(tile) || tile.IsAccessible() == Accessibility.IMPASSABLE)
        {
            return;
        }

        StorageManager.AddTile(tile);

        base.AddTile(tile);
    }

    public override void RemoveTile(Tile tile)
    {
        if (tiles.Contains(tile) == false)
        {
            return;
        }

        StorageManager.RemoveTile(tile);
        base.RemoveTile(tile);
    }

    public override void DeleteZone()
    {
        foreach (Tile tile in tiles)
        {
            if (tile.zone == this)
            {
                tile.zone = null;
                StorageManager.RemoveTile(tile);
                tile.UpdateVisual();
            }
        }

        tiles.Clear();
        GameManager.GetZoneManager().RemoveZone(this, zoneType);
    }
}
