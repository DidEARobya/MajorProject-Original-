using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum DroppedObjectType
{
    WOOD
}
public class DroppedObject
{
    public Tile baseTile;
    public DroppedObjectTypes type;
    public GameObject gameObject;

    CharacterController heldBy;
    public int movementCost;

    Action<DroppedObject, CharacterController> updateObjectCallback;

    protected DroppedObject()
    {

    }
    static public DroppedObject SpawnObject(DroppedObjectTypes _type, Tile tile)
    {
        DroppedObject obj = new DroppedObject();
        obj.baseTile = tile;
        obj.type = _type;

        obj.movementCost = DroppedObjectTypes.GetMovementCost(_type);

        if (obj.movementCost != 0)
        {
            GameManager.GetWorldGrid().InvalidatePathGraph();
        }

        if (tile.PlaceObject(obj) == false)
        {
            return null;
        }

        CharacterController character = CharacterManager.characters[0];

        Task task = new Task(tile, (t) => { obj.PickUp(character); }, TaskType.CONSTRUCTION);

        TaskManager.AddTask(task, task.taskType);

        return obj;
    }
    public void Update(float deltaTime)
    {
        if ((updateObjectCallback != null && heldBy != null))
        {
            updateObjectCallback(this, heldBy);
        }
    }
    public void PickUp(CharacterController character)
    {
        baseTile.droppedObject = null;

        if (movementCost != 0)
        {
            GameManager.GetWorldGrid().InvalidatePathGraph();
        }

        if(character.heldObject != null)
        {
            character.heldObject.Drop(baseTile);
        }

        heldBy = character;
        character.heldObject = this;
        gameObject.transform.position = character.characterObj.transform.position;
        gameObject.transform.SetParent(character.characterObj.transform);

        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Held";
    }
    public void Drop(Tile tile)
    {
        if(tile.inventory.CanBeStored(this) == false)
        {
            return; 
        }

        if (movementCost != 0)
        {
            GameManager.GetWorldGrid().InvalidatePathGraph();
        }

        if (heldBy.heldObject != this)
        {
            return;
        }

        heldBy.heldObject = null;
        heldBy = null;

        gameObject.transform.position = tile.tileObj.transform.position;
        gameObject.transform.SetParent(tile.tileObj.transform);

        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Item";

        tile.inventory.StoreItem(this);
    }
    public void Destroy()
    {
        if (movementCost != 0 && baseTile != null)
        {
            GameManager.GetWorldGrid().InvalidatePathGraph();
        }

        baseTile = null;
        ObjectManager.RemoveDroppedObject(this);

        RemoveDroppedObjectUpdate(updateObjectCallback);
        UnityEngine.Object.Destroy(gameObject);
    }
    public void AddDroppedObjectUpdate(Action<DroppedObject, CharacterController> callback)
    {
        updateObjectCallback += callback;
    }
    public void RemoveDroppedObjectUpdate(Action<DroppedObject, CharacterController> callback)
    {
        updateObjectCallback -= callback;
    }
}
public class DroppedObjectTypes
{
    protected readonly DroppedObjectType type;
    protected readonly int movementCost;

    protected readonly int width = 1;
    protected readonly int height = 1;

    protected readonly int maxStackSize;

    public static readonly DroppedObjectTypes WOOD = new DroppedObjectTypes(DroppedObjectType.WOOD, 50);

    protected DroppedObjectTypes(DroppedObjectType _type, int _maxStackSize, int _movementCost = 0)
    {
        type = _type;
        movementCost = _movementCost;
        maxStackSize = _maxStackSize;
    }

    public static DroppedObjectType GetObjectType(DroppedObjectTypes type)
    {
        return type.type;
    }
    public static int GetMovementCost(DroppedObjectTypes type)
    {
        return type.movementCost;
    }
    public static int GetMaxStackSize(DroppedObjectTypes type)
    {
        return type.maxStackSize;
    }
}