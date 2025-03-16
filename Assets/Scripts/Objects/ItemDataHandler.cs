using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataHandler : MonoBehaviour
{
    private static ItemDataHandler instance;
    [SerializeField]
    public string fileName;

    private List<ItemData> _items = new List<ItemData>();
    private Dictionary<ItemType, ItemData> _itemDictionary = new Dictionary<ItemType, ItemData>();

    // Start is called before the first frame update
    void Awake()
    {
        _items = FileHandler.ReadFromJSON<ItemData>(fileName, false);

        foreach (ItemData data in _items)
        {
            _itemDictionary.Add(data.type, data);
        }

        Debug.Log(_itemDictionary.Count);
        instance = this;
    }

    public static ItemData GetItemData(ItemType type)
    {
        if (instance != null && instance._itemDictionary.ContainsKey(type))
        {
            return instance._itemDictionary[type];
        }

        return null;
    }

}
