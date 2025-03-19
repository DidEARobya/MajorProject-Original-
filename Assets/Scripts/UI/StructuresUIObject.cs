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

    // Start is called before the first frame update
    void Start()
    {
        itemType = ItemType.WOOD;
        mouseController = GameManager.instance.mouseController;
        UpdateDisplay();
    }
    public void OnButtonLeftClick()
    {
        switch(objectType)
        {
            case BuildingType.WALL:
            mouseController.SetObject(itemType.ToString() + "_" + objectType.ToString(), MouseMode.ROW);
            break;

           case BuildingType.DOOR:
            mouseController.SetObject(itemType.ToString() + "_" + objectType.ToString(), MouseMode.SINGLE);
            break;

            case BuildingType.FLOOR:

                switch(itemType)
                {
                    case ItemType.WOOD:
                        mouseController.SetFloor(FloorType.WOOD_FLOOR, MouseMode.AREA);
                        break;
                    case ItemType.STONE:
                        mouseController.SetFloor(FloorType.STONE_FLOOR, MouseMode.AREA);
                        break;
                }

                break;

            case BuildingType.BED:
                mouseController.SetObject(itemType.ToString() + "_" + objectType.ToString(), MouseMode.SINGLE);
                break;
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
        nameText.text = itemType.ToString() + " " + objectType.ToString();
        image.sprite = atlas.GetSprite(itemType.ToString() + "_" + objectType.ToString());
    }
}
