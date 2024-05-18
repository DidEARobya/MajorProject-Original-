using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class TileSpriteController : MonoBehaviour
{
    SpriteAtlas terrainSprites;
    SpriteAtlas floorSprites;

    public GameObject overlay;
    public GameObject zoneParent;
    public GameObject selectedParent;

    WorldController worldController;
    public void AssignAtlas()
    {
        terrainSprites = GameManager.instance.terrainAtlas;
        floorSprites = GameManager.instance.floorAtlas;
    }
    public void Init()
    {
        worldController = GameManager.GetWorldController();
    }
    public void SetTileSprite(GameObject tile, TerrainType type, FloorType floor)
    {
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

        if (floor == FloorType.NONE)
        {
            renderer.sprite = terrainSprites.GetSprite(type.ToString());

            return;
        }

        renderer.sprite = floorSprites.GetSprite(floor.ToString());
    }

    public void OnTileTypeChange(Tile tileData, GameObject tileObj)
    {
        TerrainType type = TerrainTypes.GetTerrainType(tileData.terrainType);
        FloorType floor = FloorTypes.GetFloorType(tileData.floorType);

        SpriteRenderer renderer = tileObj.GetComponent<SpriteRenderer>();

        if (floor == FloorType.NONE)
        {
            renderer.sprite = terrainSprites.GetSprite(type.ToString());
        }
        else
        {
            worldController.worldGrid.InvalidatePathGraph();
            renderer.sprite = floorSprites.GetSprite(floor.ToString());
        }

        if (tileData == null || tileObj == null)
        {
            return;
        }

        SetZoneVisual(tileData, tileObj);
        SetSelectedVisual(tileData, tileObj);
    }

    void SetZoneVisual(Tile tileData, GameObject tileObj)
    {
        if (tileData.zone == null)
        {
            if (tileData.zoneObj != null)
            {
                Destroy(tileData.zoneObj);
            }

            return;
        }

        SpriteRenderer zoneRenderer;

        if (tileData.zoneObj == null)
        {
            GameObject obj = Instantiate(overlay);
            obj.transform.SetParent(zoneParent.transform, true);
            obj.transform.position = tileObj.transform.position;

            tileData.zoneObj = obj;
        }

        zoneRenderer = tileData.zoneObj.GetComponent<SpriteRenderer>();
        zoneRenderer.sortingLayerName = "Zones";
        zoneRenderer.color = tileData.zone.zoneColour;
    }

    void SetSelectedVisual(Tile tileData, GameObject tileObj)
    {
        if (tileData.isSelected == false)
        {
            if (tileData.selectedObj != null)
            {
                tileData.selectedObj.SetActive(false);
            }

            return;
        }

        SpriteRenderer selectedRenderer;

        if (tileData.selectedObj == null)
        {
            GameObject obj = Instantiate(overlay);
            obj.transform.SetParent(selectedParent.transform, true);
            obj.transform.position = tileObj.transform.position;

            tileData.selectedObj = obj;

            selectedRenderer = tileData.selectedObj.GetComponent<SpriteRenderer>();
            selectedRenderer.sortingLayerName = "Zones";

            Color colour = Color.blue;
            colour.a = 0.3f;
            selectedRenderer.color = colour;
        }

        tileData.selectedObj.SetActive(true);
    }
}
