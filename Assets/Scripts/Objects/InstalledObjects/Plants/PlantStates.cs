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
    public TendTask task;

    protected Plant plant;
	protected float growthRate;
    protected float growth;
    protected int growthStage;

	protected float tendDelay = 20;
	protected float delay = 0;

    bool taskCreated;
    bool hasEnded;

    public GrowthState(Plant _plant)
	{
		plant = _plant;
	}
    public override void StateStart()
	{
        growthRate = PlantTypes.GetGrowthRate(plant.plantType);
        delay = 0;
        growth = 0;

        growthStage = 50;
    }
    public override void StateEnd()
	{
        hasEnded = true;
	}
    public override void Update(float deltaTime)
	{
        if(hasEnded == true)
        {
            return;
        }

        if (delay >= tendDelay)
        {
            if(taskCreated == false)
            {
                task = new TendTask(plant.baseTile, (t) => { delay = 0; taskCreated = false; }, TaskType.AGRICULTURE, false, 50);
                TaskManager.AddTask(task, TaskType.AGRICULTURE);

                taskCreated = true;
            }

            return;
        }

        delay += deltaTime;
	}
}
public class SeedState : GrowthState
{
    public SeedState(Plant _plant) : base(_plant)
    {
        plant = _plant;
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (delay >= tendDelay)
        {
            return;
        }

        growth += deltaTime * growthRate;

        if (growth > growthStage)
        {
            plant.SetState(PlantState.EARLY_GROWTH);
            return;
        }
    }
}
public class EarlyGrowthState : GrowthState
{
    public EarlyGrowthState(Plant _plant) : base(_plant)
    {
        plant = _plant;
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (delay >= tendDelay)
        {
            return;
        }

        growth += deltaTime * growthRate;

        if (growth > growthStage)
        {
            plant.SetState(PlantState.LATE_GROWTH);
            return;
        }
    }
}
public class LateGrowthState : GrowthState
{
    public LateGrowthState(Plant _plant) : base(_plant)
    {
        plant = _plant;
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (delay >= tendDelay)
        {
            return;
        }

        growth += deltaTime * growthRate;

        if (growth > growthStage)
        {
            plant.SetState(PlantState.GROWN);
            return;
        }
    }
}
public class GrownState : GrowthState
{
    public GrownState(Plant _plant) : base(_plant)
    {
        plant = _plant;
    }
    public override void StateStart()
    {
        delay = 0;
    }
}