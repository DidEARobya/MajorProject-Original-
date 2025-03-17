using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderStrategy : IActionStrategy
{
    GOAP_Agent agent;
    CharacterController character;

    int radius;
    public bool CanPerform => !Complete;
    public bool Complete
    {
        get { return character.taskList.Count != 0 || (character.requestedPath == false && character.pathFinder == null && agent.isSocialising == false); }
        private set { }
    }

    public WanderStrategy(GOAP_Agent _agent, int _radius)
    {
        agent = _agent;
        character = agent.character;
        radius = _radius;
    }

    public void Start()
    {
        Tile start = character.currentTile;

        Tile tile = GameManager.GetWorldGrid().GetRandomTileUsingRadius(start, radius);

        if (tile == null)
        {
            return;
        }

        PathRequestHandler.RequestPath(character, tile, false);
    }
    public void Stop()
    {
        Complete = false;
    }
}
