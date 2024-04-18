using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildModeController : MonoBehaviour
{
    public static BuildModeController instance;

    public WorldGrid grid;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void Init()
    {
        grid = GameManager.GetWorldController().worldGrid;
    }
    public void Build(Tile tile, BuildMode mode)
    {
        switch(mode)
        {
            case BuildMode.OBJECT:

                if (Input.GetMouseButtonUp(0))
                {
                    if (tile != null && tile.isPendingTask == false)
                    {
                        grid.InstallObject(InstalledObjectTypes.WALL, tile);
                        InstalledObject obj = tile.GetInstalledObject();

                        Task task = new Task(tile, (t) => { obj.Install(); }, TaskType.CONSTRUCTION);

                        TaskManager.AddTask(task, task.taskType);
                    }
                }
                break;

            case BuildMode.FLOOR:

                tile.SetFloorType(FloorTypes.WOOD);
                break;

            case BuildMode.CLEAR:

                tile.SetFloorType(FloorTypes.NONE);
                break;
        }    
    }
}
