using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static bool IsValidTile(Tile tile)
    {
        if (tile == null)
        {
            return false;
        }

        if (tile.installedObject != null)
        {
            return false;
        }

        return true;
    }

    public static int GetRandomInt(int min, int max)
    {
        return Random.Range(min, max);
    }
    public static float GetRandomFloat(float min, float max)
    {
        return Random.Range(min, max);
    }
}
