using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InstalledObject
{
    public Tile baseTile;
    public InstalledObjectTypes type;
    public GameObject gameObject;

    public Action<InstalledObject, float> updateActionCallback;

    public float openVal = 0.0f;
    public bool isOpening = false;

    public float durability;
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
        obj.durability = InstalledObjectTypes.GetDurability(_type);

        if (tile.InstallObject(obj) == false)
        {
            return null;
        }
        
        return obj;
    }

    public void Update(float deltaTime)
    {
        if (updateActionCallback != null)
        {
            updateActionCallback(this, deltaTime);
        }
    }
    public void Install()
    {
        isInstalled = true;
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
        if (InstalledObjectTypes.GetBaseAccessibility(type) == Accessibility.DELAYED)
        {
            RemoveOnActionCallback(InstalledObjectAction.Door_UpdateAction);
        }

        GameManager.GetInstalledSpriteController().Uninstall(this);

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