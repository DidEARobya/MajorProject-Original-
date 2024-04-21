using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectManager
{
    static List<InstalledObject> installedObjects = new List<InstalledObject>();
    static Action<InstalledObject> installObjectCallback;


    public static void InstallObject(InstalledObjectTypes type, Tile tile, bool isInstalled)
    {
        InstalledObject obj = InstalledObject.PlaceObject(type, tile, isInstalled);

        if (obj == null)
        {
            return;
        }

        if (installObjectCallback != null)
        {
            installObjectCallback(obj);
        }
    }

    public static void Update(float deltaTime)
    {
        foreach (InstalledObject obj in installedObjects)
        {
            obj.Update(deltaTime);
        }
        foreach (DroppedObject obj in droppedObjects)
        {
            obj.Update(deltaTime);
        }
    }
    public static void AddInstalledObject(InstalledObject installedObject)
    {
        if (installedObject == null || installedObjects.Contains(installedObject) == true)
        {
            return;
        }

        installedObjects.Add(installedObject);
    }
    public static void RemoveInstalledObject(InstalledObject installedObject)
    {
        if (installedObjects.Contains(installedObject) == false)
        {
            return;
        }

        installedObjects.Remove(installedObject);
    }
    public static void SetInstallObjectCallback(Action<InstalledObject> callback)
    {
        installObjectCallback += callback;
    }
    public static void RemoveInstallObjectCallback(Action<InstalledObject> callback)
    {
        installObjectCallback -= callback;
    }

    static List<DroppedObject> droppedObjects = new List<DroppedObject>();
    static Action<DroppedObject> droppedObjectCallback;

    public static void SpawnObject(DroppedObjectTypes type, Tile tile)
    {
        DroppedObject obj = DroppedObject.SpawnObject(type, tile);

        if (obj == null)
        {
            return;
        }

        if (droppedObjectCallback != null)
        {
            droppedObjectCallback(obj);
        }
    }
    public static void AddDroppedObject(DroppedObject droppedObject)
    {
        if (droppedObject == null || droppedObjects.Contains(droppedObject) == true)
        {
            return;
        }

        droppedObjects.Add(droppedObject);
    }
    public static void RemoveDroppedObject(DroppedObject droppedObject)
    {
        if (droppedObjects.Contains(droppedObject) == false)
        {
            return;
        }

        droppedObjects.Remove(droppedObject);
    }
    public static void SetDroppedObjectCallback(Action<DroppedObject> callback)
    {
        droppedObjectCallback += callback;
    }
    public static void RemoveDroppedObjectCallback(Action<DroppedObject> callback)
    {
        droppedObjectCallback -= callback;
    }
}
