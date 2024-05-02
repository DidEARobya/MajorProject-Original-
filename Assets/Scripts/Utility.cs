using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static Path_AStar CheckIfTaskValid(Tile start, Tile goal)
    {
        Path_AStar pathFinder = new Path_AStar(start, goal, true);

        if (pathFinder.Length() == 0)
        {
            return null;
        }

        return pathFinder;
    }
    public static int GetRandomNumber(int min, int max)
    {
        return Random.Range(min, max);
    }
}
