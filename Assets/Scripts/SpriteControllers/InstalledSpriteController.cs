using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class InstalledSpriteController : MonoBehaviour
{
    [SerializeField]
    public SpriteAtlas objSprites;

    // Start is called before the first frame update
    public void Init()
    {
        ObjectManager.SetInstallObjectCallback(OnObjectInstalled);
    }
    public void OnObjectInstalled(InstalledObject _obj)
    {
        GameObject obj = new GameObject();

        ObjectManager.AddInstalledObject(_obj);

        obj.transform.position = new Vector3(_obj.baseTile.x, _obj.baseTile.y);
        obj.transform.SetParent(this.transform, true);

        _obj.gameObject = obj;

        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();

        string name = " ";

        if (_obj.type == InstalledObjectType.FURNITURE)
        {
            name = FurnitureTypes.GetObjectType(((_obj as Furniture).furnitureType)).ToString();
        }
        else if (_obj.type == InstalledObjectType.ORE)
        {
            name = OreTypes.GetObjectType(((_obj as Ore).oreType)).ToString();
            //(_obj as Ore).QueueMiningTask();
        }

        obj.name = name + _obj.baseTile.x + "_" + _obj.baseTile.y;
        renderer.sprite = objSprites.GetSprite(name);

        renderer.sortingLayerName = "Walls";

        if(_obj.isInstalled == false)
        {
            Color colour = renderer.color;
            colour.a = 0.3f;

            renderer.color = colour;
        }

        _obj.AddOnUpdateCallback(OnInstalledObjectChanged);
    }
    public void OnInstalledObjectChanged(InstalledObject obj)
    {
        if (obj.isInstalled == true)
        {
            SpriteRenderer renderer = obj.gameObject.GetComponent<SpriteRenderer>();
            Color colour = renderer.color;
            colour.a = 100;

            renderer.color = colour;
            renderer.sortingLayerName = "Walls";
        }
    }

    public void Uninstall(InstalledObject obj)
    {
        ObjectManager.RemoveInstalledObject(obj);
        obj.RemoveOnUpdateCallback(OnInstalledObjectChanged);
    }
}
