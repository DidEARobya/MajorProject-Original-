using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledSpriteController : MonoBehaviour
{
    [SerializeField]
    public List<Sprite> installedSprites = new List<Sprite>();

    // Start is called before the first frame update
    public void Init()
    {
        ObjectManager.SetInstallObjectCallback(OnObjectInstalled);
    }
    public void OnObjectInstalled(InstalledObject _obj)
    {
        GameObject obj = new GameObject();

        ObjectManager.AddInstalledObject(_obj);

        obj.name = InstalledObjectTypes.GetObjectType((_obj.type)).ToString() + _obj.baseTile.x + "_" + _obj.baseTile.y;
        obj.transform.position = new Vector3(_obj.baseTile.x, _obj.baseTile.y);
        obj.transform.SetParent(this.transform, true);

        _obj.gameObject = obj;

        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();

        InstalledObjectType type = InstalledObjectTypes.GetObjectType(_obj.type);

        switch (type)
        {
            case InstalledObjectType.WALL:
                renderer.sprite = installedSprites[0];
                break;
            case InstalledObjectType.DOOR:
                renderer.sprite = installedSprites[1];
                break;
        }

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
