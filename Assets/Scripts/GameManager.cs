using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public WorldController worldController;
    public TileSpriteController tileSpriteController;
    public InstalledSpriteController installedSpriteController;
    public CharacterSpriteController characterSpriteController;
    public InventorySpriteController inventorySpriteController;
    public BuildModeController buildModeController;
    public MouseController mouseController;

    WorldGrid worldGrid;

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
        worldController.Init(tileSpriteController);
        worldGrid = worldController.worldGrid;

        inventorySpriteController.Init();
        tileSpriteController.Init();
        installedSpriteController.Init();
        characterSpriteController.Init();

        mouseController.Init(worldGrid);
        buildModeController.Init();
        TaskManager.Init();
    }
    private void Update()
    {
        ObjectManager.Update(Time.deltaTime);
        CharacterManager.Update(Time.deltaTime);
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
