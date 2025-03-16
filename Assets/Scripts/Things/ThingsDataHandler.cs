using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThingsDataHandler : MonoBehaviour
{
    private static ThingsDataHandler _instance;

    [SerializeField]
    public string buildingsFileName;
    [SerializeField]
    public string itemsFileName;
    [SerializeField]
    public string oreFileName;
    [SerializeField]
    public string plantFileName;
    [SerializeField]
    public string floorFileName;

    private List<BuildingData> _buildings = new List<BuildingData>();
    private Dictionary<string, BuildingData> _buildingDictionary = new Dictionary<string, BuildingData>();

    private List<ItemData> _items = new List<ItemData>();
    private Dictionary<ItemType, ItemData> _itemDictionary = new Dictionary<ItemType, ItemData>();

    private List<OreData> _ores = new List<OreData>();
    private Dictionary<OreType, OreData> _oreDictionary = new Dictionary<OreType, OreData>();

    private List<PlantData> _plants = new List<PlantData>();
    private Dictionary<PlantType, PlantData> _plantDictionary = new Dictionary<PlantType, PlantData>();

    private List<FloorData> _floors = new List<FloorData>();
    private Dictionary<FloorType, FloorData> _floorDictionary = new Dictionary<FloorType, FloorData>();

    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        _items = FileHandler.ReadFromJSON<ItemData>(itemsFileName, false);

        foreach (ItemData data in _items)
        {
            _itemDictionary.Add(data.type, data);
        }

        _ores = FileHandler.ReadFromJSON<OreData>(oreFileName, false);

        foreach (OreData data in _ores)
        {
            _oreDictionary.Add(data.type, data);
        }

        _buildings = FileHandler.ReadFromJSON<BuildingData>(buildingsFileName, false);

        foreach (BuildingData data in _buildings)
        {
            data.Init();
            _buildingDictionary.Add(data.name, data);
        }

        _plants = FileHandler.ReadFromJSON<PlantData>(plantFileName, false);
        
        foreach(PlantData data in _plants)
        {
            _plantDictionary.Add(data.type, data);
        }

        _floors = FileHandler.ReadFromJSON<FloorData>(floorFileName, false);

        foreach (FloorData data in _floors)
        {
            _floorDictionary.Add(data.type, data);
        }
    }

    public static BuildingData GetBuildingData(string name)
    {
        if (_instance._buildingDictionary.ContainsKey(name) == false)
        {
            return null;
        }

        return _instance._buildingDictionary[name];
    }
    public static ItemData GetItemData(ItemType type)
    {
        if (_instance._itemDictionary.ContainsKey(type) == false)
        {
            return null;
        }

        return _instance._itemDictionary[type];
    }
    public static OreData GetOreData(OreType type)
    {
        if (_instance._oreDictionary.ContainsKey(type) == false)
        {
            return null;
        }

        return _instance._oreDictionary[type];
    }
    public static PlantData GetPlantData(PlantType type)
    {
        if (_instance._plantDictionary.ContainsKey(type) == false)
        {
            return null;
        }

        return _instance._plantDictionary[type];
    }
    public static FloorData GetFloorData(FloorType type)
    {
        if (_instance._floorDictionary.ContainsKey(type) == false)
        {
            return null;
        }

        return _instance._floorDictionary[type];
    }
}
