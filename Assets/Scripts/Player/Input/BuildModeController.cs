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

                BuildingData data = ThingsDataHandler.GetBuildingData(toBuild);

                HaulSite site = new HaulSite(obj.GetTiles().ToList(), data.GetRequirements(), () => { new ConstructionSite(obj.GetTiles().ToList(), data, () => obj.Install()); });
            }
        }  
    }
    public void DestroyObject(HashSet<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.IsObjectInstalled() == true && tile.installedObject.type == InstalledObjectType.BUILDING && tile.site == null)
            {
                if (GameManager.instance.devMode == true)
                {
                    tile.UninstallObject();
                    continue;
                }

                DestructionSite site = new DestructionSite(tile, TaskType.CONSTRUCTION, () => { tile.UninstallObject(); });
            }
        }
    }
    public void MineOre(HashSet<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.IsObjectInstalled() == true && tile.installedObject.type == InstalledObjectType.ORE)
            {
                if (GameManager.instance.devMode == true)
                {
                    tile.UninstallObject();
                    continue;
                }

                DestructionSite site = new DestructionSite(tile, TaskType.MINING, () => { tile.UninstallObject(); });
            }
        }
    }
    public void Harvest(HashSet<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            if (tile.IsObjectInstalled() == true && tile.installedObject.type == InstalledObjectType.PLANT)
            {
                if (GameManager.instance.devMode == true)
                {
                    tile.UninstallObject();
                    continue;
                }

                DestructionSite site = new DestructionSite(tile, TaskType.AGRICULTURE, () => { tile.UninstallObject(); });
            }
        }
    }
    public void BuildFloor(HashSet<Tile> tiles, FloorType floorType)
    {
        foreach (Tile tile in tiles)
        {
            if (tile != null && tile.IsAccessible() != Accessibility.IMPASSABLE)
            {
                if (GameManager.instance.devMode == true)
                {
                    tile.SetFloorType(floorType);
                    continue;
                }

                FloorData data = ThingsDataHandler.GetFloorData(floorType);
                HaulSite site = new HaulSite(tile, data.GetRequirements(), () => { new ConstructionSite(tile, data, () => tile.SetFloorType(floorType)); });
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

            DestructionSite site = new DestructionSite(tile, TaskType.CONSTRUCTION, () => { tile.SetFloorType(FloorType.NONE); }, true);
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
