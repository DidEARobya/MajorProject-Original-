using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

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

        int length = grid.mapHeight;

        int x = 10;
        int y = 10;

        for (int _x = -1; _x <= 1; _x++)
        {
            for (int _y = -1; _y <= 1; _y++)
            {
                if (_x == 0 && _y == 0)
                {
                    continue;
                }
                int checkX = x + _x;
                int checkY = y + _y;

                if (checkX >= 0 && checkX < length && checkY >= 0 && checkY < length)
                {
                    if (grid.GetTile(checkX, checkY) != null)
                    {
                        InventoryManager.AddToTileInventory(ItemTypes.WOOD, grid.GetTile(checkX, checkY), 50);
                    }
                }
            }
        }

        InventoryManager.AddToTileInventory(ItemTypes.WOOD, grid.GetTile(x, y), 60);

        ObjectManager.InstallObject(InstalledObjectTypes.WALL, grid.GetTile(15, 15), true);

        InventoryManager.AddToTileInventory(ItemTypes.STONE, grid.GetTile(20, 10), 2);
        InventoryManager.AddToTileInventory(ItemTypes.STONE, grid.GetTile(21, 10), 1);
        InventoryManager.AddToTileInventory(ItemTypes.STONE, grid.GetTile(22, 10), 3);
        InventoryManager.AddToTileInventory(ItemTypes.STONE, grid.GetTile(23, 10), 3);
        InventoryManager.AddToTileInventory(ItemTypes.STONE, grid.GetTile(24, 10), 2);
    }
    public void Build(Tile tile, BuildMode mode, InstalledObjectTypes toBuild = null)
    {
        Task task;

        switch(mode)
        {
            case BuildMode.OBJECT:

                if (Input.GetMouseButtonUp(0))
                {
                    if (tile != null && toBuild != null && tile.GetInstalledObject() == null && tile.isPendingTask == false)
                    {
                        ObjectManager.InstallObject(toBuild, tile, false);
                        InstalledObject obj = tile.GetInstalledObject();

                        task = new RequirementTask(tile, (t) => { obj.Install(); }, TaskType.CONSTRUCTION, InstalledObjectTypes.GetRequirements(toBuild), false);
                        TaskManager.AddTask(task, task.taskType);
                    }
                }

                break;

            case BuildMode.FLOOR:

                task = new RequirementTask(tile, (t) => { tile.SetFloorType(FloorTypes.WOOD); }, TaskType.CONSTRUCTION, FloorTypes.GetRequirements(FloorTypes.WOOD), true, 0.3f);
                TaskManager.AddTask(task, task.taskType);

                break;

            case BuildMode.CLEAR:

                if(tile.floorType == FloorTypes.NONE)
                {
                    return;
                }

                task = new DestroyTask(tile, (t) => { tile.SetFloorType(FloorTypes.NONE); }, TaskType.CONSTRUCTION, true, 50);
                TaskManager.AddTask(task, task.taskType);

                break;

            case BuildMode.DESTROY:

                if(tile.GetInstalledObject() != null && tile.GetInstalledObject().isInstalled == true)
                {
                    task = new DestroyTask(tile, (t) => { tile.UninstallObject(); }, TaskType.CONSTRUCTION, false, tile.GetInstalledObject().durability);
                    TaskManager.AddTask(task, task.taskType);
                }

                break;

            case BuildMode.CANCEL:

                if(tile.task != null)
                {
                    if(tile.task.worker == null)
                    {
                        tile.task.CancelTask(false);
                    }
                    else
                    {
                        tile.task.worker.CancelTask(false);
                    }
                }

                break;

        }    
    }
}
