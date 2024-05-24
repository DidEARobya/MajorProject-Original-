using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class ZoneManager
{
    HashSet<Zone> growZones = new HashSet<Zone>();
    HashSet<Zone> storageZones = new HashSet<Zone>();

    Dictionary<ZoneType, HashSet<Zone>> zones = new Dictionary<ZoneType, HashSet<Zone>>();

    public void Init()
    {
        zones.Add(ZoneType.GROW, growZones);
        zones.Add(ZoneType.STORAGE, storageZones);
    }
    public void AddTile(Tile tile, ZoneType type)
    {
        if(tile.zone != null && tile.zone.zoneType != type)
        {
            return;
        }

        Zone z = null;

        foreach (Tile t in tile.GetNeighboursDict().Keys)
        {
            Direction dir = tile.GetDirection(t);

            if(dir == Direction.NE || dir == Direction.SE || dir == Direction.SW || dir == Direction.NW)
            {
                continue;
            }

            if (t.zone != null && zones[type].Contains(t.zone) == true)
            {
                z = t.zone;
                z.AddTile(tile);
                break;
            }
        }

        if(z != null)
        {
            foreach (Tile t in tile.GetNeighboursDict().Keys)
            {
                if (t.zone == null || t.zone == z || t.zone.zoneType != z.zoneType)
                {
                    continue;
                }

                if(t.zone.tiles.Count > z.tiles.Count)
                {
                    MergeZones(t.zone, z);
                    return;
                }
                else
                {
                    MergeZones(z, t.zone);
                    return;
                }
            }

            z.UpdateZoneTasks();
        }
        else
        {
            Zone zone = null;

            if (type == ZoneType.GROW)
            {
                zone = new GrowZone();
            }
            else if(type == ZoneType.STORAGE)
            {
                zone = new StorageZone();
            }

            zone.AddTile(tile);
            zones[type].Add(zone);
            zone.UpdateZoneTasks();
        }
    }

    public void RemoveTile(Tile tile, ZoneType toRemove)
    {
        if (tile.zone == null || tile.zone.zoneType != toRemove)
        {
            return;
        }

        HashSet<Tile> set = null;

        ZoneType type = toRemove;

        foreach (Zone zone in zones[type])
        {
            if (tile.zone == zone)
            {
                zone.RemoveTile(tile);
                set = zone.UpdateZone();
                zone.UpdateZoneTasks();
                break;
            }
        }

        if (set != null)
        {
            Zone newZone = null;

            if (type == ZoneType.GROW)
            {
                newZone = new GrowZone();
            }
            else if (type == ZoneType.STORAGE)
            {
                newZone = new StorageZone();
            }

            foreach (Tile t in set)
            {
                newZone.AddTile(t);
            }

            zones[type].Add(newZone);

            newZone.UpdateZoneTasks();
        }
    }
    public void RemoveZone(Zone zone, ZoneType type)
    {
        if (zones[type].Contains(zone) == false)
        {
            return;
        }

        zones[type].Remove(zone);
    }
    void MergeZones(Zone mergeInto, Zone toMerge)
    {
        foreach(Tile tile in toMerge.tiles)
        {
            mergeInto.AddTile(tile);
        }

        toMerge.DeleteZone();
        mergeInto.UpdateZoneTasks();
    }
}
