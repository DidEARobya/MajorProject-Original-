using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

[JsonConverter(typeof(StringEnumConverter))]
public enum FurnitureType
{
    [EnumMember(Value = "WALL")]
    WALL,
    [EnumMember(Value = "DOOR")]
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
[JsonConverter(typeof(StringEnumConverter))]
public enum ItemType
{
    [EnumMember(Value = "WOOD")]
    WOOD,
    [EnumMember(Value = "STONE")]
    STONE,
    [EnumMember(Value = "IRON")]
    IRON
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
    public static Dictionary<ItemData, int> GetComponents(OreTypes type)
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
    public static Dictionary<ItemData, int> GetYield(PlantTypes type, PlantState state)
    {
        return CropYields.GetYield(type.yield, state);
    }
}