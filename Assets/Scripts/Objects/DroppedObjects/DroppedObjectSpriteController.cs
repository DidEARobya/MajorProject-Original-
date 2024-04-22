using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedObjectSpriteController : MonoBehaviour
{
    [SerializeField]
    public List<Sprite> droppedSprites = new List<Sprite>();

    // Start is called before the first frame update
    public void Init()
    {
        ObjectManager.SetDroppedObjectCallback(OnObjectSpawned);
    }
    public void OnObjectSpawned(DroppedObject _obj)
    {
        GameObject obj = new GameObject();

        ObjectManager.AddDroppedObject(_obj);

        obj.name = DroppedObjectTypes.GetObjectType((_obj.type)).ToString() + _obj.baseTile.x + "_" + _obj.baseTile.y;

        obj.transform.position = new Vector3(_obj.baseTile.x, _obj.baseTile.y);
        obj.transform.SetParent(_obj.baseTile.tileObj.transform, true);

        _obj.gameObject = obj;

        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();

        DroppedObjectType type = DroppedObjectTypes.GetObjectType(_obj.type);

        switch (type)
        {
            case DroppedObjectType.WOOD:
                renderer.sprite = droppedSprites[0];
                break;
        }

        renderer.sortingLayerName = "Item";

        _obj.AddDroppedObjectUpdate(OnObjectUpdate);
    }
    void OnObjectUpdate(DroppedObject obj, CharacterController character)
    {
        obj.gameObject.transform.position = new Vector2(character.x, character.y);
    }
}
