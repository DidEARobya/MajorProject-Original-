using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int mapSize;
    int regionMapSize
    {
        get 
        { 
            if(mapSize % 10 != 0)
            {
                Debug.Log("Invalid Map Size");
            }

            return mapSize / 10; 
        }
    }

    public int octaves;
    public float terrainScale;
    public int terrainOffset;
    public float persistence;
    public float lacunarity;

    public int caIterations;
    public int noiseDensity;
    public int seed;

    public System.Random random;

    public WorldController worldController;
    public TileSpriteController tileSpriteController;
    public InstalledSpriteController installedSpriteController;
    public CharacterSpriteController characterSpriteController;
    public InventorySpriteController inventorySpriteController;
    public BuildModeController buildModeController;
    public MouseController mouseController;

    public SpriteAtlas terrainAtlas;
    public SpriteAtlas floorAtlas;
    public SpriteAtlas itemAtlas;
    public SpriteAtlas objectAtlas;

    WorldGrid worldGrid;

    public GameObject menu;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random(seed);

        inventorySpriteController.AssignAtlas();
        tileSpriteController.AssignAtlas();
        installedSpriteController.AssignAtlas();

        worldController.Init(tileSpriteController);
        worldGrid = worldController.worldGrid;

        TaskManager.Init();

        inventorySpriteController.Init();
        tileSpriteController.Init();
        installedSpriteController.Init();
        characterSpriteController.Init();

        mouseController.Init(worldGrid);
        buildModeController.Init();

        RegionManager.Init(worldGrid, regionMapSize);
        worldController.GenerateTerrain();
    }
    private void Update()
    {
        ObjectManager.Update(Time.deltaTime);
        CharacterManager.Update(Time.deltaTime);
        TaskRequestHandler.Update();
        PathRequestHandler.Update();
    }
    public static WorldController GetWorldController()
    {
        return instance.worldController;
    }
    public static WorldGrid GetWorldGrid()
    {
        return instance.worldGrid;
    }
    public static TileSpriteController GetTileSpriteController()
    {
        return instance.tileSpriteController;
    }
    public static InstalledSpriteController GetInstalledSpriteController()
    {
        return instance.installedSpriteController;
    }
    public static CharacterSpriteController GetCharacterSpriteController()
    {
        return instance.characterSpriteController;
    }
}
