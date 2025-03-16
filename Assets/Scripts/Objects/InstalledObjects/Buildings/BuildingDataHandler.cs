using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;


public class BuildingDataHandler : MonoBehaviour
{
    private static BuildingDataHandler instance;

    [SerializeField]
    public string fileName;

    private List<BuildingData> _buildings = new List<BuildingData>();
    private Dictionary<string, BuildingData> _buildingDictionary = new Dictionary<string, BuildingData>();

    private void Start()
    {
        _buildings = FileHandler.ReadFromJSON<BuildingData>(fileName, false);

        foreach(BuildingData data in _buildings)
        {
            data.Init();
            _buildingDictionary.Add(data.name, data);
        }

        ObjectManager.SetBuildingDataHandler(this);

        instance = this;
    }

    public static BuildingDataHandler GetInstance()
    {
        return instance;
    }
    public BuildingData GetBuildingData(string name)
    {
        if(_buildingDictionary.ContainsKey(name) == false)
        {
            return null;
        }

        return _buildingDictionary[name];
    }
}

