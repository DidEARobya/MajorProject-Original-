using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : InstalledObject
{
    public PlantTypes plantType;
    public PlantState plantState;

    public GrowthState state;
    public Dictionary<PlantState, GrowthState> states;

    static public Plant PlaceObject(PlantTypes _type, Tile tile, PlantState state)
    {
        Plant obj = new Plant();
        obj.type = InstalledObjectType.PLANT;

        obj.baseTile = tile;
        obj.plantType = _type;
        obj.durability = PlantTypes.GetDurability(_type);

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
        baseTile.accessibility = PlantTypes.GetBaseAccessibility(plantType);
        baseTile.installedObject = this;
        isInstalled = true;

        states.Add(PlantState.SEED, new SeedState(this));
        states.Add(PlantState.EARLY_GROWTH, new EarlyGrowthState(this));
        states.Add(PlantState.LATE_GROWTH, new LateGrowthState(this));
        states.Add(PlantState.GROWN, new GrownState(this));

        state = states[plantState];
        state.StateStart();

        GameManager.GetWorldGrid().InvalidatePathGraph();

        RegionManager.UpdateCluster(RegionManager.GetClusterAtTile(baseTile));

        if (updateObjectCallback != null)
        {
            updateObjectCallback(this);
        }
    }
    public override void UnInstall()
    {
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
                InventoryManager.AddToTileInventory(baseTile, PlantTypes.GetYield(plantType, plantState));
            }

            RegionManager.UpdateCluster(RegionManager.GetClusterAtTile(baseTile));
            GameManager.GetWorldGrid().InvalidatePathGraph();
        }

        UnityEngine.Object.Destroy(gameObject);
    }
    public override string GetObjectNameToString()
    {
        return PlantTypes.GetObjectType(plantType).ToString();
    }
    public override int GetMovementCost()
    {
        return PlantTypes.GetMovementCost(plantType);
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
