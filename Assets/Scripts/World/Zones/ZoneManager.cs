using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public static class ZoneManager
{
    static HashSet<Zone> growZones = new HashSet<Zone>();
    //static HashSet<Zone> storageZones = new HashSet<Zone>();

    static Dictionary<ZoneType, HashSet<Zone>> zones = new Dictionary<ZoneType, HashSet<Zone>>();

    public static void Init()
    {
        zones.Add(ZoneType.GROW, growZones);
        //zones.Add(ZoneType.STORAGE, storageZones);
    }
    public static void AddTile(Tile tile, ZoneType type)
    {
        if(tile.zone != null && tile.zone.zoneType != type)
        {
            tile.zone.RemoveTile(tile);
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
                if (t.zone == null || t.zone == z)
                {
                    continue;
                }

                if(t.zone.tiles.Count > z.tiles.Count)
                {
                    MergeZones(t.zone, z);
                    break;
                }
                else
                {
                    MergeZones(z, t.zone);
                    break;
                }
            }
        }
        else
        {
            Zone zone = null;

            if (type == ZoneType.GROW)
            {
                zone = new GrowZone();
            }

            zone.AddTile(tile);
            zones[type].Add(zone);
        }
    }

    public static void RemoveTile(Tile tile)
    {
        if (tile.zone == null)
        {
            return;
        }

        HashSet<Tile> set = null;

        ZoneType type = tile.zone.zoneType;

        foreach (Zone zone in zones[type])
        {
            if (tile.zone == zone)
            {
                zone.RemoveTile(tile);
                set = zone.UpdateZone();
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

            foreach (Tile t in set)
            {
                newZone.AddTile(t);
            }

            zones[type].Add(newZone);
        }
    }
    public static void RemoveZone(Zone zone, ZoneType type)
    {
        if (zones[type].Contains(zone) == false)
        {
            return;
        }

        zones[type].Remove(zone);
    }
    static void MergeZones(Zone mergeInto, Zone toMerge)
    {
        foreach(Tile tile in toMerge.tiles)
        {
            mergeInto.AddTile(tile);
        }

        toMerge.DeleteZone();
    }
}
