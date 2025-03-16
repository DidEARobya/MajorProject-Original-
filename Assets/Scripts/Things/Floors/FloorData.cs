using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorData
{
    public FloorType type;
    public int movementCost;
    public float constructionTime;
    public List<ItemType> requirementsItems;
    public List<int> requirementsAmounts;

    private Dictionary<ItemData, int> _requirements;
    public Dictionary<ItemData, int> GetRequirements()
    {
        if (_requirements != null && _requirements.Count > 0)
        {
            return _requirements;
        }

        _requirements = new Dictionary<ItemData, int>();

        for (int i = 0; i < requirementsAmounts.Count; i++)
        {
            _requirements.Add(ThingsDataHandler.GetItemData(requirementsItems[i]), requirementsAmounts[i]);
        }

        return _requirements;
    }
}
