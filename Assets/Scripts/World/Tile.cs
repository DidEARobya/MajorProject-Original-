using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

public class Tile : INodeData
{
    public GameObject tileObj;
    public Node pathNode;

    public TerrainTypes terrainType = TerrainTypes.GOOD_SOIL;
    public FloorTypes floorType = FloorTypes.NONE;

    InstalledObject installedObject;
    DroppedObject droppedObject;

    public WorldGrid world;

    public int x;
    public int y;

    Action<Tile> tileChangedCallback;

    public bool isPendingTask = false;

    public Tile(WorldGrid grid, int _x, int _y)
    {
        world = grid;

        x = _x;
        y = _y;
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
    public int GetCost()
    {
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
    public bool IsNeighbour(Tile tile, bool includeDiagonals = false)
    {
        int x1 = this.x;
        int y1 = this.y;

        int x2 = tile.x;
        int y2 = tile.y;

        if(Mathf.Abs(x2 - x1) + Mathf.Abs(y2 - y1) == 1)
        {
            return true;
        }

        if(includeDiagonals == true)
        {
            if (x1 == x2 + 1 || x1 == x2 - 1 && Mathf.Abs(x2 - x1) == 1 && Mathf.Abs(y2 - y1) == 1)
            {
                return true;
            }
        }

        return false;
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