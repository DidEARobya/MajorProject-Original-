using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileActionsPanel : MonoBehaviour
{
    Tile tile;

    public GameObject setPlantPanel;
    public GameObject removeGrowZonePanel;
    public GameObject removeStorageZonePanel;

    PlantTypes plantType;

    public void Init(Tile _tile)
    {
        setPlantPanel.SetActive(false);
        removeGrowZonePanel.SetActive(false);
        removeStorageZonePanel.SetActive(false);

        if(_tile == null)
        {
            return;
        }

        tile = _tile;

        if(tile.zone != null)
        {
            gameObject.SetActive(true);

            setPlantPanel.SetActive(tile.zone.zoneType == ZoneType.GROW);
            removeGrowZonePanel.SetActive(tile.zone.zoneType == ZoneType.GROW);
            removeStorageZonePanel.SetActive(tile.zone.zoneType == ZoneType.STORAGE);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void RemoveFromZone()
    {
        if(tile == null)
        {
            return;
        }

        GameManager.GetZoneManager().RemoveTile(tile, tile.zone.zoneType);
    }
    public void SetZonePlant(PlantTypes type)
    {
        if (tile == null || tile.zone == null || tile.zone.zoneType != ZoneType.GROW)
        {
            return;
        }

        plantType = type;
        (tile.zone as GrowZone).SetToGrow(plantType);
    }
}
