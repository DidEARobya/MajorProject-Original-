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

    private Dictionary<ItemData, int> requirements;

    public BuildingData(string name, FurnitureType type, int durability, Accessibility baseAccessibility, bool hasRelativeRotation, List<ItemType> requirementsItems, List<int> requirementAmounts)
    {
        this.name = name;
        this.type = type;
        this.durability = durability;
        this.baseAccessibility = baseAccessibility;
        this.hasRelativeRotation = hasRelativeRotation;
        this.requirementsItems = requirementsItems;
        this.requirementsAmounts = requirementAmounts;
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
        if (requirements != null && requirements.Count > 0)
        {
            return requirements;
        }

        requirements = new Dictionary<ItemData, int>();

        for (int i = 0; i < requirementsAmounts.Count; i++)
        {
            requirements.Add(ItemDataHandler.GetItemData(requirementsItems[i]), requirementsAmounts[i]);
        }

        return requirements;
    }
}
