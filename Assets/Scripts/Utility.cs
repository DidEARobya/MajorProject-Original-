using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static int GetRandomInt(int min, int max)
    {
        return Random.Range(min, max);
    }
    public static float GetRandomFloat(float min, float max)
    {
        return Random.Range(min, max);
    }
}
