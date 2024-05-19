using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public enum TerrainType
{
    GRASS,
    GOOD_SOIL,
    POOR_SOIL,
}
public enum FloorType
{
    NONE,
    TASK_FLOOR,
    WOOD_FLOOR,
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
public enum Accessibility
{
    IMPASSABLE,
    DELAYED,
    ACCESSIBLE
}
public class Tile : InventoryOwner, ITileData
{
    Dictionary<Tile, Direction> neighbourTiles = new Dictionary<Tile, Direction>();
    Dictionary<Direction, Tile> neighbourDirections = new Dictionary<Direction, Tile>();

    public GameObject tileObj;
    public Node pathNode;

    public TerrainTypes terrainType = TerrainTypes.GOOD_SOIL;
    public FloorTypes floorType = FloorTypes.NONE;

    public InstalledObject installedObject;

    public Inventory inventory;

    public WorldGrid world;
    public Region region;

    public new int x;
    public new int y;

    Action<Tile> tileChangedCallback;

    public Accessibility accessibility = Accessibility.ACCESSIBLE;

    public Task task;

    public bool isPendingTask = false;
    public CharacterController reservedBy = null;

    public Zone zone = null;
    public GameObject zoneObj = null;

    public bool isSelected = false;
    public GameObject selectedObj = null;

    public Tile(WorldGrid grid, int _x, int _y, float noiseVal = 0) : base (InventoryOwnerType.TILE)
    {
        world = grid;

        x = _x;
        y = _y;

        if(noiseVal < 0)
        {
            SetTerrainType(TerrainTypes.GOOD_SOIL);
        }
        else
        {
            SetTerrainType(TerrainTypes.POOR_SOIL);
        }

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
    public void SetTerrainType(TerrainTypes terrain)
    {
        if(terrainType == terrain)
        {
            return;
        }

        terrainType = terrain;

        UpdateVisual();
    }
    public void SetFloorType(FloorTypes floor)
    {
        if (floorType == floor)
        {
            return;
        }

        FloorType oldType = FloorTypes.GetFloorType(floorType);
        FloorType newType = FloorTypes.GetFloorType(floor);

        if (oldType != FloorType.NONE || oldType != FloorType.TASK_FLOOR)
        {
            if(newType != FloorType.TASK_FLOOR)
            {
                InventoryManager.AddToTileInventory(this, FloorTypes.GetRequirements(floorType));
            }
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
    public bool InstallObject(InstalledObject obj)
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
    public void UninstallObject()
    {
        if(installedObject == null)
        {
            return;
        }

        installedObject.UnInstall();

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

        if(floorType == FloorTypes.NONE)
        {
            cost += TerrainTypes.GetMovementCost(terrainType);
        }
        else
        {
            cost += FloorTypes.GetMovementCost(floorType);
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
    public Tile GetNearestAvailableInventory(ItemTypes type, int amount)
    {
        if(inventory.item == null || inventory.CanBeStored(type, amount) != 0)
        {
            return this;
        }
        
        foreach(Tile tile in neighbourTiles.Keys)
        {
            if(tile.inventory.item == null || tile.inventory.CanBeStored(type, amount) != 0)
            {
                return tile;
            }
        }

        foreach (Tile tile in neighbourTiles.Keys)
        {
            foreach(Tile _tile in tile.neighbourTiles.Keys)
            {
                if (_tile.inventory.item == null || _tile.inventory.CanBeStored(type, amount) != 0)
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
    public bool IsNeighbour(Tile tile)
    {
        return neighbourTiles.ContainsKey(tile);
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

public class TerrainTypes
{
    protected readonly TerrainType type;
    protected readonly int movementCost;
    protected readonly int plantGrowthChance;
    protected readonly float fertilityMultiplier;

    public static readonly TerrainTypes GOOD_SOIL = new TerrainTypes(TerrainType.GOOD_SOIL, 2, 10, 1.1f);
    public static readonly TerrainTypes POOR_SOIL = new TerrainTypes(TerrainType.POOR_SOIL, 1, 2, 0.9f);

    protected TerrainTypes(TerrainType _type, int _movementCost, int growthChance, float _fertilityMultiplier)
    {
        type = _type;
        movementCost = _movementCost;
        plantGrowthChance = growthChance;
        fertilityMultiplier = _fertilityMultiplier;
    }

    public static TerrainType GetTerrainType(TerrainTypes type) 
    {
        return type.type;
    }
    public static int GetMovementCost(TerrainTypes type)
    {
        return type.movementCost;
    }
    public static int GetGrowthChance(TerrainTypes type)
    {
        return type.plantGrowthChance;
    }
    public static float GetFertilityMultiplier(TerrainTypes type)
    {
        return type.fertilityMultiplier;
    }
}
public class FloorTypes
{
    protected readonly FloorType type;
    protected readonly int movementCost;

    protected readonly FloorRequirements requirements;

    public static readonly FloorTypes NONE = new FloorTypes(FloorType.NONE, 0, null);
    public static readonly FloorTypes TASK = new FloorTypes(FloorType.TASK_FLOOR, 0, null);
    public static readonly FloorTypes WOOD = new FloorTypes(FloorType.WOOD_FLOOR, 1, FloorRequirements.WOOD);

    protected FloorTypes(FloorType _type, int _movementCost, FloorRequirements _requirements)
    {
        type = _type;
        movementCost = _movementCost;
        requirements = _requirements;
    }

    public static FloorType GetFloorType(FloorTypes type)
    {
        return type.type;
    }
    public static int GetMovementCost(FloorTypes type)
    {
        return type.movementCost;
    }
    public static Dictionary<ItemTypes, int> GetRequirements(FloorTypes type)
    {
        if(type.requirements == null)
        {
            return null;
        }

        return FloorRequirements.GetRequirements(type.requirements);
    }
}