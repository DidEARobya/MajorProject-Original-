using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class StructuresUIObject : MonoBehaviour, IButtonClickHandler
{
    public enum UIObjectType
    {
        WALL,
        DOOR,
        FLOOR
    }

    private new Camera camera;
    public GameObject typeMenu;

    public Image image;
    public TextMeshProUGUI nameText;

    MouseController mouseController;

    public SpriteAtlas atlas;
    public UIObjectType objectType;

    ItemTypes itemType;
    FurnitureTypes furnitureTypes;

    // Start is called before the first frame update
    void Start()
    {
        itemType = ItemTypes.WOOD;
        mouseController = GameManager.instance.mouseController;
        UpdateDisplay();
    }
    public void OnButtonLeftClick()
    {
        switch(objectType)
        {
            case UIObjectType.WALL:
            mouseController.SetObject(FurnitureTypes.WALL, itemType, MouseMode.ROW);
            break;

            case UIObjectType.DOOR:
            mouseController.SetObject(FurnitureTypes.DOOR, itemType, MouseMode.SINGLE);
            break;

            case UIObjectType.FLOOR:

                ItemType type = ItemTypes.GetItemType(itemType);

                switch(type)
                {
                    case ItemType.WOOD:
                        mouseController.SetFloor(FloorTypes.WOOD, itemType, MouseMode.AREA);
                        break;
                    case ItemType.STONE:
                        mouseController.SetFloor(FloorTypes.TASK, itemType, MouseMode.AREA);
                        break;
                }

            break;
        }
    }
    public void OnButtonRightClick()
    {
        DisplayMaterialMenu();
    }
    public void SetType(ItemTypes type)
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
        nameText.text = ItemTypes.GetItemType(itemType).ToString() + " " + objectType.ToString();
        image.sprite = atlas.GetSprite(ItemTypes.GetItemType(itemType).ToString() + "_" + objectType.ToString());
    }
    private void DisplayMaterialMenu()
    {
        typeMenu.GetComponent<MenuDropdown>().uiObject = this;
        typeMenu.transform.position = transform.position + new Vector3(0, 150, 0);
        typeMenu.SetActive(true);
    }
}
