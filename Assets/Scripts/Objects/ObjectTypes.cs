using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum FurnitureType
{
    WOOD_WALL,
    STONE_WALL
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
    protected readonly int durability;

    protected readonly Accessibility baseAccessibility;
    protected readonly ConstructionRequirements requirements;

    protected readonly int width = 1;
    protected readonly int height = 1;

    public static readonly FurnitureTypes WOOD_WALL = new FurnitureTypes(FurnitureType.WOOD_WALL, 100, 50, 200, Accessibility.IMPASSABLE, ConstructionRequirements.WOOD_WALL);
    public static readonly FurnitureTypes DOOR = new FurnitureTypes(FurnitureType.STONE_WALL, 4, 100, 200, Accessibility.DELAYED, ConstructionRequirements.DOOR);

    protected FurnitureTypes(FurnitureType _type, int _movementCost, int _constructionTime, int _durability, Accessibility _baseAccessibility, ConstructionRequirements _requirements)
    {
        type = _type;
        movementCost = _movementCost;
        constructionTime = _constructionTime;
        durability = _durability;
        baseAccessibility = _baseAccessibility;
        requirements = _requirements;
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
    public static int GetDurability(FurnitureTypes type)
    {
        return type.durability;
    }
    public static Accessibility GetBaseAccessibility(FurnitureTypes type)
    {
        return type.baseAccessibility;
    }
    public static Dictionary<ItemTypes, int> GetRequirements(FurnitureTypes type)
    {
        return ConstructionRequirements.GetRequirements(type.requirements);
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

    protected readonly Accessibility baseAccessibility;
    protected readonly CropYields yield;

    protected readonly int width = 1;
    protected readonly int height = 1;

    public static readonly PlantTypes OAK_TREE = new PlantTypes(PlantType.OAK_TREE, 100, 20, 2f, Accessibility.IMPASSABLE, CropYields.OAK_TREE);

    protected PlantTypes(PlantType _type, int _movementCost, int _durability, float _growthRate, Accessibility _baseAccessibility, CropYields _yield)
    {
        type = _type;
        movementCost = _movementCost;
        durability = _durability;
        growthRate = _growthRate;
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
    public static float GetGrowthRate(PlantTypes type)
    {
        return type.growthRate;
    }
    public static Accessibility GetBaseAccessibility(PlantTypes type)
    {
        return type.baseAccessibility;
    }
    public static Dictionary<ItemTypes, int> GetYield(PlantTypes type)
    {
        return CropYields.GetYield(type.yield);
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