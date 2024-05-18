using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum InstalledObjectType
{
    FURNITURE,
    ORE,
    PLANT
}
public class InstalledObject
{
    public Tile baseTile;
    public GameObject gameObject;
    public InstalledObjectType type;

    public Action<InstalledObject, float> updateActionCallback;

    public float openVal = 0.0f;
    public bool isOpening = false;

    public float durability;
    public bool isInstalled = false;
    public bool hasRelativeRotation = false;

    protected Action<InstalledObject> updateObjectCallback;

    protected InstalledObject()
    {

    }

    public virtual void Update(float deltaTime)
    {
        if (updateActionCallback != null)
        {
            updateActionCallback(this, deltaTime);
        }
    }
    public void UpdateNeighourSprites(InstalledObject obj)
    {
        if(obj == null)
        {
            return;
        }

        if(obj.baseTile.North != null && obj.baseTile.North.IsObjectInstalled() == true)
        {
            updateObjectCallback(obj.baseTile.North.installedObject);
        }
        if (obj.baseTile.South != null && obj.baseTile.South.IsObjectInstalled() == true)
        {
            updateObjectCallback(obj.baseTile.South.installedObject);
        }
        if (obj.baseTile.West != null && obj.baseTile.West.IsObjectInstalled() == true)
        {
            updateObjectCallback(obj.baseTile.West.installedObject);
        }
        if (obj.baseTile.East != null && obj.baseTile.East.IsObjectInstalled() == true)
        {
            updateObjectCallback(obj.baseTile.East.installedObject);
        }
    }
    public virtual void Install() { }
    public virtual void UnInstall() { }
    public virtual int GetMovementCost() { return 0; }
    public virtual string GetObjectType()
    {
        return " ";
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