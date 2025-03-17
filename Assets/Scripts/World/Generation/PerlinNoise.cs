using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PerlinNoise
{
    int width;
    int height;

    float[,] noiseValues;

    int octaves;
    int seed = 5;
    float seedVal = 0;
    float scale = 0.15f;
    int offset = 20;
    float persistence;
    float lacunarity;

    System.Random random;

    public PerlinNoise(int _width, int _height)
    {
        GameManager manager = GameManager.instance;

        width = _width;
        height = _height;

        random = manager.random;

        seed = Utility.GetRandomInt(1, 9999);
        seedVal = GenerateSeedValue(seed);
        scale = manager.terrainScale;
        offset = manager.terrainOffset;
        octaves = manager.octaves;
        lacunarity = manager.lacunarity;
        persistence = manager.persistence;

        noiseValues = new float[width, height];
    }
    public float[,] GenerateTerrainNoise()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = 0;
                float amplitude = 1;
                float frequency = 1;

                for (int i = 0; i < octaves; i++)
                {
                    float xSample = (x + 0.1f) * scale * frequency + offset;
                    float ySample = (y + 0.1f) * scale * frequency + offset;

                    float perlinValue = Mathf.PerlinNoise(xSample, ySample) * 2 - 1;
                    noise += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                noiseValues[x, y] = noise;
            }
        }

        return noiseValues;
    }
    public float Get2DPerlin(int x, int y, float scale, int offset)
    {
        float xSample = (x + 0.1f) * scale + seedVal;
        float ySample = (y + 0.1f) * scale + seedVal;

        return Mathf.PerlinNoise(xSample, ySample);
    }

    float GenerateSeedValue(int seed)
    {
        return random.Next(-10000, 10000);
    }
}
