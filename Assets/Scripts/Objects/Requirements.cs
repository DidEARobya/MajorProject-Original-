using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionRequirements
{
    protected readonly InstalledObjectTypes type;
    protected readonly Dictionary<ItemTypes, int> requirements;

    public static readonly ConstructionRequirements WALL = new ConstructionRequirements(InstalledObjectTypes.WALL, new Dictionary<ItemTypes, int>() { { ItemTypes.WOOD, 4 }, { ItemTypes.STONE, 4 } });
    public static readonly ConstructionRequirements DOOR = new ConstructionRequirements(InstalledObjectTypes.DOOR, new Dictionary<ItemTypes, int>() { { ItemTypes.STONE, 4 } });
    protected ConstructionRequirements(InstalledObjectTypes _type, Dictionary<ItemTypes, int> _requirements)
    {
        type = _type;
        requirements = _requirements;
    }

    public static Dictionary<ItemTypes, int> GetRequirements(ConstructionRequirements type)
    {
        return type.requirements;
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