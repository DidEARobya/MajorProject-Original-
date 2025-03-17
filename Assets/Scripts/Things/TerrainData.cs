using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainData
{
    public TerrainType type;
    public int movementCost;
    public int randomPlantGrowthChance;
    public float fertilityMultiplier;
    //Optional for world noiseMap. Ignore if guaranteed generation is set.
    public float noiseMin;
    public float noiseMax;
}

