using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : InstalledObject
{
    public FurnitureTypes furnitureType;
    public ItemTypes baseMaterial;

    static public Furniture PlaceObject(FurnitureTypes _type, ItemTypes _baseMaterial, Tile tile, bool _isInstalled)
    {
        Furniture obj = new Furniture();
        obj.type = InstalledObjectType.FURNITURE;

        obj.baseTile = tile;
        obj.furnitureType = _type;
        obj.baseMaterial = _baseMaterial;
        obj.durability = FurnitureTypes.GetDurability(_type, obj.baseMaterial);
        obj.hasRelativeRotation = FurnitureTypes.HasRelativeRotation(_type);

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
        baseTile.accessibility = FurnitureTypes.GetBaseAccessibility(furnitureType);
        GameManager.GetWorldGrid().InvalidatePathGraph();

        if(baseTile.accessibility == Accessibility.IMPASSABLE && baseTile.zone != null)
        {
            baseTile.zone.RemoveTile(baseTile); 
        }

        GameManager.GetRegionManager().UpdateCluster(GameManager.GetRegionManager().GetClusterAtTile(baseTile));

        if (FurnitureTypes.GetBaseAccessibility(furnitureType) == Accessibility.DELAYED)
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

        if (FurnitureTypes.GetBaseAccessibility(furnitureType) == Accessibility.DELAYED)
        {
            RemoveOnActionCallback(InstalledObjectAction.Door_UpdateAction);
        }

        if (isInstalled == true)
        {
            InventoryManager.AddToTileInventory(baseTile, FurnitureTypes.GetRequirements(furnitureType, baseMaterial));
            GameManager.GetRegionManager().UpdateCluster(GameManager.GetRegionManager().GetClusterAtTile(baseTile));
            GameManager.GetWorldGrid().InvalidatePathGraph();
        }

        UnityEngine.Object.Destroy(gameObject);
    }
    public override string GetObjectNameToString()
    {
        return ItemTypes.GetItemType(baseMaterial).ToString() + "_" + FurnitureTypes.GetObjectType(furnitureType).ToString();
    }
    public override int GetMovementCost()
    {
        return FurnitureTypes.GetMovementCost(furnitureType);
    }
}
