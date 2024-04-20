using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InstalledObjectsManager
{
    static List<InstalledObject> installedObjects = new List<InstalledObject>();
    public static void Update(float deltaTime)
    {
        foreach (InstalledObject obj in installedObjects)
        {
            obj.Update(deltaTime);
        }
    }
    public static void AddInstalledObject(InstalledObject installedObject)
    {
        if(installedObject == null || installedObjects.Contains(installedObject) == true)
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
}
