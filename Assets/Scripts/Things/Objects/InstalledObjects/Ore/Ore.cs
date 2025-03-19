using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ore : InstalledObject
{
    public OreData _data;

    static public Ore PlaceObject(OreType type, Tile tile)
    {
        Ore obj = new Ore();
        obj.type = InstalledObjectType.ORE;

        obj._data = ThingsDataHandler.GetOreData(type);
        obj.baseTile = tile;
        obj.durability = obj._data.durability;

        if (tile.PlaceObject(obj) == false)
        {
            return null;
        }

        obj.Install();

        return obj;
    }
    public override void Install()
    {
        base.Install();

        isInstalled = true;
        baseTile.accessibility = Accessibility.IMPASSABLE;

        GameManager.GetWorldGrid().InvalidatePathGraph();

        GameManager.GetRegionManager().UpdateCluster(GameManager.GetRegionManager().GetClusterAtTile(baseTile), baseTile, false);

        if (updateObjectCallback != null)
        {
            updateObjectCallback(this);
        }
    }
    public override void UnInstall()
    {
        base.UnInstall();
        GameManager.GetInstalledSpriteController().Uninstall(this);

        if (isInstalled == true)
        {
            InventoryManager.AddToTileInventory(ThingsDataHandler.GetItemData(_data.resourceType), baseTile, Utility.GetRandomInt(_data.yieldMin, _data.yieldMax));
            GameManager.GetRegionManager().UpdateCluster(GameManager.GetRegionManager().GetClusterAtTile(baseTile), baseTile, false);
            GameManager.GetWorldGrid().InvalidatePathGraph();
        }

        UnityEngine.Object.Destroy(gameObject);
    }
    public void QueueMiningTask()
    {
        Task task = new MineTask(baseTile, (t) => { baseTile.UninstallObject(); }, TaskType.MINING, false, baseTile.GetInstalledObject().durability);
        GameManager.GetTaskManager().AddTask(task, task.taskType);
    }
    public override string GetObjectNameToString()
    {
        return _data.type.ToString();
    }
}
