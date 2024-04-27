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
        grid = GameManager.GetWorldGrid();

        InventoryManager.AddToTileInventory(ItemTypes.WOOD, grid.GetTile(10, 10), 2);
        InventoryManager.AddToTileInventory(ItemTypes.WOOD, grid.GetTile(11, 10), 1);
        InventoryManager.AddToTileInventory(ItemTypes.WOOD, grid.GetTile(12, 10), 3);
        InventoryManager.AddToTileInventory(ItemTypes.WOOD, grid.GetTile(13, 10), 3);
        InventoryManager.AddToTileInventory(ItemTypes.WOOD, grid.GetTile(14, 10), 2);
    }
    public void Build(Tile tile, BuildMode mode, InstalledObjectTypes toBuild = null)
    {
        switch(mode)
        {
            case BuildMode.OBJECT:

                if (Input.GetMouseButtonUp(0))
                {
                    if (tile != null && toBuild != null && tile.GetInstalledObject() == null && tile.isPendingTask == false)
                    {
                        ObjectManager.InstallObject(toBuild, tile, false);
                        InstalledObject obj = tile.GetInstalledObject();

                        Task task = new Task(tile, (t) => { obj.Install(); }, TaskType.CONSTRUCTION, ItemTypes.WOOD, 3);

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

            case BuildMode.DESTROY:

                if(tile.GetInstalledObject() != null && tile.GetInstalledObject().isInstalled == true)
                {
                    tile.GetInstalledObject().UnInstall();
                }

                break;
        }    
    }
}
