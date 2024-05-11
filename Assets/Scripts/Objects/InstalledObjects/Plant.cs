using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class Plant : InstalledObject
{
    public PlantTypes plantType;
    public PlantState plantState;

    public State state;
    public State[] plantStates;

    static public Plant PlaceObject(PlantTypes _type, Tile tile, PlantState state)
    {
        Plant obj = new Plant();
        obj.type = InstalledObjectType.PLANT;

        obj.baseTile = tile;
        obj.plantType = _type;
        obj.durability = PlantTypes.GetDurability(_type);

        obj.plantStates = new State[2];
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
        plantStates[0] = new GrowthState(this);
        plantStates[1] = new GrownState(this);

        if(plantState != PlantState.GROWN)
        {
            state = plantStates[0];
        }
        else
        {
            state = plantStates[1];
        }

        state.StateStart();

        isInstalled = true;
        baseTile.accessibility = PlantTypes.GetBaseAccessibility(plantType);
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
        state.StateEnd();
        plantStates = null;

        InventoryManager.AddToTileInventory(baseTile, PlantTypes.GetYield(plantType));
        GameManager.GetWorldGrid().InvalidatePathGraph();

        RegionManager.UpdateCluster(RegionManager.GetClusterAtTile(baseTile));

        GameManager.GetInstalledSpriteController().Uninstall(this);

        UnityEngine.Object.Destroy(gameObject);
    }
    public override int GetMovementCost()
    {
        return PlantTypes.GetMovementCost(plantType);
    }
    public void UpdateGrowthState(PlantState _state)
    {
        if(plantState == _state)
        {
            return;
        }

        plantState = _state;

        if (updateObjectCallback != null)
        {
            updateObjectCallback(this);
        }
    }
    public void SetState(PlantState _state)
    {
        state.StateEnd();

        plantState = _state;

        if (plantState != PlantState.GROWN)
        {
            state = plantStates[0];
        }
        else
        {
            state = plantStates[1];
        }

        state.StateStart();

        if (updateObjectCallback != null)
        {
            updateObjectCallback(this);
        }
    }

}
