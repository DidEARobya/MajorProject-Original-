using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum InstalledObjectType
{
    WALL,
    DOOR
}
public class InstalledObject
{
    public Tile baseTile;
    public InstalledObjectTypes type;
    public GameObject gameObject;

    public bool isInstalled = false;

    Action<InstalledObject> updateObjectCallback;

    protected InstalledObject()
    {

    }
    static public InstalledObject CreatePrototype(InstalledObjectTypes _type)
    {
        InstalledObject obj = new InstalledObject();
        obj.type = _type;

        return obj;
    }

    static public InstalledObject PlaceObject(InstalledObjectTypes _type, Tile tile)
    {
        InstalledObject obj = new InstalledObject();
        obj.baseTile = tile;
        obj.type = _type;

        if (tile.InstallObject(obj) == false)
        {
            return null;
        }
        
        return obj;
    }

    public void Install()
    {
        isInstalled = true;
        baseTile.isPendingTask = false;
        baseTile.isPlayerWalkable = false;
        GameManager.GetWorldController().worldGrid.InvalidatePathGraph();

        if(updateObjectCallback != null)
        {
            updateObjectCallback(this);
        }
    }
    public void UnInstall()
    {
        GameManager.GetWorldController().worldGrid.InvalidatePathGraph();
        GameManager.GetInstalledSpriteController().Uninstall(this);
        baseTile.UninstallObject();
        MonoBehaviour.Destroy(gameObject);
    }
    public void AddOnUpdateCallback(Action<InstalledObject> callback)
    {
        updateObjectCallback += callback;
    }
    public void RemoveOnUpdateCallback(Action<InstalledObject> callback)
    {
        updateObjectCallback -= callback;
    }
}

public class InstalledObjectTypes
{
    protected readonly InstalledObjectType type;
    protected readonly int movementCost;

    protected readonly int width = 1;
    protected readonly int height = 1;

    public static readonly InstalledObjectTypes WALL = new InstalledObjectTypes(InstalledObjectType.WALL, 100);
    protected InstalledObjectTypes(InstalledObjectType _type, int _movementCost)
    {
        type = _type;
        movementCost = _movementCost;
    }

    public static InstalledObjectType GetObjectType(InstalledObjectTypes type)
    {
        return type.type;
    }
    public static int GetMovementCost(InstalledObjectTypes type)
    {
        return type.movementCost;
    }
}