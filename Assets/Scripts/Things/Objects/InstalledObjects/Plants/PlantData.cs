using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class PlantData
{
    public PlantType type;
    public int movementCost;
    public int durability;
    public float growthRate;
    public int growthStageInterval;
    public bool needsTending;

    public List<ItemType> yieldItems;
    public List<int> lowYieldAmounts;
    public List<int> highYieldAmounts;
    public List<int> matureYieldAmounts;

    public Dictionary<ItemData, int> GetYield(PlantState state)
    {
        if(state == PlantState.SEED)
        {
            return null;
        }

        Dictionary<ItemData, int> yield = new Dictionary<ItemData, int>();
        List<int> amounts = new List<int>();

        switch (state)
        {
            case PlantState.EARLY_GROWTH:
                amounts = lowYieldAmounts;
                break;
            case PlantState.LATE_GROWTH:
                amounts = highYieldAmounts;
                break;
            case PlantState.GROWN:
                amounts = matureYieldAmounts;
                break;
        }

        for (int i = 0; i < yieldItems.Count; i++)
        {
            yield.Add(ThingsDataHandler.GetItemData(yieldItems[i]), amounts[i]);
        }

        return yield;
    }
}
