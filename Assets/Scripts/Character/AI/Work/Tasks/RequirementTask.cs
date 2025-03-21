using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TreeEditor;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;

public class RequirementTask : Task
{
    public RequirementTask(Tile _tile, Action<Task> _taskCompleteCallback, TaskType type, Dictionary<ItemData, int> _requirements, bool _isFloor, float _taskTime = 1) : base(_tile, _taskCompleteCallback, type, _isFloor, _taskTime)
    {
        //if(isFloor == true)
        //{
        //    AddTaskCompleteCallback((t) => { if (tile.taskDisplayObject != null) { GameObject.Destroy(tile.taskDisplayObject); }; });

        //    if (tile.taskDisplayObject == null)
        //    {
        //        tile.taskDisplayObject = new GameObject();
        //        tile.taskDisplayObject.transform.SetParent(tile.tileObj.transform, true);
        //        tile.taskDisplayObject.name = "TASK";
        //        tile.taskDisplayObject.transform.position = tile.tileObj.transform.position;

        //        SpriteRenderer taskRenderer = tile.taskDisplayObject.AddComponent<SpriteRenderer>();
        //        taskRenderer.sprite = GameManager.GetTileSpriteController().GetFloorSprite(FloorType.TASK_FLOOR);
        //        taskRenderer.sortingLayerName = "Foreground";
        //    }
        //}
    }
    public override void InitTask(CharacterController character)
    {
        if (character.inventory.item != null)
        {
            InventoryManager.DropInventory(character.inventory, character.currentTile);
        }

        base.InitTask(character);
    }
    public override void DoWork(float workTime)
    {
        base.DoWork(workTime);
    }
    public override void CancelTask(bool isCancelled, bool toIgnore = false)
    {
        tile.accessibility = Accessibility.ACCESSIBLE;

        if(isCancelled == false)
        {
            if (isFloor == true)
            {
                if (tile.taskDisplayObject != null)
                {
                    GameObject.Destroy(tile.taskDisplayObject);
                }
            }
            else if (tile.installedObject != null)
            {
                tile.UninstallObject();
            }
        }

        base.CancelTask(isCancelled, toIgnore);
    }
}
