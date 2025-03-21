using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class WorldController : MonoBehaviour
{
    public WorldGrid worldGrid;
    public MouseController mouseController;

    public new Camera camera;
    public Cinemachine.CinemachineVirtualCamera vCamera;

    public GameObject cameraBounds;

    private void Awake()
    {
        camera = Camera.main;
    }
    public void Init(TileSpriteController tileSpriteController)
    {
        worldGrid = new WorldGrid();
        camera.transform.position = new Vector3(worldGrid.mapSize / 2, worldGrid.mapSize / 2, -10);

        cameraBounds.transform.position = new Vector3(worldGrid.mapSize / 2, (worldGrid.mapSize / 2) - 0.5f, 0);
        cameraBounds.transform.localScale = new Vector3(worldGrid.mapSize, worldGrid.mapSize + 1, 1);

        for (int x = 0; x < worldGrid.mapSize; x++)
        {
            for (int y = 0; y < worldGrid.mapSize; y++)
            {
                Tile tileData = worldGrid.GetTile(x, y);
                tileData.SetNeighbours();
                Vector3 tilePos = new Vector2(tileData.x, tileData.y);

                GameObject tileObj = new GameObject();
                tileObj.name = "Tile: " + x + "_" + y;
                tileObj.transform.SetParent(tileSpriteController.transform, true);
                tileObj.transform.position = tilePos;

                SpriteRenderer renderer = tileObj.AddComponent<SpriteRenderer>();
                renderer.sortingLayerName = "Background";

                tileData.SetGameObject(tileObj);
                tileData.SetTileChangedCallback((tile) => { tileSpriteController.OnTileTypeChange(tile, tileObj); });

                tileSpriteController.SetTileSprite(tileObj, tileData.terrainType, tileData.floorType);
            }
        }
    }
    public void GenerateTerrain()
    {
        int[,] caValues = worldGrid.cellularAutomataValues;

        for (int x = 0; x < worldGrid.mapSize; x++)
        {
            for (int y = 0; y < worldGrid.mapSize; y++)
            {
                int rand = Utility.GetRandomInt(0, 100);

                if (caValues[x, y] == 0)
                {
                    if(rand < 70)
                    {
                        ObjectManager.SpawnOre(OreType.STONE_ORE, worldGrid.GetTile(x, y));
                    }
                    else
                    {
                        ObjectManager.SpawnOre(OreType.IRON_ORE, worldGrid.GetTile(x, y));
                    }
                }
                else
                {
                    Tile tile = worldGrid.GetTile(x, y);

                    if(rand < ThingsDataHandler.GetTerrainData(tile.terrainType).randomPlantGrowthChance)
                    {
                        PlantState state = (PlantState)Utility.GetRandomInt(0, 4);
                        ObjectManager.SpawnPlant(PlantType.OAK_TREE, tile, state);
                    }
                }
            }
        }

        GameManager.GetRegionManager().UpdateMaps();
    }
    public Vector2 GetWorldToCell(Vector2 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);

        return new Vector2(x, y);
    }
    public Vector2 GetWorldToCellCentre(Vector2 pos)
    {
        int x = Mathf.FloorToInt(pos.x + 0.5f);
        int y = Mathf.FloorToInt(pos.y + 0.5f);

        return new Vector2(x, y);
    }
}
