using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : InstalledObject
{
    public PlantData _data;
    public PlantState plantState;

    public GrowthState state;
    public Dictionary<PlantState, GrowthState> states;

    static public Plant PlaceObject(PlantType _type, Tile tile, PlantState state)
    {
        Plant obj = new Plant();
        obj.type = InstalledObjectType.PLANT;

        obj._data = ThingsDataHandler.GetPlantData(_type);
        obj.baseTile = tile;
        obj.durability = obj._data.durability;

        obj.states = new Dictionary<PlantState, GrowthState>();
        obj.plantState = state;

        if (tile.InstallObject(obj) == false)
        {
            return null;
        }

        obj.Install();

        return obj;
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if(state == null)
        {
            return;
        }

        state.Update(deltaTime);
    }
    public override void Install()
    {
        base.Install();

        baseTile.accessibility = Accessibility.ACCESSIBLE;
        baseTile.installedObject = this;
        isInstalled = true;

        states.Add(PlantState.SEED, new SeedState(this));
        states.Add(PlantState.EARLY_GROWTH, new EarlyGrowthState(this));
        states.Add(PlantState.LATE_GROWTH, new LateGrowthState(this));
        states.Add(PlantState.GROWN, new GrownState(this));

        state = states[plantState];
        state.StateStart();

        GameManager.GetWorldGrid().InvalidatePathGraph();

        GameManager.GetRegionManager().UpdateCluster(GameManager.GetRegionManager().GetClusterAtTile(baseTile), baseTile);

        if (updateObjectCallback != null)
        {
            updateObjectCallback(this);
        }
    }
    public override void UnInstall()
    {
        base.UnInstall();

        if(state.task != null)
        {
            state.task.CancelTask(false);
        }

        state.StateEnd();
        states = null;

        GameManager.GetInstalledSpriteController().Uninstall(this);

        if(isInstalled == true)
        {
            if (plantState != PlantState.SEED)
            {
                InventoryManager.AddToTileInventory(baseTile, _data.GetYield(plantState));
            }

            if (baseTile.zone != null)
            {
                baseTile.zone.UpdateZoneTasks();
            }

            GameManager.GetRegionManager().UpdateCluster(GameManager.GetRegionManager().GetClusterAtTile(baseTile), baseTile);
            GameManager.GetWorldGrid().InvalidatePathGraph();
        }

        UnityEngine.Object.Destroy(gameObject);
    }
    public override string GetObjectNameToString()
    {
        return _data.type.ToString();
    }
    public override string GetObjectSpriteName(bool updateNeighbours)
    {
        return GetObjectNameToString() + "_" + plantState.ToString();
    }
    public override int GetMovementCost()
    {
        return _data.movementCost;
    }
    public void SetState(PlantState _state)
    {
        if(plantState == _state)
        {
            return;
        }

        state.StateEnd();

        plantState = _state;
        state = states[plantState];

        state.StateStart();

        if (updateObjectCallback != null)
        {
            updateObjectCallback(this);
        }
    }
}
