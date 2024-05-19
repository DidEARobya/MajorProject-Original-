using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;

public class InstalledSpriteController : MonoBehaviour
{       
    SpriteAtlas objSprites;
    public void AssignAtlas()
    {
        objSprites = GameManager.instance.objectAtlas;
    }
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

        string name = _obj.GetObjectNameToString();

        if (_obj.type == InstalledObjectType.PLANT)
        {
            name += "_" + (_obj as Plant).plantState.ToString();
        }

        renderer.sortingLayerName = "Foreground";

        obj.name = name + _obj.baseTile.x + "_" + _obj.baseTile.y;
        renderer.sprite = objSprites.GetSprite(name);

        

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
        if (obj.hasRelativeRotation == true)
        {
            UpdateSpriteRotation(obj);
        }

        if (obj.isInstalled == false)
        {
            return;
        }

        SpriteRenderer renderer = obj.gameObject.GetComponent<SpriteRenderer>();
        Color colour = renderer.color;
        colour.a = 100;

        if (obj.type == InstalledObjectType.PLANT)
        {
            name = obj.GetObjectNameToString() + "_" + (obj as Plant).plantState.ToString();
            renderer.sprite = objSprites.GetSprite(name);
        }

        renderer.color = colour;
        renderer.sortingLayerName = "Foreground"; 
    }
    public void UpdateSpriteRotation(InstalledObject obj)
    {
        if (obj.baseTile.North != null && obj.baseTile.North.IsObjectInstalled() == true && obj.baseTile.South != null && obj.baseTile.South.IsObjectInstalled() == true)
        {
            obj.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
            obj.gameObject.transform.position += new Vector3(1, 0, 0);
        }
        else if (obj.baseTile.West != null && obj.baseTile.West.IsObjectInstalled() == true && obj.baseTile.East != null && obj.baseTile.East.IsObjectInstalled() == true)
        {
            obj.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            obj.gameObject.transform.position = obj.baseTile.tileObj.transform.position;
        }
    }
    public void Uninstall(InstalledObject obj)
    {
        ObjectManager.RemoveInstalledObject(obj);
        obj.RemoveOnUpdateCallback(OnInstalledObjectChanged);
    }
}
