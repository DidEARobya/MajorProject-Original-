using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PlantState
{
	SEED,
    EARLY_GROWTH,
    LATE_GROWTH,
    GROWN
}
public class GrowthState : State
{
    protected Plant plant;
	protected float growthRate;
    protected float growth;

	float tendDelay = 5;
	float delay = 0;

    float seedMax;
    float lowGrowthMax;
    float highGrowthMax;

    bool taskCreated;
    public GrowthState(Plant _plant)
	{
		plant = _plant;
	}
    public override void StateStart()
	{
        growthRate = PlantTypes.GetGrowthRate(plant.plantType);
        delay = 0;
        growth = 0;

        seedMax = plant.durability * 0.33f;
        lowGrowthMax = plant.durability * 0.66f;
    }
    public override void StateEnd()
	{
        
	}
    public override void Update(float deltaTime)
	{
        if(growth < seedMax && plant.plantState != PlantState.SEED)
        {
            plant.UpdateGrowthState(PlantState.SEED);
        }
        else if(growth > seedMax && growth < lowGrowthMax && plant.plantState != PlantState.EARLY_GROWTH)
        {
            plant.UpdateGrowthState(PlantState.EARLY_GROWTH);
        }
        else if(growth > lowGrowthMax && plant.plantState != PlantState.LATE_GROWTH)
        {
            plant.UpdateGrowthState(PlantState.LATE_GROWTH);
        }

        if(growth > plant.durability)
        {
            plant.SetState(PlantState.GROWN);
            return;
        }

        if (delay >= tendDelay)
        {
            if(taskCreated == false)
            {
                TendTask task = new TendTask(plant.baseTile, (t) => { delay = 0; taskCreated = false; }, TaskType.CONSTRUCTION, false, 50);
                TaskManager.AddTask(task, TaskType.CONSTRUCTION);

                taskCreated = true;
            }

            return;
        }

        delay += deltaTime;
        growth += deltaTime * growthRate;
	}
}
public class GrownState : State
{
    protected Plant plant;

    float tendDelay = 100;
    float delay = 0;

    public GrownState(Plant _plant)
    {
        plant = _plant;
    }
    public override void StateStart()
    {
        delay = 0;
    }
    public override void StateEnd()
    {

    }
    public override void Update(float deltaTime)
    {
        if (delay >= tendDelay)
        {
            return;
        }

        delay += deltaTime;
    }
}