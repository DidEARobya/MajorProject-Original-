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

    public Action<InstalledObject, float> updateActionCallback;

    public float openVal = 0.0f;
    public bool isOpening = false;

    public bool isInstalled = false;

    Action<InstalledObject> updateObjectCallback;

    protected InstalledObject()
    {

    }
    static public InstalledObject PlaceObject(InstalledObjectTypes _type, Tile tile, bool _isInstalled)
    {
        InstalledObject obj = new InstalledObject();
        obj.baseTile = tile;
        obj.type = _type;
        obj.isInstalled = _isInstalled;

        if (tile.InstallObject(obj) == false)
        {
            return null;
        }
        
        return obj;
    }

    public void Update(float deltaTime)
    {
        if ((updateActionCallback != null))
        {
            updateActionCallback(this, deltaTime);
        }
    }
    public void Install()
    {
        isInstalled = true;
        baseTile.isPendingTask = false;
        baseTile.accessibility = InstalledObjectTypes.GetBaseAccessibility(type);
        GameManager.GetWorldGrid().InvalidatePathGraph();

        if(InstalledObjectTypes.GetBaseAccessibility(type) == Accessibility.DELAYED)
        {
            AddOnActionCallback(InstalledObjectAction.Door_UpdateAction);
        }

        if (updateObjectCallback != null)
        {
            updateObjectCallback(this);
        }
    }
    public void UnInstall()
    {
        GameManager.GetWorldGrid().InvalidatePathGraph();
        GameManager.GetInstalledSpriteController().Uninstall(this);
        baseTile.UninstallObject();

        if (InstalledObjectTypes.GetBaseAccessibility(type) == Accessibility.DELAYED)
        {
            RemoveOnActionCallback(InstalledObjectAction.Door_UpdateAction);
        }

        ObjectManager.SpawnObject(InstalledObjectTypes.GetDroppedObjectType(type), baseTile);
        baseTile = null; 
        UnityEngine.Object.Destroy(gameObject);
    }
    public void AddOnActionCallback(Action<InstalledObject, float> callback)
    {
        updateActionCallback += callback;
    }
    public void RemoveOnActionCallback(Action<InstalledObject, float> callback)
    {
        updateActionCallback -= callback;
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

    protected readonly Accessibility baseAccessibility;
    protected readonly DroppedObjectTypes droppedObject;

    protected readonly int width = 1;
    protected readonly int height = 1;

    public static readonly InstalledObjectTypes WALL = new InstalledObjectTypes(InstalledObjectType.WALL, 100, Accessibility.IMPASSABLE, DroppedObjectTypes.WOOD);
    public static readonly InstalledObjectTypes DOOR = new InstalledObjectTypes(InstalledObjectType.DOOR, 4, Accessibility.DELAYED);

    protected InstalledObjectTypes(InstalledObjectType _type, int _movementCost, Accessibility _baseAccessibility, DroppedObjectTypes _droppedObject = null)
    {
        type = _type;
        movementCost = _movementCost;
        baseAccessibility = _baseAccessibility;
        droppedObject = _droppedObject;
    }

    public static InstalledObjectType GetObjectType(InstalledObjectTypes type)
    {
        return type.type;
    }
    public static int GetMovementCost(InstalledObjectTypes type)
    {
        return type.movementCost;
    }
    public static Accessibility GetBaseAccessibility(InstalledObjectTypes type)
    {
        return type.baseAccessibility;
    }
    public static DroppedObjectTypes GetDroppedObjectType(InstalledObjectTypes type)
    {
        return type.droppedObject;
    }
}