using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_Sensor
{
    CharacterController character;
    GOAP_Agent agent;
    Tile currentTile => character.currentTile;

    float scanInterval;
    float timer;
    public GOAP_Sensor(CharacterController _character, GOAP_Agent _agent)
    {
        character = _character;
        agent = _agent;

        scanInterval = 0.1f;
        timer = 0.0f;
    }

    public void Update(float deltaTime)
    {
        timer += deltaTime;

        if(character.activeTask != null || character.taskList.Count != 0)
        {
            return;
        }

        if(timer > scanInterval)
        {
            TaskRequestHandler.RequestTask(character);
            timer = 0.0f;
        }
    }
}
