using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements;

public class CellularAutomata
{
    int width;
    int height;

    int[,] cellularAutomataMap;

    int caIterations;
    int noiseDensity;

    System.Random random;

    public CellularAutomata(int _width, int _height)
    {
        GameManager manager = GameManager.instance;

        width = _width;
        height = _height;

        caIterations = manager.caIterations;
        noiseDensity = manager.noiseDensity;

        random = manager.random;

        cellularAutomataMap = new int[width, height];
    }

    public int[,] RandomFillMap()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                int rand = Utility.GetRandomNumber(0, 100);

                if(rand > noiseDensity)
                {
                    cellularAutomataMap[x, y] = 1;
                }
                else
                {
                    cellularAutomataMap[x, y] = 0;
                }
            }
        }

        for(int i = 0; i < caIterations; i++)
        {
            SmoothMap();
        }

        return cellularAutomataMap;
    }
    void SmoothMap()
    {
        int[,] baseGrid = cellularAutomataMap;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourTileValue = GetNeighbourCount(baseGrid, x, y);

                if(neighbourTileValue > 4)
                {
                    cellularAutomataMap[x, y] = 1;
                }
                else if(neighbourTileValue < 4)
                {
                    cellularAutomataMap[x, y] = 0;
                }

            }
        }
    }
    int GetNeighbourCount(int[,] grid, int _x, int _y)
    {
        int count = 0;

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = x + _x;
                int checkY = y + _y;

                if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                {
                    count += grid[checkX, checkY];
                }
            }
        }

        return count;
    }
}
