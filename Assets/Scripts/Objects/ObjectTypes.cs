using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum InstalledObjectType
{
    WALL,
    DOOR
}
public enum ItemType
{
    WOOD,
    STONE
}
public class InstalledObjectTypes
{
    protected readonly InstalledObjectType type;
    protected readonly int movementCost;

    protected readonly Accessibility baseAccessibility;
    protected readonly ConstructionRequirements requirements;

    protected readonly int width = 1;
    protected readonly int height = 1;

    public static readonly InstalledObjectTypes WALL = new InstalledObjectTypes(InstalledObjectType.WALL, 100, Accessibility.IMPASSABLE, ConstructionRequirements.WALL);
    public static readonly InstalledObjectTypes DOOR = new InstalledObjectTypes(InstalledObjectType.DOOR, 4, Accessibility.DELAYED, ConstructionRequirements.DOOR);

    protected InstalledObjectTypes(InstalledObjectType _type, int _movementCost, Accessibility _baseAccessibility, ConstructionRequirements _requirements)
    {
        type = _type;
        movementCost = _movementCost;
        baseAccessibility = _baseAccessibility;
        requirements = _requirements;
    }

    public static InstalledObjectType GetObjectType(InstalledObjectTypes type)
    {
        return type.type;
    }
    public static int GetMovementCost(InstalledObjectTypes type)
    {
        return type.movementCost;
    }
    public static Accessibility GetBaseAccessibility(InstalledObjectTypes type)
    {
        return type.baseAccessibility;
    }
    public static Dictionary<ItemTypes, int> GetRequirements(InstalledObjectTypes type)
    {
        return ConstructionRequirements.GetRequirements(type.requirements);
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
        return type.maxStackSize;
    }
}