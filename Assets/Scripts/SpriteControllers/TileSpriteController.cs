using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TileSpriteController : MonoBehaviour
{
    SpriteAtlas terrainSprites;
    SpriteAtlas floorSprites;

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
            return;
        }

        worldController.worldGrid.InvalidatePathGraph();
        renderer.sprite = floorSprites.GetSprite(floor.ToString());
    }
}
