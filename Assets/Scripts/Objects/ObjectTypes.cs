using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum FurnitureType
{
    WALL,
    DOOR
}
public enum OreType
{
    STONE_ORE,
    IRON_ORE
}
public enum PlantType
{
    OAK_TREE
}
public enum ItemType
{
    WOOD,
    STONE,
    IRON
}
public class FurnitureTypes
{
    protected readonly FurnitureType type;
    protected readonly int movementCost;
    protected readonly int constructionTime;
    protected readonly Dictionary<ItemTypes, int> durabilities;

    protected readonly ConstructionRequirements requirements;
    protected readonly int baseMaterialAmount;

    protected readonly Accessibility baseAccessibility;

    protected readonly int width = 1;
    protected readonly int height = 1;

    protected readonly bool hasRelativeRotation;

    public static readonly FurnitureTypes WALL = new FurnitureTypes(FurnitureType.WALL, 100, 50, new Dictionary<ItemTypes, int>() { { ItemTypes.WOOD, 300 }, { ItemTypes.STONE, 400 }, { ItemTypes.IRON, 500 } }, Accessibility.IMPASSABLE, false, 4);
    public static readonly FurnitureTypes DOOR = new FurnitureTypes(FurnitureType.DOOR, 4, 100, new Dictionary<ItemTypes, int>() { { ItemTypes.WOOD, 300 }, { ItemTypes.STONE, 400 }, { ItemTypes.IRON, 500 } }, Accessibility.DELAYED, true, 4);

    protected FurnitureTypes(FurnitureType _type, int _movementCost, int _constructionTime, Dictionary<ItemTypes, int> _durabilities, Accessibility _baseAccessibility, bool relativeRotation, int _baseMaterialAmount, ConstructionRequirements _requirements = null)
    {
        type = _type;
        movementCost = _movementCost;
        constructionTime = _constructionTime;
        durabilities = _durabilities;
        baseAccessibility = _baseAccessibility;
        baseMaterialAmount = _baseMaterialAmount;
        requirements = _requirements;
        hasRelativeRotation = relativeRotation;
    }
    public static FurnitureType GetObjectType(FurnitureTypes type)
    {
        return type.type;
    }
    public static int GetMovementCost(FurnitureTypes type)
    {
        return type.movementCost;
    }
    public static int GetConstructionTime(FurnitureTypes type)
    {
        return type.constructionTime;
    }
    public static int GetDurability(FurnitureTypes type, ItemTypes material)
    {
        return type.durabilities[material];
    }
    public static bool HasRelativeRotation(FurnitureTypes type)
    {
        return type.hasRelativeRotation;
    }
    public static Accessibility GetBaseAccessibility(FurnitureTypes type)
    {
        return type.baseAccessibility;
    }
    public static Dictionary<ItemTypes, int> GetRequirements(FurnitureTypes type, ItemTypes material)
    {
        if(type.requirements == null)
        {
            return new Dictionary<ItemTypes, int>() { { material, type.baseMaterialAmount } };
        }

        Dictionary<ItemTypes, int> toReturn = new Dictionary<ItemTypes, int>(ConstructionRequirements.GetRequirements(type.requirements));
        toReturn.Add(material, type.baseMaterialAmount);

        return toReturn;
    }
}
public class OreTypes
{
    protected readonly OreType type;
    protected readonly int movementCost;
    protected readonly int durability;

    protected readonly Accessibility baseAccessibility;
    protected readonly OreComponents components;

    protected readonly int width = 1;
    protected readonly int height = 1;

    public static readonly OreTypes STONE_ORE = new OreTypes(OreType.STONE_ORE, 100, 200, Accessibility.IMPASSABLE, OreComponents.STONE);
    public static readonly OreTypes IRON_ORE = new OreTypes(OreType.IRON_ORE, 100, 200, Accessibility.IMPASSABLE, OreComponents.IRON);

    protected OreTypes(OreType _type, int _movementCost, int _durability, Accessibility _baseAccessibility, OreComponents _components)
    {
        type = _type;
        movementCost = _movementCost;
        durability = _durability;
        baseAccessibility = _baseAccessibility;
        components = _components;
    }

    public static OreType GetObjectType(OreTypes type)
    {
        return type.type;
    }
    public static int GetMovementCost(OreTypes type)
    {
        return type.movementCost;
    }
    public static int GetDurability(OreTypes type)
    {
        return type.durability;
    }
    public static Accessibility GetBaseAccessibility(OreTypes type)
    {
        return type.baseAccessibility;
    }
    public static Dictionary<ItemTypes, int> GetComponents(OreTypes type)
    {
        return OreComponents.GetComponents(type.components);
    }
}
public class PlantTypes
{
    protected readonly PlantType type;
    protected readonly int movementCost;
    protected readonly int durability;
    protected readonly float growthRate;
    protected readonly int growthStage;
    protected readonly bool needsTending;

    protected readonly Accessibility baseAccessibility;
    protected readonly CropYields yield;

    protected readonly int width = 1;
    protected readonly int height = 1;

    public static readonly PlantTypes OAK_TREE = new PlantTypes(PlantType.OAK_TREE, 100, 20, 100, 2f, false, Accessibility.ACCESSIBLE, CropYields.OAK_TREE);

    protected PlantTypes(PlantType _type, int _movementCost, int _durability, int _growthStage, float _growthRate, bool _needsTending, Accessibility _baseAccessibility, CropYields _yield)
    {
        type = _type;
        movementCost = _movementCost;
        durability = _durability;
        growthRate = _growthRate;
        growthStage = _growthStage;
        needsTending = _needsTending;
        baseAccessibility = _baseAccessibility;
        yield = _yield;
    }

    public static PlantType GetObjectType(PlantTypes type)
    {
        return type.type;
    }
    public static int GetMovementCost(PlantTypes type)
    {
        return type.movementCost;
    }
    public static int GetDurability(PlantTypes type)
    {
        return type.durability;
    }
    public static int GetGrowthStage(PlantTypes type)
    {
        return type.growthStage;
    }
    public static float GetGrowthRate(PlantTypes type)
    {
        return type.growthRate;
    }
    public static bool GetNeedsTending(PlantTypes type)
    {
        return type.needsTending;
    }
    public static Accessibility GetBaseAccessibility(PlantTypes type)
    {
        return type.baseAccessibility;
    }
    public static Dictionary<ItemTypes, int> GetYield(PlantTypes type, PlantState state)
    {
        return CropYields.GetYield(type.yield, state);
    }
}
public class ItemTypes
{
    protected readonly ItemType type;
    protected readonly int movementCost;

    protected readonly int width = 1;
    protected readonly int height = 1;

    protected readonly int maxStackSize;

    public static readonly ItemTypes WOOD = new ItemTypes(ItemType.WOOD, 50);
    public static readonly ItemTypes STONE = new ItemTypes(ItemType.STONE, 50);
    public static readonly ItemTypes IRON = new ItemTypes(ItemType.IRON, 50);

    protected ItemTypes(ItemType _type, int _maxStackSize, int _movementCost = 0)
    {
        type = _type;
        movementCost = _movementCost;
        maxStackSize = _maxStackSize;
    }

    public static ItemType GetItemType(ItemTypes type)
    {
        return type.type;
    }
    public static int GetMovementCost(ItemTypes type)
    {
        return type.movementCost;
    }
    public static int GetMaxStackSize(ItemTypes type)
    {
        if(type == null)
        {
            return 0;
        }

        return type.maxStackSize;
    }
}