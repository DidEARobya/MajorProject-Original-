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

        terrainText.text = "Terrain: " + TerrainTypes.GetTerrainType(tile.terrainType).ToString();
        floorText.text = "Floor: " + FloorTypes.GetFloorType(tile.floorType).ToString();

        if(tile.IsObjectInstalled() == true)
        {
            installedText.text = "Object: " + tile.installedObject.GetObjectType();
        }
        else
        {
            installedText.text = "Object: " + "NONE";
        }

        if(tile.inventory.item != null)
        {
            inventoryText.text = "Contains: " + ItemTypes.GetItemType(tile.inventory.item).ToString() + ", " + tile.inventory.stackSize.ToString();
        }
        else
        {
            inventoryText.text = "Empty";
        }
    }
}
