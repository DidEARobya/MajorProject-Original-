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
    public TaskSpriteController taskSpriteController;
    public BuildModeController buildModeController;

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
        tileSpriteController.Init();
        installedSpriteController.Init();
        characterSpriteController.Init();
        taskSpriteController.Init();
        buildModeController.Init();
        TaskManager.Init();
    }
    public static WorldController GetWorldController()
    {
        return instance.worldController;
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
    public static TaskSpriteController GetTaskSpriteController()
    {
        return instance.taskSpriteController;
    }
}
