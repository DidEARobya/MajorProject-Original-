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
    public override int GetMovementCost()
    {
        //Edit
        return 1;
    }
}
