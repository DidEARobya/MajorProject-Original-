using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UIObject : MonoBehaviour
{
    public GameObject typeMenu;

    public Image image;
    public TextMeshProUGUI nameText;

    public SpriteAtlas atlas;

    public void OnButtonRightClick()
    {
        DisplayMaterialMenu();
    }
    public virtual void SetType(ItemType type)
    {

    }
    public virtual void SetType(PlantType type)
    {

    }
    protected void DisplayMaterialMenu()
    {
        typeMenu.GetComponent<MenuDropdown>().uiObject = this;
        typeMenu.transform.position = transform.position + new Vector3(0, 150, 0);
        typeMenu.SetActive(true);
    }
}
