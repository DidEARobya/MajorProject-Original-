using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldController : MonoBehaviour
{
    public WorldGrid worldGrid;
    public MouseController mouseController;

    public new Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }
    public void Init(TileSpriteController tileSpriteController)
    {
        worldGrid = new WorldGrid();
        camera.transform.position = new Vector3(worldGrid.mapWidth / 2, worldGrid.mapHeight / 2, -10);

        for (int x = 0; x < worldGrid.mapWidth; x++)
        {
            for (int y = 0; y < worldGrid.mapHeight; y++)
            {
                Tile tileData = worldGrid.GetTile(x, y);
                tileData.SetNeighbours();
                Vector2 tilePos = new Vector2(tileData.x, tileData.y);

                GameObject tileObj = new GameObject();
                tileObj.name = "Tile: " + x + "_" + y;
                tileObj.transform.SetParent(tileSpriteController.transform, true);
                tileObj.transform.position = tilePos;

                SpriteRenderer renderer = tileObj.AddComponent<SpriteRenderer>();
                renderer.sortingLayerName = "Ground";

                tileData.SetGameObject(tileObj);
                tileData.SetTileChangedCallback((tile) => { tileSpriteController.OnTileTypeChange(tile, tileObj); });

                tileSpriteController.SetTileSprite(tileObj, TerrainTypes.GetTerrainType(tileData.terrainType), FloorTypes.GetFloorType(tileData.floorType));

            }
        }
    }
    public void GenerateTerrain()
    {
        bool isSpawned = false;
        int xCentre = worldGrid.mapWidth / 2;
        int yCentre = worldGrid.mapHeight / 2;

        int[,] caValues = worldGrid.cellularAutomataValues;

        for (int x = 0; x < worldGrid.mapWidth; x++)
        {
            for (int y = 0; y < worldGrid.mapHeight; y++)
            {
                if (caValues[x, y] == 0)
                {
                    int rand = Utility.GetRandomNumber(0, 100);

                    if(rand < 70)
                    {
                        ObjectManager.SpawnOre(OreTypes.STONE_ORE, worldGrid.GetTile(x, y));
                    }
                    else
                    {
                        ObjectManager.SpawnOre(OreTypes.IRON_ORE, worldGrid.GetTile(x, y));
                    }
                }

                if(isSpawned == false)
                {
                    if(x >= xCentre - 5 && x <= xCentre + 5 && y >= yCentre - y && y <= xCentre + y)
                    {
                        CharacterManager.CreateCharacter(worldGrid.GetTile(x, y));
                        isSpawned = true;
                    }
                }
            }
        }
 
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
