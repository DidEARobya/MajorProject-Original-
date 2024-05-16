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
    public void Build(HashSet<Tile> tiles, BuildMode mode, FurnitureTypes toBuild = null, ItemTypes material = null)
    {
        foreach (Tile tile in tiles)
        {
            Task task;

            if (tile != null && toBuild != null && tile.GetInstalledObject() == null && tile.isPendingTask == false)
            {
                ObjectManager.InstallFurniture(toBuild, material, tile, true);
                //InstalledObject obj = tile.GetInstalledObject();

                //task = new RequirementTask(tile, (t) => { obj.Install(); }, TaskType.CONSTRUCTION, FurnitureTypes.GetRequirements(toBuild), false, FurnitureTypes.GetConstructionTime(toBuild));
                //TaskManager.AddTask(task, task.taskType);
            }
        }  
    }
    public void DestroyObject(HashSet<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            Task task;

            if (tile.GetInstalledObject() != null && tile.GetInstalledObject().isInstalled == true && tile.GetInstalledObject().type == InstalledObjectType.FURNITURE)
            {
                tile.UninstallObject();
                continue;
                task = new DestroyTask(tile, (t) => { tile.UninstallObject(); }, TaskType.CONSTRUCTION, false, tile.GetInstalledObject().durability);
                TaskManager.AddTask(task, task.taskType);
            }
        }
    }
    public void MineOre(HashSet<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            Task task;

            if (tile.GetInstalledObject() != null && tile.GetInstalledObject().isInstalled == true && tile.GetInstalledObject().type == InstalledObjectType.ORE)
            {
                tile.UninstallObject();
                continue;
                task = new DestroyTask(tile, (t) => { tile.UninstallObject(); }, TaskType.CONSTRUCTION, false, tile.GetInstalledObject().durability);
                TaskManager.AddTask(task, task.taskType);
            }
        }
    }
    public void Harvest(HashSet<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            Task task;

            if (tile.GetInstalledObject() != null && tile.GetInstalledObject().isInstalled == true && tile.GetInstalledObject().type == InstalledObjectType.PLANT)
            {
                tile.UninstallObject();
                continue;
                task = new DestroyTask(tile, (t) => { tile.UninstallObject(); }, TaskType.CONSTRUCTION, false, tile.GetInstalledObject().durability);
                TaskManager.AddTask(task, task.taskType);
            }
        }
    }
    public void BuildFloor(HashSet<Tile> tiles, FloorTypes floorType)
    {
        foreach (Tile tile in tiles)
        {
            Task task;

            if (tile != null && tile.IsAccessible() != Accessibility.IMPASSABLE)
            {
                tile.SetFloorType(floorType);

                //task = new RequirementTask(tile, (t) => { tile.SetFloorType(floorType); }, TaskType.CONSTRUCTION, FloorTypes.GetRequirements(FloorTypes.WOOD), true, 0.3f);
                //TaskManager.AddTask(task, task.taskType);
            }
        }
    }
    public void ClearFloor(HashSet<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.floorType == FloorTypes.NONE)
            {
                continue;
            }

            Task task;

            tile.SetFloorType(FloorTypes.NONE);

            //task = new DestroyTask(tile, (t) => { tile.SetFloorType(FloorTypes.NONE); }, TaskType.CONSTRUCTION, true, 50);
            //TaskManager.AddTask(task, task.taskType);
        }
    }
    public void CancelTask(HashSet<Tile> tiles)
    {
        foreach(Tile tile in tiles)
        {
            if (tile.task != null)
            {
                tile.task.CancelTask(false);
            }
        }
    }
    public void SpawnCharacter(HashSet<Tile> tiles)
    {
        foreach(Tile tile in tiles)
        {
            if (tile.IsAccessible() == Accessibility.IMPASSABLE)
            {
                continue;
            }

            CharacterManager.CreateCharacter(tile);
        }
    }
}
