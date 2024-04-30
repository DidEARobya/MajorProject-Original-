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

        int length = grid.mapWidth;
        int halfLength = grid.worldCentre.x;
        int offset = 5;

        int spawnX = halfLength;
        int spawnY = halfLength;


        for (int _x = -1; _x <= 1; _x++)
        {
            for (int _y = -1; _y <= 1; _y++)
            {
                int checkX = spawnX + _x;
                int checkY = spawnY + _y;

                if (checkX >= 0 && checkX < length && checkY >= 0 && checkY < length)
                {
                    if (grid.GetTile(checkX, checkY) != null)
                    {
                        InventoryManager.AddToTileInventory(ItemTypes.WOOD, grid.GetTile(checkX - offset, checkY), 50);
                        InventoryManager.AddToTileInventory(ItemTypes.STONE, grid.GetTile(checkX + offset, checkY), 50);
                        InventoryManager.AddToTileInventory(ItemTypes.IRON, grid.GetTile(checkX, checkY + offset), 50);

                        ObjectManager.SpawnOre(OreTypes.STONE_ORE, grid.GetTile(checkX - offset, checkY - offset));
                        ObjectManager.SpawnOre(OreTypes.IRON_ORE, grid.GetTile(checkX + offset, checkY + offset));
                    }
                }
            }
        }
    }
    public void Build(Tile tile, BuildMode mode, FurnitureTypes toBuild = null)
    {
        Task task;

        switch(mode)
        {
            case BuildMode.OBJECT:

                if (Input.GetMouseButtonUp(0))
                {
                    if (tile != null && toBuild != null && tile.GetInstalledObject() == null && tile.isPendingTask == false)
                    {
                        ObjectManager.InstallFurniture(toBuild, tile, false);
                        InstalledObject obj = tile.GetInstalledObject();

                        task = new RequirementTask(tile, (t) => { obj.Install(); }, TaskType.CONSTRUCTION, FurnitureTypes.GetRequirements(toBuild), false, FurnitureTypes.GetConstructionTime(toBuild));
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
                    if(tile.GetInstalledObject().type == InstalledObjectType.FURNITURE)
                    {
                        task = new DestroyTask(tile, (t) => { tile.UninstallObject(); }, TaskType.CONSTRUCTION, false, tile.GetInstalledObject().durability);
                        TaskManager.AddTask(task, task.taskType);
                    }
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
                        tile.task.worker.CancelTask(false, tile.task);
                    }
                }

                break;

        }    
    }
}
