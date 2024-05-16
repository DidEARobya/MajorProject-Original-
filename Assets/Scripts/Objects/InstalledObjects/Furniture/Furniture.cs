using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

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
        isInstalled = true;
        baseTile.accessibility = FurnitureTypes.GetBaseAccessibility(furnitureType);
        GameManager.GetWorldGrid().InvalidatePathGraph();

        if(baseTile.accessibility == Accessibility.IMPASSABLE && baseTile.zone != null)
        {
            baseTile.zone.RemoveTile(baseTile); 
        }
        RegionManager.UpdateCluster(RegionManager.GetClusterAtTile(baseTile));

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
        if (FurnitureTypes.GetBaseAccessibility(furnitureType) == Accessibility.DELAYED)
        {
            RemoveOnActionCallback(InstalledObjectAction.Door_UpdateAction);
        }

        if(isInstalled == true)
        {
            //RegionManager.UpdateCluster(RegionManager.GetClusterAtTile(baseTile));
            InventoryManager.AddToTileInventory(baseTile, FurnitureTypes.GetRequirements(furnitureType, baseMaterial));
            GameManager.GetWorldGrid().InvalidatePathGraph();
        }

        GameManager.GetInstalledSpriteController().Uninstall(this);

        UnityEngine.Object.Destroy(gameObject);
    }
    public override int GetMovementCost()
    {
        return FurnitureTypes.GetMovementCost(furnitureType);
    }
}
