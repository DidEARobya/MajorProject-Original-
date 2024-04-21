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

    public void InvalidatePathGraph()
    {
        CharacterManager.ResetCharacterTaskIgnores();
        pathGraph = null;
    }
}