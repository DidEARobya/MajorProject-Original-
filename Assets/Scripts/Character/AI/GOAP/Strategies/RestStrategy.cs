using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Complete
    {
        get; private set;
    }

    GOAP_Agent agent;
    public RestStrategy(GOAP_Agent _agent)
    {
        agent = _agent;
    }

    public void Start()
    {

    }

    public void Update(float deltaTime)
    {
        if (agent.stamina < 100)
        {
            agent.stamina += 2.5f * deltaTime;

            if (agent.stamina > 100)
            {
                agent.stamina = 100;
            }
        }

        if (agent.stamina >= 70)
        {
            Complete = true;
        }
    }

    public void Stop()
    {
        Complete = false;
    }
}
