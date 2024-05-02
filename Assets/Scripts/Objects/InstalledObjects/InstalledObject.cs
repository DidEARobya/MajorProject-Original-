using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum InstalledObjectType
{
    FURNITURE,
    ORE
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

    protected Action<InstalledObject> updateObjectCallback;

    protected InstalledObject()
    {

    }

    public void Update(float deltaTime)
    {
        if (updateActionCallback != null)
        {
            updateActionCallback(this, deltaTime);
        }
    }
    public virtual void Install()
    {
        Debug.Log("Calling Parent Install Function");
    }
    public virtual void UnInstall()
    {
        Debug.Log("Calling Parent Uninstall Function");
    }
    public virtual int GetMovementCost()
    {
        Debug.Log("Calling Parent MovementCost Function");
        return 0;
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