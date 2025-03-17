using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Building : InstalledObject
{
    public ItemData baseMaterial;
    public BuildingData _data;
    static public Building PlaceObject(BuildingData data, Tile tile, bool _isInstalled)
    {
        if(data == null)
        {
            Debug.LogError("FAILED TO FIND BUILDINGDATA");
            return null;
        }

        Building obj = new Building();
        obj.type = InstalledObjectType.FURNITURE;

        obj.baseTile = tile;
        obj.durability = data.durability;
        obj.hasRelativeRotation = data.hasRelativeRotation;
        obj._data = data;

        if (tile.InstallObject(obj) == false)
        {
            return null;
        }

        if (_isInstalled == true)
        {
            obj.Install();
        }

        return obj;
    }
    public override void Install()
    {
        base.Install();

        isInstalled = true;
        baseTile.accessibility = _data.baseAccessibility;
        GameManager.GetWorldGrid().InvalidatePathGraph();

        if (baseTile.accessibility == Accessibility.IMPASSABLE && baseTile.zone != null)
        {
            baseTile.zone.RemoveTile(baseTile); 
        }

        GameManager.GetRegionManager().UpdateCluster(GameManager.GetRegionManager().GetClusterAtTile(baseTile), baseTile);

        if (_data.baseAccessibility == Accessibility.DELAYED)
        {
            AddOnActionCallback(InstalledObjectAction.Door_UpdateAction);
        }

        if (updateObjectCallback != null)
        {
            updateObjectCallback(this);
        }
    }
    public override void UnInstall()
    {
        base.UnInstall();
        GameManager.GetInstalledSpriteController().Uninstall(this);

        if (_data.baseAccessibility == Accessibility.DELAYED)
        {
            RemoveOnActionCallback(InstalledObjectAction.Door_UpdateAction);
        }

        if (isInstalled == true)
        {
            InventoryManager.AddToTileInventory(baseTile, _data.GetRequirements());
            GameManager.GetRegionManager().UpdateCluster(GameManager.GetRegionManager().GetClusterAtTile(baseTile), baseTile);
            GameManager.GetWorldGrid().InvalidatePathGraph();
        }

        UnityEngine.Object.Destroy(gameObject);
    }
    public override string GetObjectNameToString()
    {
        return _data.name;
    }
    public override string GetObjectSpriteName(bool updateNeighbours)
    {
        if (_data.type != FurnitureType.WALL)
        {
            return GetObjectNameToString();
        }

        string spriteName = _data.name + "_";
        List<Building> validNeighbours = new List<Building>();

        bool hasNeighbour = false;

        if(baseTile.North != null && baseTile.North.installedObject != null && baseTile.North.installedObject.type == InstalledObjectType.FURNITURE)
        {
            BuildingData data = (baseTile.North.installedObject as Building)._data;

            if(data != null && (data.type == FurnitureType.WALL || data.type == FurnitureType.DOOR))
            {
                spriteName += "N";
                hasNeighbour = true;

                validNeighbours.Add(baseTile.North.installedObject as Building);
            }
        }
        if (baseTile.East != null && baseTile.East.installedObject != null && baseTile.East.installedObject.type == InstalledObjectType.FURNITURE)
        {
            BuildingData data = (baseTile.East.installedObject as Building)._data;

            if (data != null && (data.type == FurnitureType.WALL || data.type == FurnitureType.DOOR))
            {
                spriteName += "E";
                hasNeighbour = true;

                validNeighbours.Add(baseTile.East.installedObject as Building);
            }
        }
        if (baseTile.South != null && baseTile.South.installedObject != null && baseTile.South.installedObject.type == InstalledObjectType.FURNITURE)
        {
            BuildingData data = (baseTile.South.installedObject as Building)._data;

            if (data != null && (data.type == FurnitureType.WALL || data.type == FurnitureType.DOOR))
            {
                spriteName += "S";
                hasNeighbour = true;

                validNeighbours.Add(baseTile.South.installedObject as Building);
            }
        }
        if (baseTile.West != null && baseTile.West.installedObject != null && baseTile.West.installedObject.type == InstalledObjectType.FURNITURE)
        {
            BuildingData data = (baseTile.West.installedObject as Building)._data;

            if (data != null && (data.type == FurnitureType.WALL || data.type == FurnitureType.DOOR))
            {
                spriteName += "W";
                hasNeighbour = true;

                validNeighbours.Add(baseTile.West.installedObject as Building);
            }
        }

        if(hasNeighbour == false)
        {
            spriteName += "N";
        }
        
        if(updateNeighbours == true)
        {
            UpdateNeighbourSprites(validNeighbours);
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
        //Edit
        return 1;
    }
}
