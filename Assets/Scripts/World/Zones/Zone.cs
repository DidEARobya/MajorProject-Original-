using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ZoneType
{
    GROW,
    STORAGE
}
public class Zone
{
    public ZoneType zoneType;
    public Color zoneColour;
    public HashSet<Tile> tiles = new HashSet<Tile>();

    public HashSet<Tile> UpdateZone()
    {
        if(tiles.Count == 0)
        {
            return null;
        }

        Tile tile = tiles.First();

        Stack<Tile> stack = new Stack<Tile>();
        HashSet<Tile> beenChecked = new HashSet<Tile>();

        stack.Push(tile);

        while (stack.Count > 0)
        {
            Tile t = stack.Pop();

            beenChecked.Add(t);
            Dictionary<Tile, Direction> neighbours = t.GetNeighboursDict();

            foreach (Tile t2 in neighbours.Keys)
            {
                Direction dir = t.GetDirection(t2);

                if (dir == Direction.NE || dir == Direction.SE || dir == Direction.SW || dir == Direction.NW)
                {
                    continue;
                }

                if (t2 != null && beenChecked.Contains(t2) == false && t2.zone == this)
                {
                    stack.Push(t2);
                    continue;
                }
            }
        }

        if(beenChecked.Count != tiles.Count)
        {
            HashSet<Tile> newZone = new HashSet<Tile>();

            foreach (Tile t in tiles.ToList())
            {
                if(beenChecked.Contains(t) == true)
                {
                    continue;
                }

                tiles.Remove(t);
                newZone.Add(t);
            }

            return newZone;
        }

        return null;
    }
    public virtual void AddTile(Tile tile)
    {
        if (tiles.Contains(tile) || tile.IsAccessible() == Accessibility.IMPASSABLE)
        {
            return;
        }

        if(zoneType == ZoneType.GROW && tile.floorType != FloorType.NONE)
        {
            return;
        }

        tiles.Add(tile);
        tile.zone = this;
        tile.UpdateVisual();
    }

    public virtual void RemoveTile(Tile tile)
    {
        if (tiles.Contains(tile) == false)
        {
            return;
        }

        tiles.Remove(tile);
        tile.zone = null;

        if(tile.task != null)
        {
            tile.task.CancelTask(false);
        }

        tile.UpdateVisual();

        if(tiles.Count == 0)
        {
            GameManager.GetZoneManager().RemoveZone(this, zoneType);
        }
    }

    public virtual void DeleteZone()
    {
        foreach(Tile tile in tiles)
        {
            if (tile.zone == this)
            {
                tile.zone = null;
                tile.UpdateVisual();
            }
        }

        tiles.Clear();
        GameManager.GetZoneManager().RemoveZone(this, zoneType);
    }
    public virtual void UpdateZoneTasks()
    {

    }
}
