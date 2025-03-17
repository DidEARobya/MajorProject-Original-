using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class PlantsUIObject : UIObject, IButtonClickHandler
{
    public TileActionsPanel tileActions;

    PlantType plantType;

    // Start is called before the first frame update
    void Start()
    {
        plantType = PlantType.NONE;
        UpdateDisplay();
    }
    public void OnButtonLeftClick()
    {
        tileActions.SetZonePlant(plantType);
    }
    public override void SetType(PlantType type)
    {
        typeMenu.SetActive(false);

        if (type == plantType)
        {
            return;
        }

        plantType = type;
        UpdateDisplay();
        OnButtonLeftClick();
    }
    private void UpdateDisplay()
    {
        if (plantType == PlantType.NONE)
        {
            nameText.text = "Set Plant";
            image.sprite = null;
            return;
        }

        nameText.text = plantType.ToString();
        image.sprite = atlas.GetSprite(nameText.text + "_GROWN");
    }
}
