using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

public static class RegionManager
{
    public const int regionSize = 10;
    public static Region[,] regions;

    public static void Init(WorldGrid grid, int size)
    {
        regions = new Region[size, size];

        for(int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                regions[x, y] = new Region(grid, x, y);
            }
        }
    }
}
