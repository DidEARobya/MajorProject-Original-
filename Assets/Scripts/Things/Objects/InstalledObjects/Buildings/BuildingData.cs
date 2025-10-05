using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum ConstructionType
{
    BUILDING, 
    FLOORING
}

public class ConstructionData
{
    protected ConstructionData(ConstructionType type)
    {
       constructionType = type;
    }

    public readonly ConstructionType constructionType;

    public float constructionTime;
    public int movementCost;
    public List<ItemType> requirementsItems;
    public List<int> requirementsAmounts;
}

[Serializable]
public class BuildingData : ConstructionData
{
    public string name;
    public BuildingType type;
    public Accessibility baseAccessibility;
    public int width;
    public int height;
    public int durability;
    public bool canRotate;

    private Dictionary<ItemData, int> _requirements;

    public BuildingData() : base(ConstructionType.BUILDING)
    {
    }

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

[Serializable]
public class FloorData : ConstructionData
{
    public FloorData() : base(ConstructionType.FLOORING)
    {
    }

    public FloorType type;

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