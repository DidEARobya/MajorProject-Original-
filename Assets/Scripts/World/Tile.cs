using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum TerrainType
{
    GOOD_SOIL,
    POOR_SOIL,
}
public enum FloorType
{
    NONE,
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
public class Tile : InventoryOwner, INodeData
{
    Dictionary<Tile, Direction> neighbours = new Dictionary<Tile, Direction>();

    public GameObject tileObj;
    public Node pathNode;

    public TerrainTypes terrainType = TerrainTypes.GOOD_SOIL;
    public FloorTypes floorType = FloorTypes.NONE;

    public InstalledObject installedObject;

    public Inventory inventory;

    public WorldGrid world;

    public new int x;
    public new int y;

    Action<Tile> tileChangedCallback;

    public Accessibility accessibility = Accessibility.ACCESSIBLE;

    public bool isPendingTask = false;
    public bool reservedByCharacter = false;

    public Tile(WorldGrid grid, int _x, int _y) : base (InventoryOwnerType.TILE)
    {
        world = grid;

        x = _x;
        y = _y;

        InventoryManager.CreateNewInventory(InventoryOwnerType.TILE, this);
    }

    public void SetNeighbours()
    {
        int length = world.mapHeight;

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
                        neighbours.Add(world.GetTile(checkX, checkY), GetDirection(_x, _y));
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

        if(tileChangedCallback != null)
        {
            tileChangedCallback(this);
        }
    }
    public void SetFloorType(FloorTypes floor)
    {
        if (floorType == floor)
        {
            return;
        }

        floorType = floor;

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
            cost += InstalledObjectTypes.GetMovementCost(installedObject.type);
        }

        return cost;
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
    public bool IsNeighbour(Tile tile)
    {
        return neighbours.ContainsKey(tile);
    }
    public Direction GetDirection(Tile tile)
    {
        if(neighbours.ContainsKey(tile) == false)
        {
            return 0;
        }

        return neighbours[tile];
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

    public static readonly TerrainTypes GOOD_SOIL = new TerrainTypes(TerrainType.GOOD_SOIL, 2);
    public static readonly TerrainTypes POOR_SOIL = new TerrainTypes(TerrainType.POOR_SOIL, 1);

    protected TerrainTypes(TerrainType _type, int _movementCost)
    {
        type = _type;
        movementCost = _movementCost;
    }

    public static TerrainType GetTerrainType(TerrainTypes type) 
    {
        return type.type;
    }
    public static int GetMovementCost(TerrainTypes type)
    {
        return type.movementCost;
    }
}
public class FloorTypes
{
    protected readonly FloorType type;
    protected readonly int movementCost;

    public static readonly FloorTypes NONE = new FloorTypes(FloorType.NONE, 0);
    public static readonly FloorTypes WOOD = new FloorTypes(FloorType.WOOD_FLOOR, 1);

    protected FloorTypes(FloorType _type, int _movementCost)
    {
        type = _type;
        movementCost = _movementCost;
    }

    public static FloorType GetFloorType(FloorTypes type)
    {
        return type.type;
    }
    public static int GetMovementCost(FloorTypes type)
    {
        return type.movementCost;
    }
}