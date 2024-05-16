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
using System.IO;

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
    }
    public void Build(Tile tile, BuildMode mode, FurnitureTypes toBuild = null, FloorTypes floorType = null, ItemTypes material = null, ItemTypes toGenerate = null)
    {
        Task task;

        switch(mode)
        {
            case BuildMode.OBJECT:

                if (tile != null && toBuild != null && tile.GetInstalledObject() == null && tile.isPendingTask == false)
                {
                    ObjectManager.InstallFurniture(toBuild, material, tile, true);
                    //InstalledObject obj = tile.GetInstalledObject();

                    //task = new RequirementTask(tile, (t) => { obj.Install(); }, TaskType.CONSTRUCTION, FurnitureTypes.GetRequirements(toBuild), false, FurnitureTypes.GetConstructionTime(toBuild));
                    //TaskManager.AddTask(task, task.taskType);
                }

                break;

            case BuildMode.FLOOR:

                if(tile != null && tile.IsAccessible() != Accessibility.IMPASSABLE)
                {
                    tile.SetFloorType(floorType);

                    //task = new RequirementTask(tile, (t) => { tile.SetFloorType(floorType); }, TaskType.CONSTRUCTION, FloorTypes.GetRequirements(FloorTypes.WOOD), true, 0.3f);
                    //TaskManager.AddTask(task, task.taskType);
                }

                break;

            case BuildMode.CLEAR_FLOOR:

                if(tile.floorType == FloorTypes.NONE)
                {
                    return;
                }

                tile.SetFloorType(FloorTypes.NONE);

                //task = new DestroyTask(tile, (t) => { tile.SetFloorType(FloorTypes.NONE); }, TaskType.CONSTRUCTION, true, 50);
                //TaskManager.AddTask(task, task.taskType);

                break;

            case BuildMode.DESTROY:

                if(tile.GetInstalledObject() != null && tile.GetInstalledObject().isInstalled == true)
                {
                    if(tile.GetInstalledObject().type == InstalledObjectType.FURNITURE)
                    {
                        tile.UninstallObject();
                        return;
                        task = new DestroyTask(tile, (t) => { tile.UninstallObject(); }, TaskType.CONSTRUCTION, false, tile.GetInstalledObject().durability);
                        TaskManager.AddTask(task, task.taskType);
                    }
                }

                break;

            case BuildMode.GENERATE:

                if (tile != null && toGenerate != null && tile.GetInstalledObject() == null && tile.isPendingTask == false)
                {
                    InventoryManager.AddToTileInventory(toGenerate, tile, 1);
                }

                break;
            case BuildMode.SPAWNCHARACTER:

                if (tile != null && tile.GetInstalledObject() == null && tile.isPendingTask == false)
                {
                    CharacterManager.CreateCharacter(tile);
                }

                break;
            case BuildMode.CANCEL:

                if(tile.task != null)
                {
                    tile.task.CancelTask(false);
                }

                break;
        }    
    }
}
