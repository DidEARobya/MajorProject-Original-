using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGrid
{
    protected Tile[,] tiles;

    public int mapWidth;
    public int mapHeight;

    public Path_TileGraph pathGraph;
    List<CharacterController> characters;

    Action<InstalledObject> installObjectCallback;
    Action<CharacterController> characterCreatedCallback;
    public WorldGrid(int width = 50, int height = 50)
    {
        mapWidth = width;
        mapHeight = height;

        tiles = new Tile[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
            }
        }

        InstalledObject wallPrototype = InstalledObject.CreatePrototype(InstalledObjectTypes.WALL);

        InstalledObjectPrototypes.AddPrototype(wallPrototype.type, wallPrototype);

        characters = new List<CharacterController>();
    }
    public void Update(float deltaTime)
    {
        foreach(CharacterController character in characters)
        {
            character.Update(deltaTime);
        }
    }
    public Tile GetTile(float _x, float _y)
    {
        int x = Mathf.FloorToInt(_x);
        int y = Mathf.FloorToInt(_y);

        if (x > mapWidth - 1 || x < 0 || y > mapHeight - 1 || y < 0)
        {
            Debug.Log("Tile is out of range");
            return null;
        }

        return tiles[x, y];
    }

    public void InstallObject(InstalledObjectTypes type, Tile tile)
    {
        if(InstalledObjectPrototypes.GetInstalledObject(type) == null)
        {
            Debug.Log("No prototype of " + type);
            return;
        }

        InstalledObject obj = InstalledObject.PlaceObject(type, tile);

        if(obj == null)
        {
            return;
        }

        if(installObjectCallback != null)
        {
            InvalidatePathGraph();
            installObjectCallback(obj);
        }
    }

    public CharacterController CreateCharacter(Tile tile)
    {
        CharacterController test = new CharacterController(tile);
        characters.Add(test);

        if(characterCreatedCallback != null)
        {
            characterCreatedCallback(test);
        }

        return test;
    }
    public void InvalidatePathGraph()
    {
        pathGraph = null;
    }
    public void SetInstallObjectCallback(Action<InstalledObject> callback)
    {
        installObjectCallback += callback;
    }
    public void RemoveInstallObjectCallback(Action<InstalledObject> callback)
    {
        installObjectCallback -= callback;
    }
    public void SetCharacterCreatedCallback(Action<CharacterController> callback)
    {
        characterCreatedCallback += callback;
    }
    public void RemoveCharacterCreatedCallback(Action<CharacterController> callback)
    {
        characterCreatedCallback -= callback;
    }
}

public static class InstalledObjectPrototypes
{
    static readonly Dictionary<InstalledObjectTypes, InstalledObject> installedObjects = new Dictionary<InstalledObjectTypes, InstalledObject>();

    public static void AddPrototype(InstalledObjectTypes type, InstalledObject obj)
    {
        if(installedObjects.ContainsKey(type))
        {
            return;
        }

        installedObjects.Add(type, obj);
    }

    public static InstalledObject GetInstalledObject(InstalledObjectTypes type)
    {
        if(installedObjects.ContainsKey(type))
        {
            return installedObjects[type];
        }

        return null;
    }

}
