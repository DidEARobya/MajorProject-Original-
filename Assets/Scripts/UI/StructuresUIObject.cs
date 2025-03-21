using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class StructuresUIObject : UIObject, IButtonClickHandler
{
    MouseController mouseController;
    public BuildingType objectType;

    ItemType itemType;
    public string additionalInfo;

    // Start is called before the first frame update
    void Start()
    {
        itemType = ItemType.WOOD;
        mouseController = GameManager.instance.mouseController;
        UpdateDisplay();
    }
    public void OnButtonLeftClick()
    {
        if(objectType == BuildingType.TABLE)
        {
            mouseController.SetObject(itemType.ToString() + "_" + objectType.ToString() + "_" + additionalInfo, objectType);
        }
        else if (objectType == BuildingType.FLOOR)
        {
            switch (itemType)
            {
                case ItemType.WOOD:
                    mouseController.SetFloor(FloorType.WOOD_FLOOR);
                    break;
                case ItemType.STONE:
                    mouseController.SetFloor(FloorType.STONE_FLOOR);
                    break;
            }
        }
        else
        {
            mouseController.SetObject(itemType.ToString() + "_" + objectType.ToString(), objectType);
        }
    }
    public override void SetType(ItemType type)
    {
        typeMenu.SetActive(false);

        if(type == itemType)
        {
            return;
        }

        itemType = type;
        UpdateDisplay();
        OnButtonLeftClick();
    }
    private void UpdateDisplay()
    {
        if(objectType == BuildingType.TABLE)
        {
            nameText.text = itemType.ToString() + "_" + objectType.ToString() + "_" + additionalInfo;
            image.sprite = atlas.GetSprite(itemType.ToString() + "_" + objectType.ToString() + "_" + additionalInfo);
            return;
        }

        nameText.text = itemType.ToString() + " " + objectType.ToString();
        image.sprite = atlas.GetSprite(itemType.ToString() + "_" + objectType.ToString());
    }
}
