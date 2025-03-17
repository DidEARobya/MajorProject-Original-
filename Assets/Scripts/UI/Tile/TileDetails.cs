using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileDetails : MonoBehaviour
{
    public TextMeshProUGUI posText;

    public TextMeshProUGUI terrainText;
    public TextMeshProUGUI floorText;
    public TextMeshProUGUI installedText;
    public TextMeshProUGUI inventoryText;
    public TextMeshProUGUI regionText;

    Tile tile = null;

    public void SetDisplayedTile(Tile _tile)
    {
        if(_tile == tile)
        {
            return;
        }

        if(_tile == null)
        {
            return;
        }

        tile = _tile;

        posText.text = "x: " + tile.x + ", y: " + tile.y;

        terrainText.text = "Terrain: " + tile.terrainType.ToString();
        floorText.text = "Floor: " + tile.floorType.ToString();

        if(tile.IsObjectInstalled() == true)
        {
            installedText.text = "Object: " + tile.installedObject.GetObjectNameToString();
        }
        else
        {
            installedText.text = "Object: " + "NONE";
        }

        if(tile.inventory.item != null)
        {
            inventoryText.text = "Contains: " + tile.inventory.item.type.ToString() + ", " + tile.inventory.stackSize.ToString();
        }
        else
        {
            inventoryText.text = "Empty";
        }
    }
}
