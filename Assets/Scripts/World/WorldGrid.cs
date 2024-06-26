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

    public int mapSize;

    public Vector2Int worldCentre;

    public Path_TileGraph pathGraph;

    public WorldGrid()
    {
        mapSize = GameManager.instance.mapSize;

        worldCentre = new Vector2Int(Mathf.FloorToInt(mapSize / 2), Mathf.FloorToInt(mapSize / 2));

        CellularAutomata ca = new CellularAutomata(mapSize, mapSize);
        cellularAutomataValues = ca.RandomFillMap();

        PerlinNoise noise = new PerlinNoise(mapSize, mapSize);
        noiseValues = noise.GenerateTerrainNoise();

        tiles = new Tile[mapSize, mapSize];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                tiles[x, y] = new Tile(this, x, y, noiseValues[x,y]);
            }
        }
    }
    public Tile GetTile(float _x, float _y)
    {
        int x = Mathf.FloorToInt(_x);
        int y = Mathf.FloorToInt(_y);

        if (x > mapSize - 1 || x < 0 || y > mapSize - 1 || y < 0)
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
