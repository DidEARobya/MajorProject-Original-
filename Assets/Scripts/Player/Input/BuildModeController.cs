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
using System.Linq;

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
    public void Build(HashSet<Tile> tiles, BuildMode mode, Direction rotation, string toBuild = "")
    {
        foreach (Tile tile in tiles)
        {
            //Task task;

            if (Utility.IsValidTile(tile) /*tile != null && toBuild != null && tile.GetInstalledObject() == null && tile.isPendingTask == false*/)
            {
                if(GameManager.instance.devMode == true)
                {
                    ObjectManager.InstallBuilding(toBuild, tile, true, rotation);
                    continue;
                }

                ObjectManager.InstallBuilding(toBuild, tile, false, rotation);
                Building obj = tile.GetBuilding();

                if(obj == null)
                {
                    return;
                }

                ConstructionSite site = new ConstructionSite(obj.GetTiles().ToList(), obj._data, (t) => { obj.Install(); });
                GameManager.GetTaskManager().AddTaskSite(site, TaskType.CONSTRUCTION);

                //task = new RequirementTask(tile, (t) => { obj.Install(); }, TaskType.CONSTRUCTION, ThingsDataHandler.GetBuildingData(toBuild).GetRequirements(), false);
               // GameManager.GetTaskManager().AddTask(task, task.taskType);
            }
        }  
    }
    public void DestroyObject(HashSet<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            Task task;

            if (tile.IsObjectInstalled() == true && tile.installedObject.type == InstalledObjectType.BUILDING)
            {
                if (GameManager.instance.devMode == true)
                {
                    tile.UninstallObject();
                    continue;
                }

                //task = new DestroyTask(tile, (t) => { tile.UninstallObject(); }, TaskType.CONSTRUCTION, false, tile.installedObject.durability);
                //GameManager.GetTaskManager().AddTask(task, task.taskType);
            }
        }
    }
    public void MineOre(HashSet<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            Task task;

            if (tile.IsObjectInstalled() == true && tile.installedObject.type == InstalledObjectType.ORE)
            {
                if (GameManager.instance.devMode == true)
                {
                    tile.UninstallObject();
                    continue;
                }

                //task = new DestroyTask(tile, (t) => { tile.UninstallObject(); }, TaskType.MINING, false, tile.installedObject.durability);
                //GameManager.GetTaskManager().AddTask(task, task.taskType);
            }
        }
    }
    public void Harvest(HashSet<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            Task task;

            if (tile.IsObjectInstalled() == true && tile.installedObject.type == InstalledObjectType.PLANT)
            {
                if (GameManager.instance.devMode == true)
                {
                    tile.UninstallObject();
                    continue;
                }

                //task = new DestroyTask(tile, (t) => { tile.UninstallObject(); }, TaskType.AGRICULTURE, false, tile.installedObject.durability);
                //GameManager.GetTaskManager().AddTask(task, task.taskType);
            }
        }
    }
    public void BuildFloor(HashSet<Tile> tiles, FloorType floorType)
    {
        foreach (Tile tile in tiles)
        {
            Task task;

            if (tile != null && tile.IsAccessible() != Accessibility.IMPASSABLE)
            {
                if (GameManager.instance.devMode == true)
                {
                    tile.SetFloorType(floorType);
                    continue;
                }

                //task = new RequirementTask(tile, (t) => { tile.SetFloorType(floorType); }, TaskType.CONSTRUCTION, ThingsDataHandler.GetFloorData(floorType).GetRequirements(), true, 0.3f);
                //GameManager.GetTaskManager().AddTask(task, task.taskType);
            }
        }
    }
    public void ClearFloor(HashSet<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.floorType == FloorType.NONE)
            {
                continue;
            }

            if (GameManager.instance.devMode == true)
            {
                tile.SetFloorType(FloorType.NONE);
                continue;
            }

            Task task;

            //task = new DestroyTask(tile, (t) => { tile.SetFloorType(FloorType.NONE); }, TaskType.CONSTRUCTION, true, 50);
            //GameManager.GetTaskManager().AddTask(task, task.taskType);
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
