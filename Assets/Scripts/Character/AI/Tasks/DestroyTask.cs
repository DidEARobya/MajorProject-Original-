using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DestroyTask : Task
{
    Color colour;
    SpriteRenderer spriteRenderer;

    public DestroyTask(Tile _tile, Action<Task> _taskCompleteCallback, TaskType type, bool _isFloor, float targetDurability) : base(_tile, _taskCompleteCallback, type, _isFloor, targetDurability)
    {
        if(isFloor == false)
        {
            spriteRenderer = tile.installedObject.gameObject.GetComponent<SpriteRenderer>();
            colour = spriteRenderer.color;
        }
        else
        {
            spriteRenderer = tile.tileObj.GetComponent<SpriteRenderer>();
            colour = spriteRenderer.color;
        }

        Color _colour = colour;
        _colour.r = 1f;
        _colour.g = 0f;
        _colour.b = 0f;

        spriteRenderer.color = _colour;
    }
    public override void InitTask(CharacterController character)
    {
        base.InitTask(character);
        PathRequestHandler.RequestPath(worker, tile);
    }
    public override void DoWork(float workTime)
    {
        base.DoWork(workTime);

        if (taskTime <= 0)
        {
            if (isFloor == true)
            {
                spriteRenderer.color = colour;
            }
        }
    }
    public override void CancelTask(bool isCancelled, bool toIgnore = false)
    {
        if (isCancelled == false)
        {
            if(spriteRenderer == null)
            {
                Debug.Log("Invalid Sprite Renderer");
                return;
            }
            spriteRenderer.color = colour;
        }

        base.CancelTask(isCancelled, toIgnore);
    }
}
