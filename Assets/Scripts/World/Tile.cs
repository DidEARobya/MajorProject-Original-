using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

[JsonConverter(typeof(StringEnumConverter))]
public enum TerrainType
{
    [EnumMember(Value = "GOOD_SOIL")]
    GOOD_SOIL,
    [EnumMember(Value = "POOR_SOIL")]
    POOR_SOIL,
}
[JsonConverter(typeof(StringEnumConverter))]
public enum FloorType
{
    NONE,
    TASK_FLOOR,
    [EnumMember(Value = "WOOD_FLOOR")]
    WOOD_FLOOR,
    [EnumMember(Value = "STONE_FLOOR")]
    STONE_FLOOR
}
[JsonConverter(typeof(StringEnumConverter))]
public enum Accessibility
{
    [EnumMember(Value = "IMPASSABLE")]
    IMPASSABLE,
    [EnumMember(Value = "DELAYED")]
    DELAYED,
    [EnumMember(Value = "ACCESSIBLE")]
    ACCESSIBLE
}
public enum Direction
{
    N,
    NE,
    E,
    SE,
    S,
    SW,
    W,
    NW
}
public class Tile : InventoryOwner, INodeData
{
    Dictionary<Tile, Direction> neighbourTiles = new Dictionary<Tile, Direction>();
    Dictionary<Direction, Tile> neighbourDirections = new Dictionary<Direction, Tile>();

    public GameObject tileObj;
    public Node pathNode;

    public TerrainType terrainType = TerrainType.GOOD_SOIL;
    public FloorType floorType = FloorType.NONE;

    public InstalledObject installedObject
    {
        get;
        private set;
    }
    public Inventory inventory;

    public WorldGrid world;
    public Region region;

    public new int x;
    public new int y;

    Action<Tile> tileChangedCallback;

    public Accessibility accessibility = Accessibility.ACCESSIBLE;

    public Task task;
    public TaskSite site;
    public CharacterController reservedBy;

    public bool isPendingTask = false;

    public Zone zone = null;
    public GameObject zoneObj = null;

    public bool isSelected = false;
    public GameObject selectedObj = null;

    public Color regionColour;
    public bool displayRegion = false;
    public GameObject regionDisplayObj = null;

    public GameObject taskDisplayObject = null;
    public Tile(WorldGrid grid, int _x, int _y, float noiseVal = 0) : base (InventoryOwnerType.TILE)
    {
        world = grid;

        x = _x;
        y = _y;

        SetTerrainType(ThingsDataHandler.GetRandomTerrainType(noiseVal));

        InventoryManager.CreateNewInventory(InventoryOwnerType.TILE, this);
    }
    public void SetNeighbours()
    {
        int length = world.mapSize;

        for (int _x = -1; _x <= 1; _x++)
        {
            for (int _y = -1; _y <= 1; _y++)
            {
                if (_x == 0 && _y == 0)
                {
                    continue;
                }

                int checkX = x + _x;
                int checkY = y + _y;

                if (checkX >= 0 && checkX < length && checkY >= 0 && checkY < length)
                {
                    if (world.GetTile(checkX, checkY) != null)
                    {
                        neighbourTiles.Add(world.GetTile(checkX, checkY), GetDirection(_x, _y));
                        neighbourDirections.Add(neighbourTiles[world.GetTile(checkX, checkY)], world.GetTile(checkX, checkY));
                    }
                }
            }
        }
    }
    public void SetGameObject(GameObject obj)
    {
        if(obj == null)
        {
            return;
        }

        tileObj = obj;
    }
    public void SetTerrainType(TerrainType terrain)
    {
        if(terrainType == terrain)
        {
            return;
        }

        terrainType = terrain;

        UpdateVisual();
    }
    public void SetFloorType(FloorType floor)
    {
        if (floorType == floor)
        {
            return;
        }

        FloorType oldType = floorType;
        FloorType newType = floor;

        if (oldType != FloorType.NONE && newType != FloorType.TASK_FLOOR && oldType != FloorType.TASK_FLOOR)
        {
            InventoryManager.AddToTileInventory(this, ThingsDataHandler.GetFloorData(oldType).GetRequirements());
        }

        if (accessibility == Accessibility.IMPASSABLE && zone != null && zone.zoneType == ZoneType.GROW)
        {
            zone.RemoveTile(this);
        }

        floorType = floor;

        UpdateVisual();
    }
    public void SetSelected(bool selected)
    {
        if(isSelected == selected)
        {
            return;
        }

        isSelected = selected;
        UpdateVisual();
    }
    public void UpdateVisual()
    {
        if (tileChangedCallback != null)
        {
            tileChangedCallback(this);
        }
    }
    public bool PlaceObject(InstalledObject obj)
    {
        if (obj == null)
        {
            installedObject = null;
            return false;
        }

        if (installedObject != null)
        {
            Debug.Log("Installed Object Exists");
            return false;
        }

        installedObject = obj;
        return true;
    }
    public InstalledObject GetInstalledObject()
    {
        return installedObject;
    }
    public Building GetBuilding()
    {
        return installedObject as Building;
    }
    public void InstallObject()
    {
        if (installedObject == null)
        {
            return;
        }

        accessibility = installedObject.GetAccessibility();

        if (accessibility == Accessibility.IMPASSABLE && zone != null)
        {
            zone.RemoveTile(this);
        }
    }
    public void UninstallObject()
    {
        if(installedObject == null)
        {
            return;
        }

        installedObject.UnInstall();
    }
    public void ClearInstalledObject()
    {
        installedObject = null;
        accessibility = Accessibility.ACCESSIBLE;
    }
    public int GetCost(bool isPlayer)
    {
        if(isPlayer == true && accessibility == Accessibility.IMPASSABLE)
        {
            return 0;
        }

        int cost = 0;

        if(floorType == FloorType.NONE)
        {
            cost += ThingsDataHandler.GetTerrainData(terrainType).movementCost;
        }
        else
        {
            cost += ThingsDataHandler.GetFloorData(floorType).movementCost;
        }

        if(installedObject != null && installedObject.isInstalled)
        {
            cost += installedObject.GetMovementCost();
        }

        if(isPendingTask == true)
        {
            cost += 100;
        }

        return cost;
    }
    public bool IsObjectInstalled()
    {
        if(installedObject == null)
        {
            return false;
        }
        if(installedObject.isInstalled == false)
        {
            return false;
        }

        return true;
    }
    public Tile GetTile()
    {
        return this;
    }
    public void SetNode(Node node)
    {
        pathNode = node;
    }
    public Accessibility IsAccessible()
    {
        if(accessibility == Accessibility.IMPASSABLE)
        {
            return Accessibility.IMPASSABLE;
        }

        if(installedObject != null && accessibility == Accessibility.DELAYED)
        {
            return InstalledObjectAction.CheckIfOpen(installedObject);
        }

        return Accessibility.ACCESSIBLE;
    }
    public Tile GetNearestAvailableInventory(ItemData type, int amount, bool acceptSelf)
    {
        if(acceptSelf == true && accessibility != Accessibility.IMPASSABLE && (inventory.item == null || inventory.CanBeStored(type, amount) != 0) && installedObject == null)
        {
             return this;      
        }

        foreach(Tile tile in neighbourTiles.Keys)
        {
            if(tile.accessibility != Accessibility.IMPASSABLE && (tile.inventory.item == null || tile.inventory.CanBeStored(type, amount) != 0) && tile.installedObject == null)
            {
                return tile;
            }
        }

        foreach (Tile tile in neighbourTiles.Keys)
        {
            foreach(Tile _tile in tile.neighbourTiles.Keys)
            {
                if (_tile.accessibility != Accessibility.IMPASSABLE && (_tile.inventory.item == null || _tile.inventory.CanBeStored(type, amount) != 0) && _tile.installedObject == null)
                {
                    return _tile;
                }
            }
        }

        return null;
    }
    public Tile GetNearestAvailableTile()
    {
        foreach (Tile tile in neighbourTiles.Keys)
        {
            if (tile.accessibility != Accessibility.IMPASSABLE)
            {
                return tile;
            }
        }

        return null;
    }
    public List<Tile> GetAdjacentNeigbours()
    {
        List<Tile> adjacent = new List<Tile>();

        if (neighbourDirections.ContainsKey(Direction.N))
        {
            adjacent.Add(neighbourDirections[Direction.N]);
        }
        if (neighbourDirections.ContainsKey(Direction.E))
        {
            adjacent.Add(neighbourDirections[Direction.E]);
        }
        if (neighbourDirections.ContainsKey(Direction.S))
        {
            adjacent.Add(neighbourDirections[Direction.S]);
        }
        if (neighbourDirections.ContainsKey(Direction.W))
        {
            adjacent.Add(neighbourDirections[Direction.W]);
        }

        return adjacent;
    }
    public bool IsNeighbour(Tile tile)
    {
        return neighbourTiles.ContainsKey(tile);
    }
    public bool IsAdjacent(Tile tile)
    {
        if(tile == North || tile == East || tile == South || tile == West)
        {
            return true;
        }

        return false;
    }
    public Tile GetTileByDirection(Direction dir)
    {
        if (neighbourDirections.ContainsKey(dir) == false)
        {
            return null;
        }

        return neighbourDirections[dir];
    }
    public Direction GetDirection(Tile tile)
    {
        if(neighbourTiles.ContainsKey(tile) == false)
        {
            return 0;
        }

        return neighbourTiles[tile];
    }
    public Direction GetDirection(int x, int y)
    {
        if (x == -1)
        {
            switch (y)
            {
                case -1:
                    return Direction.SW;

                case 0:
                    return  Direction.W;

                case 1:
                    return Direction.NW;
            }
        }
        else if (x == 0)
        {
            switch (y)
            {
                case -1:
                    return Direction.S;

                case 1:
                    return Direction.N;
            }
        }
        else
        {
            switch (y)
            {
                case -1:
                    return Direction.SE;

                case 0:
                    return Direction.E;

                case 1:
                    return Direction.NE;
            }
        }

        return 0;
    }
    public Dictionary<Tile, Direction> GetNeighboursDict()
    {
        return neighbourTiles;
    }
    public Tile North
    {
        get
        {
            if (neighbourDirections.ContainsKey(Direction.N) == false)
            {
                return null;
            }

            return neighbourDirections[Direction.N];
        }
    }
    public Tile East
    {
        get
        {
            if (neighbourDirections.ContainsKey(Direction.E) == false)
            {
                return null;
            }

            return neighbourDirections[Direction.E];
        }
    }
    public Tile South
    {
        get
        {
            if (neighbourDirections.ContainsKey(Direction.S) == false)
            {
                return null;
            }

            return neighbourDirections[Direction.S];
        }
    }
    public Tile West
    {
        get
        {
            if (neighbourDirections.ContainsKey(Direction.W) == false)
            {
                return null;
            }

            return neighbourDirections[Direction.W];
        }
    }
    public void SetTileChangedCallback(Action<Tile> callback)
    {
        tileChangedCallback += callback;
    }
    public void RemoveTileChangedCallback(Action<Tile> callback)
    {
        tileChangedCallback -= callback;
    }
}