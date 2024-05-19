using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class PlantsUIObject : UIObject, IButtonClickHandler
{
    public TileActionsPanel tileActions;

    PlantTypes plantType;

    // Start is called before the first frame update
    void Start()
    {
        plantType = null;
        UpdateDisplay();
    }
    public void OnButtonLeftClick()
    {
        tileActions.SetZonePlant(plantType);
    }
    public override void SetType(PlantTypes type)
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
        if (plantType == null)
        {
            nameText.text = "Set Plant";
            image.sprite = null;
            return;
        }

        nameText.text = PlantTypes.GetObjectType(plantType).ToString();
        image.sprite = atlas.GetSprite(nameText.text + "_GROWN");
    }
}
