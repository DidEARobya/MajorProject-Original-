using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : InstalledObject
{
    public ItemData baseMaterial;
    public BuildingData _data;

    private HashSet<Tile> _tiles = new HashSet<Tile>();
    private List<Building> _neighbourBuildings = new List<Building>();

    public Direction _rotation;
    static public Building PlaceObject(BuildingData data, Tile tile, bool _isInstalled, Direction rotation)
    {
        if(data == null)
        {
            Debug.LogError("FAILED TO FIND BUILDINGDATA");
            return null;
        }

        Building obj = new Building();
        obj._rotation = rotation;

        if(data.canRotate == false || (rotation == Direction.N || rotation == Direction.S))
        {
            for (int x = tile.x; x < (tile.x + data.width); x++)
            {
                for (int y = tile.y; y < (tile.y + data.height); y++)
                {
                    Tile t = GameManager.GetWorldGrid().GetTile(x, y);

                    if (Utility.IsValidTile(t) == false || t.PlaceObject(obj) == false)
                    {
                        foreach (Tile t2 in obj._tiles)
                        {
                            t2.ClearInstalledObject();
                        }

                        return null;
                    }

                    obj._tiles.Add(t);
                }
            }
        }
        else
        {
            for (int x = tile.x; x < (tile.x + data.height); x++)
            {
                for (int y = tile.y; y < (tile.y + data.width); y++)
                {
                    Tile t = GameManager.GetWorldGrid().GetTile(x, y);

                    if (Utility.IsValidTile(t) == false || t.PlaceObject(obj) == false)
                    {
                        foreach (Tile t2 in obj._tiles)
                        {
                            t2.ClearInstalledObject();
                        }

                        return null;
                    }

                    obj._tiles.Add(t);
                }
            }
        }

        obj.baseTile = tile;
        obj.type = InstalledObjectType.BUILDING;
        obj.durability = data.durability;
        obj.canRotate = data.canRotate;
        obj._data = data;

        if (_isInstalled == true)
        {
            obj.Install();
        }

        return obj;
    }
    public override void Install()
    {
        base.Install();

        foreach(Tile tile in _tiles)
        {
            tile.InstallObject();
        }

        GameManager.GetRegionManager().UpdateCluster(GameManager.GetRegionManager().GetClusterAtTile(baseTile), baseTile, (_tiles.Count > 1));

        isInstalled = true;

        if (_data.baseAccessibility == Accessibility.DELAYED)
        {
            AddOnActionCallback(InstalledObjectAction.Door_UpdateAction);
        }

        if (updateObjectCallback != null)
        {
            updateObjectCallback(this);
        }

        GameManager.GetWorldGrid().InvalidatePathGraph();
    }
    public override void UnInstall()
    {
        base.UnInstall();

        if (_data.baseAccessibility == Accessibility.DELAYED)
        {
            RemoveOnActionCallback(InstalledObjectAction.Door_UpdateAction);
        }

        foreach (Tile tile in _tiles)
        {
            tile.ClearInstalledObject();
        }

        if (isInstalled == true)
        {
            InventoryManager.AddToTileInventory(baseTile, _data.GetRequirements());
            GameManager.GetRegionManager().UpdateCluster(GameManager.GetRegionManager().GetClusterAtTile(baseTile), baseTile, (_tiles.Count > 1));
            GameManager.GetWorldGrid().InvalidatePathGraph();
        }

        UpdateNeighbourSprites(_neighbourBuildings);
        GameManager.GetInstalledSpriteController().Uninstall(this);

        UnityEngine.Object.Destroy(gameObject);
    }
    public override string GetObjectNameToString()
    {
        return _data.name;
    }
    public override string GetObjectSpriteName(bool updateNeighbours)
    {
        if (_data.type != BuildingType.WALL)
        {
            if(canRotate == true)
            {
                return GetObjectNameToString() + "_" + _rotation.ToString();
            }
            else
            {
                return GetObjectNameToString();
            }
        }

        string spriteName = _data.name + "_";
        _neighbourBuildings.Clear();

        bool hasNeighbour = false;

        if(baseTile.North != null && baseTile.North.installedObject != null && baseTile.North.installedObject != this && baseTile.North.installedObject.type == InstalledObjectType.BUILDING)
        {
            BuildingData data = (baseTile.North.installedObject as Building)._data;

            if(data != null && (data.type == BuildingType.WALL || data.type == BuildingType.DOOR))
            {
                spriteName += "N";
                hasNeighbour = true;

                _neighbourBuildings.Add(baseTile.North.installedObject as Building);
            }
        }
        if (baseTile.East != null && baseTile.East.installedObject != null && baseTile.East.installedObject != this && baseTile.East.installedObject.type == InstalledObjectType.BUILDING)
        {
            BuildingData data = (baseTile.East.installedObject as Building)._data;

            if (data != null && (data.type == BuildingType.WALL || data.type == BuildingType.DOOR))
            {
                spriteName += "E";
                hasNeighbour = true;

                _neighbourBuildings.Add(baseTile.East.installedObject as Building);
            }
        }
        if (baseTile.South != null && baseTile.South.installedObject != null && baseTile.South.installedObject != this && baseTile.South.installedObject.type == InstalledObjectType.BUILDING)
        {
            BuildingData data = (baseTile.South.installedObject as Building)._data;

            if (data != null && (data.type == BuildingType.WALL || data.type == BuildingType.DOOR))
            {
                spriteName += "S";
                hasNeighbour = true;

                _neighbourBuildings.Add(baseTile.South.installedObject as Building);
            }
        }
        if (baseTile.West != null && baseTile.West.installedObject != null && baseTile.West.installedObject != this && baseTile.West.installedObject.type == InstalledObjectType.BUILDING)
        {
            BuildingData data = (baseTile.West.installedObject as Building)._data;

            if (data != null && (data.type == BuildingType.WALL || data.type == BuildingType.DOOR))
            {
                spriteName += "W";
                hasNeighbour = true;

                _neighbourBuildings.Add(baseTile.West.installedObject as Building);
            }
        }

        if(hasNeighbour == false)
        {
            spriteName += "N";
        }
        
        if(updateNeighbours == true)
        {
            UpdateNeighbourSprites(_neighbourBuildings);
        }

        return spriteName;
    }
    public void UpdateSprite()
    {
        if (updateObjectCallback != null)
        {
            updateObjectCallback(this);
        }
    }
    private void UpdateNeighbourSprites(List<Building> toUpdate)
    {
        if(toUpdate.Count == 0)
        {
            return;
        }

        foreach (Building building in toUpdate)
        {
            building.UpdateSprite();
        }
    }
    public override int GetMovementCost()
    {
        return _data.movementCost;
    }
    public override Accessibility GetAccessibility()
    {
        return _data.baseAccessibility;
    }
}
