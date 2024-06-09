using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectManager
{
    static HashSet<InstalledObject> installedObjects = new HashSet<InstalledObject>();
    static Action<InstalledObject> installObjectCallback;
    public static void InstallFurniture(FurnitureTypes type, ItemTypes baseMaterial, Tile tile, bool isInstalled)
    {
        InstalledObject obj = Furniture.PlaceObject(type, baseMaterial, tile, isInstalled);

        if (obj == null)
        {
            return;
        }

        if (installObjectCallback != null)
        {
            installObjectCallback(obj);
        }
    }
    public static void SpawnOre(OreTypes type, Tile tile)
    {
        InstalledObject obj = Ore.PlaceObject(type, tile);

        if (obj == null)
        {
            return;
        }

        if (installObjectCallback != null)
        {
            installObjectCallback(obj);
        }
    }
    public static void SpawnPlant(PlantTypes type, Tile tile, PlantState state)
    {
        InstalledObject obj = Plant.PlaceObject(type, tile, state);

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
    public static bool Contains(InstalledObject installedObject)
    {
        return installedObjects.Contains(installedObject);
    }
    public static void SetInstallObjectCallback(Action<InstalledObject> callback)
    {
        installObjectCallback += callback;
    }
    public static void RemoveInstallObjectCallback(Action<InstalledObject> callback)
    {
        installObjectCallback -= callback;
    }
}
