using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldGrid
{
    protected Tile[,] tiles;

    protected float[,] noiseValues;
    public int[,] cellularAutomataValues;

    public int mapWidth;
    public int mapHeight;

    public Vector2Int worldCentre;

    public Path_TileGraph pathGraph;

    public WorldGrid()
    {
        mapWidth = GameManager.instance.mapWidth;
        mapHeight = GameManager.instance.mapHeight;

        worldCentre = new Vector2Int(Mathf.FloorToInt(mapWidth / 2), Mathf.FloorToInt(mapHeight / 2));

        CellularAutomata ca = new CellularAutomata(mapWidth, mapHeight);
        cellularAutomataValues = ca.RandomFillMap();

        PerlinNoise noise = new PerlinNoise(mapWidth, mapHeight);
        noiseValues = noise.GenerateTerrainNoise();

        tiles = new Tile[mapWidth, mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                tiles[x, y] = new Tile(this, x, y, noiseValues[x,y]);
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
