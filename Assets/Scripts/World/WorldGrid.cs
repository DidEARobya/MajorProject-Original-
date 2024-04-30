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

    public Vector2Int worldCentre;

    public Path_TileGraph pathGraph;

    public WorldGrid(int width = 50, int height = 50)
    {
        mapWidth = width;
        mapHeight = height;

        worldCentre = new Vector2Int(Mathf.FloorToInt(width / 2), Mathf.FloorToInt(height / 2));

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
