using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.WSA;

public class TileSpriteController : MonoBehaviour
{
    SpriteAtlas terrainSprites;
    SpriteAtlas floorSprites;

    public GameObject overlay;
    public GameObject zoneParent;
    public GameObject selectedParent;
    public GameObject regionParent;
    public GameObject taskFloorObject;

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
        TerrainType type = tileData.terrainType;
        FloorType floor = tileData.floorType;

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
        SetRegionVisual(tileData, tileObj);
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
                Destroy(tileData.selectedObj);
                tileData.selectedObj = null;
            }

            return;
        }

        if (tileData.selectedObj == null)
        {
            GameObject obj = Instantiate(overlay);
            obj.transform.SetParent(selectedParent.transform, true);
            obj.transform.position = tileObj.transform.position;

            tileData.selectedObj = obj;
        }

        SpriteRenderer selectedRenderer;

        selectedRenderer = tileData.selectedObj.GetComponent<SpriteRenderer>();
        selectedRenderer.sortingLayerName = "Zones";

        Color colour = Color.blue;
        colour.a = 0.3f;
        selectedRenderer.color = colour;
    }

    void SetRegionVisual(Tile tileData, GameObject tileObj)
    {
        if(tileData.region == null)
        {
            tileData.displayRegion = false;
        }    

        if (tileData.displayRegion == false)
        {
            if (tileData.regionDisplayObj != null)
            {
                Destroy(tileData.regionDisplayObj);
                tileData.regionDisplayObj = null;
            }

            return;
        }

        if (tileData.regionDisplayObj == null)
        {
            GameObject obj = Instantiate(overlay);
            obj.transform.SetParent(regionParent.transform, true);
            obj.transform.position = tileObj.transform.position;

            tileData.regionDisplayObj = obj;
        }

        SpriteRenderer selectedRenderer;

        selectedRenderer = tileData.regionDisplayObj.GetComponent<SpriteRenderer>();
        selectedRenderer.sortingLayerName = "Zones";

        Color colour = tileData.regionColour;
        colour.a = 0.1f;
        selectedRenderer.color = colour;
    }

    public Sprite GetFloorSprite(FloorType type)
    {
        return floorSprites.GetSprite(type.ToString());
    }
}
