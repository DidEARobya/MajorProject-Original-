using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class InventorySpriteController : MonoBehaviour
{
    SpriteAtlas itemSprites;

    public GameObject displayPrefab;
    public void AssignAtlas()
    {
        itemSprites = GameManager.instance.itemAtlas;
    }
    public void Init()
    {
        InventoryManager.SetInventoryUpdateCallback(OnInventoryUpdate);
    }

    public void OnInventoryUpdate(Inventory inventory)
    {
        GameObject obj;
        SpriteRenderer renderer;

        Destroy(inventory.inventoryObject);

        if (inventory.item == null)
        {
            return;
        }

        obj = Instantiate(displayPrefab, inventory.owner.gameObj.transform);

        renderer = obj.GetComponent<SpriteRenderer>();
        inventory.inventoryObject = obj;

        obj.name = inventory.item.type + " " + inventory.stackSize;
        obj.transform.SetParent(inventory.owner.gameObj.transform, true);

        renderer.sprite = itemSprites.GetSprite(inventory.item.type.ToString());

        InventoryOwnerType type = inventory.owner.GetOwnerType();
        renderer.sortingLayerName = "Foreground";

        switch (type)
        {
            case InventoryOwnerType.TILE:

                obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.stackSize.ToString();
                obj.GetComponentInChildren<Canvas>().sortingLayerName = "WorldText";
                break;
            case InventoryOwnerType.CHARACTER:
                obj.GetComponentInChildren<TextMeshProUGUI>().text = "";
                break;
        }
    }
}
