using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class StructuresUIObject : UIObject, IButtonClickHandler
{
    public enum UIObjectType
    {
        WALL,
        DOOR,
        FLOOR
    }

    MouseController mouseController;
    public UIObjectType objectType;

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
            case UIObjectType.WALL:
            mouseController.SetObject(itemType.ToString() + "_WALL", MouseMode.ROW);
            break;

           case UIObjectType.DOOR:
            mouseController.SetObject(itemType.ToString() + "_DOOR", MouseMode.SINGLE);
            break;

            case UIObjectType.FLOOR:

                //switch(itemType)
                //{
                //    case ItemType.WOOD:
                //        mouseController.SetFloor(FloorTypes.WOOD, itemType, MouseMode.AREA);
                //        break;
                //    case ItemType.STONE:
                //        mouseController.SetFloor(FloorTypes.STONE, itemType, MouseMode.AREA);
                //        break;
                //}

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
