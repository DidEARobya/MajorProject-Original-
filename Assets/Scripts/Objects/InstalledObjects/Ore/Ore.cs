using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : InstalledObject
{
    public OreTypes oreType;

    static public Ore PlaceObject(OreTypes _type, Tile tile)
    {
        Ore obj = new Ore();
        obj.type = InstalledObjectType.ORE;

        obj.baseTile = tile;
        obj.oreType = _type;
        obj.durability = OreTypes.GetDurability(_type);

        if (tile.InstallObject(obj) == false)
        {
            return null;
        }

        obj.Install();

        return obj;
    }
    public override void Install()
    {
        isInstalled = true;
        baseTile.accessibility = OreTypes.GetBaseAccessibility(oreType);
        baseTile.installedObject = this;

        GameManager.GetWorldGrid().InvalidatePathGraph();

        RegionManager.UpdateCluster(RegionManager.GetClusterAtTile(baseTile));

        if (updateObjectCallback != null)
        {
            updateObjectCallback(this);
        }
    }
    public override void UnInstall()
    {
        InventoryManager.AddToTileInventory(baseTile, OreTypes.GetComponents(oreType));
        GameManager.GetWorldGrid().InvalidatePathGraph();

        RegionManager.UpdateCluster(RegionManager.GetClusterAtTile(baseTile));

        GameManager.GetInstalledSpriteController().Uninstall(this);

        UnityEngine.Object.Destroy(gameObject);
    }
    public void QueueMiningTask()
    {
        Task task = new MineTask(baseTile, (t) => { baseTile.UninstallObject(); }, TaskType.MINING, false, baseTile.GetInstalledObject().durability);
        TaskManager.AddTask(task, task.taskType);
    }
    public override string GetObjectNameToString()
    {
        return OreTypes.GetObjectType(oreType).ToString();
    }
    public override int GetMovementCost()
    {
        return OreTypes.GetMovementCost(oreType);
    }
}
