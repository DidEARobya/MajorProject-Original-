using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BuildingData
{
    public string name;
    public FurnitureType type;
    public int durability;
    public float constructionTime;
    public Accessibility baseAccessibility;
    public bool hasRelativeRotation;
    public List<ItemType> requirementsItems;
    public List<int> requirementsAmounts;

    private Dictionary<ItemData, int> _requirements;
    public void Init()
    {
        if(requirementsAmounts.Count != requirementsItems.Count)
        {
            Debug.Log(name + " requirements are invalid");
            return;
        }
    }
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
