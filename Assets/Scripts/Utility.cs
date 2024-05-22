using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static int GetRandomNumber(int min, int max)
    {
        return Random.Range(min, max);
    }
}
