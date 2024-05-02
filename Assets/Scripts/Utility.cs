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
    public static float Get2DPerlin(int x, int y, float scale, int offset)
    {
        float seedVal = GenerateSeedValue(5, offset);

        float xSample = (x + 0.1f) * scale + seedVal;
        float ySample = (y + 0.1f) * scale + seedVal;

        return Mathf.PerlinNoise(xSample, ySample);
    }
    static float GenerateSeedValue(int seed, int sample)
    {
        System.Random rand = new System.Random(seed);
        float val = rand.Next(-10000, 10000);

        return val;
    }
}
