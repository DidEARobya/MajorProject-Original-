using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionRequirements
{
    protected readonly FurnitureTypes type;
    protected readonly Dictionary<ItemTypes, int> requirements;
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
public class CropYields
{
    protected readonly PlantTypes type;
    protected readonly Dictionary<ItemTypes, int> lowYield;
    protected readonly Dictionary<ItemTypes, int> highYield;
    protected readonly Dictionary<ItemTypes, int> matureYield;

    public static readonly CropYields OAK_TREE = new CropYields(PlantTypes.OAK_TREE, new Dictionary<ItemTypes, int>() { { ItemTypes.WOOD, 4 } }, new Dictionary<ItemTypes, int>() { { ItemTypes.WOOD, 8 } }, new Dictionary<ItemTypes, int>() { { ItemTypes.WOOD, 16 } });

    protected CropYields(PlantTypes _type, Dictionary<ItemTypes, int> _lowYield, Dictionary<ItemTypes, int> _highYield, Dictionary<ItemTypes, int> _matureYield)
    {
        type = _type;
        lowYield = _lowYield;
        highYield = _highYield;
        matureYield = _matureYield;
    }

    public static Dictionary<ItemTypes, int> GetYield(CropYields type, PlantState state)
    {
        switch(state)
        {
            case PlantState.SEED:
                return null;
            case PlantState.EARLY_GROWTH:
                return type.lowYield;
            case PlantState.LATE_GROWTH:
                return type.highYield;
            case PlantState.GROWN:
                return type.matureYield;
        }

        return null;
    }
}
public class FloorRequirements
{
    protected readonly FloorTypes type;
    protected readonly Dictionary<ItemTypes, int> requirements;

    public static readonly FloorRequirements WOOD = new FloorRequirements(FloorTypes.WOOD, new Dictionary<ItemTypes, int>() { { ItemTypes.WOOD, 2 } });
    public static readonly FloorRequirements STONE = new FloorRequirements(FloorTypes.STONE, new Dictionary<ItemTypes, int>() { { ItemTypes.STONE, 2 } });

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