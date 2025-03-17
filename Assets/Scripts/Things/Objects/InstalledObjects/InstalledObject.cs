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
    public virtual void Install() 
    {
        ObjectManager.AddInstalledObject(this);
        baseTile.installedObject = this;
    }
    public virtual void UnInstall() 
    {
        ObjectManager.RemoveInstalledObject(this);
        baseTile.installedObject = null;
        baseTile.accessibility = Accessibility.ACCESSIBLE;
    }
    public virtual int GetMovementCost() { return 0; }
    public virtual string GetObjectNameToString()
    {
        return " ";
    }
    public virtual string GetObjectSpriteName(bool updateNeighbours)
    {
        return GetObjectNameToString();
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