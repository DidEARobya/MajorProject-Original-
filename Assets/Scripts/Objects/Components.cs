using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreComponents
{
    protected readonly OreTypes type;
    protected readonly Dictionary<ItemData, int> components;

    public static readonly OreComponents STONE = new OreComponents(OreTypes.STONE_ORE, new Dictionary<ItemData, int>() {  });
    public static readonly OreComponents IRON = new OreComponents(OreTypes.IRON_ORE, new Dictionary<ItemData, int>() { } );

    protected OreComponents(OreTypes _type, Dictionary<ItemData, int> _components)
    {
        type = _type;
        components = _components;
    }

    public static Dictionary<ItemData, int> GetComponents(OreComponents type)
    {
        return type.components;
    }
}
public class CropYields
{
    protected readonly PlantTypes type;
    protected readonly Dictionary<ItemData, int> lowYield;
    protected readonly Dictionary<ItemData, int> highYield;
    protected readonly Dictionary<ItemData, int> matureYield;

    public static readonly CropYields OAK_TREE = new CropYields(PlantTypes.OAK_TREE, new Dictionary<ItemData, int>() { }, new Dictionary<ItemData, int>() { }, new Dictionary<ItemData, int>() { });

    protected CropYields(PlantTypes _type, Dictionary<ItemData, int> _lowYield, Dictionary<ItemData, int> _highYield, Dictionary<ItemData, int> _matureYield)
    {
        type = _type;
        lowYield = _lowYield;
        highYield = _highYield;
        matureYield = _matureYield;
    }

    public static Dictionary<ItemData, int> GetYield(CropYields type, PlantState state)
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
    protected readonly Dictionary<ItemData, int> requirements;

    public static readonly FloorRequirements WOOD = new FloorRequirements(FloorTypes.WOOD, new Dictionary<ItemData, int>() {  });
    public static readonly FloorRequirements STONE = new FloorRequirements(FloorTypes.STONE, new Dictionary<ItemData, int>() {  });

    protected FloorRequirements(FloorTypes _type, Dictionary<ItemData, int> _requirements)
    {
        type = _type;
        requirements = _requirements;
    }

    public static Dictionary<ItemData, int> GetRequirements(FloorRequirements type)
    {
        return type.requirements;
    }
}