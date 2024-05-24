using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool devMode = false;

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

    [SerializeField]
    public WorldController worldController;
    [SerializeField]
    public TileSpriteController tileSpriteController;
    [SerializeField]
    public InstalledSpriteController installedSpriteController;
    [SerializeField]
    public CharacterSpriteController characterSpriteController;
    [SerializeField]
    public InventorySpriteController inventorySpriteController;
    [SerializeField]
    public BuildModeController buildModeController;
    [SerializeField]
    public MouseController mouseController;

    TaskManager taskManager;
    TaskRequestHandler taskRequestHandler;

    RegionManager regionManager;

    ZoneManager zoneManager;

    [SerializeField]
    public SpriteAtlas terrainAtlas;
    [SerializeField]
    public SpriteAtlas floorAtlas;
    [SerializeField]
    public SpriteAtlas itemAtlas;
    [SerializeField]
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

        taskManager = new TaskManager();
        taskManager.Init();
        taskRequestHandler = new TaskRequestHandler();

        inventorySpriteController.Init();
        tileSpriteController.Init();
        installedSpriteController.Init();
        characterSpriteController.Init();

        mouseController.Init(worldGrid);
        buildModeController.Init();

        regionManager = new RegionManager();
        regionManager.Init(worldGrid, regionMapSize);

        zoneManager = new ZoneManager();
        zoneManager.Init();

        worldController.GenerateTerrain();
    }
    private void Update()
    {
        ObjectManager.Update(Time.deltaTime);
        CharacterManager.Update(Time.deltaTime);
        taskRequestHandler.Update();
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
    public static TaskManager GetTaskManager()
    {
        return instance.taskManager;
    }
    public static TaskRequestHandler GetTaskRequestHandler()
    {
        return instance.taskRequestHandler;
    }
    public static RegionManager GetRegionManager()
    {
        return instance.regionManager;
    }
    public static ZoneManager GetZoneManager()
    {
        return instance.zoneManager;
    }
}
