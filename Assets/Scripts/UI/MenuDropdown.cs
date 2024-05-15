using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuDropdown : MonoBehaviour
{
    public GameObject menu;
    public StructuresUIObject uiObject;

    public List<Button> buttons = new List<Button>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            ItemType type = new ItemType();
            type = (ItemType)i;

            buttons[i].onClick.AddListener(() => { SetType(type); });
        }
    }
    public void SetType(ItemType type)
    {
        if(uiObject == null)
        {
            Debug.Log("No UI Object");
            return;
        }

        switch(type)
        {
            case ItemType.WOOD:
            uiObject.SetType(ItemTypes.WOOD); 
            break;

            case ItemType.STONE:
            uiObject.SetType(ItemTypes.STONE);
            break;

            case ItemType.IRON:
            uiObject.SetType(ItemTypes.IRON);
            break;
        }
    }
}
