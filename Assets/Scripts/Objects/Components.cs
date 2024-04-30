using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionRequirements
{
    protected readonly FurnitureTypes type;
    protected readonly Dictionary<ItemTypes, int> requirements;

    public static readonly ConstructionRequirements WOOD_WALL = new ConstructionRequirements(FurnitureTypes.WOOD_WALL, new Dictionary<ItemTypes, int>() { { ItemTypes.WOOD, 4 } });
    public static readonly ConstructionRequirements DOOR = new ConstructionRequirements(FurnitureTypes.DOOR, new Dictionary<ItemTypes, int>() { { ItemTypes.STONE, 4 } });
    protected ConstructionRequirements(FurnitureTypes _type, Dictionary<ItemTypes, int> _requirements)
    {
        type = _type;
        requirements = _requirements;
    }

    public static Dictionary<ItemTypes, int> GetRequirements(ConstructionRequirements type)
    {
        return type.requirements;
    }
}
public class OreComponents
{
    protected readonly OreTypes type;
    protected readonly Dictionary<ItemTypes, int> components;

    public static readonly OreComponents STONE = new OreComponents(OreTypes.STONE_ORE, new Dictionary<ItemTypes, int>() { { ItemTypes.STONE, 8 } });
    public static readonly OreComponents IRON = new OreComponents(OreTypes.IRON_ORE, new Dictionary<ItemTypes, int>() { { ItemTypes.IRON, 5 } });

    protected OreComponents(OreTypes _type, Dictionary<ItemTypes, int> _components)
    {
        type = _type;
        components = _components;
    }

    public static Dictionary<ItemTypes, int> GetComponents(OreComponents type)
    {
        return type.components;
    }
}

public class FloorRequirements
{
    protected readonly FloorTypes type;
    protected readonly Dictionary<ItemTypes, int> requirements;

    public static readonly FloorRequirements WOOD = new FloorRequirements(FloorTypes.WOOD, new Dictionary<ItemTypes, int>() { { ItemTypes.WOOD, 1 } });

    protected FloorRequirements(FloorTypes _type, Dictionary<ItemTypes, int> _requirements)
    {
        type = _type;
        requirements = _requirements;
    }

    public static Dictionary<ItemTypes, int> GetRequirements(FloorRequirements type)
    {
        return type.requirements;
    }
}