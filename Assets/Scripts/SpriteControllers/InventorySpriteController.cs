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

        obj.name = ItemTypes.GetItemType(inventory.item) + " " + inventory.stackSize;
        obj.transform.SetParent(inventory.owner.gameObj.transform, true);

        renderer.sprite = itemSprites.GetSprite(ItemTypes.GetItemType(inventory.item).ToString());

        InventoryOwnerType type = inventory.owner.GetOwnerType();
        
        switch(type)
        {
            case InventoryOwnerType.TILE:
                renderer.sortingLayerName = "Item";
                obj.GetComponentInChildren<TextMeshProUGUI>().text = inventory.stackSize.ToString();
                break;
            case InventoryOwnerType.CHARACTER:
                renderer.sortingLayerName = "Held";
                obj.GetComponentInChildren<TextMeshProUGUI>().text = "";
                break;
        }
    }
}
