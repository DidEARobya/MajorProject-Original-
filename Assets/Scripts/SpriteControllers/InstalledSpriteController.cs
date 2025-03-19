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

        obj.transform.position = new Vector3(_obj.baseTile.x, _obj.baseTile.y);
        obj.transform.SetParent(this.transform, true);

        _obj.gameObject = obj;

        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();

        string name;
        
        if(_obj.type == InstalledObjectType.BUILDING)
        {
            //Direction rotation = (_obj as Building)._rotation;

            //switch(()
            //{
            //    case Direction.N:
            //        break;

            //    case Direction.E:
            //        obj.transform.position += new Vector3(0, 1);
            //        obj.transform.Rotate(new Vector3(0, 0, -90));
            //        break;
            //    case Direction.S:
            //        obj.transform.position += new Vector3(1, 1);
            //        obj.transform.Rotate(new Vector3(0, 0, 180));
            //        break;
            //    case Direction.W:
            //        obj.transform.position += new Vector3(1, 0);
            //        obj.transform.Rotate(new Vector3(0, 0, 90));
            //        break;
            //}

            name = _obj.GetObjectSpriteName(true);
        }
        else
        {
            name = _obj.GetObjectSpriteName(false);
        }

        renderer.sortingLayerName = "Foreground";

        obj.name = name + _obj.baseTile.x + "_" + _obj.baseTile.y;

        renderer.sprite = objSprites.GetSprite(name);

        if(renderer.sprite == null )
        {
            Debug.Log("MISSING: " + name);
        }

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

        if (obj.type == InstalledObjectType.BUILDING)
        {
            name = obj.GetObjectSpriteName(false);
        }
        else
        {
            name = obj.GetObjectSpriteName(false);
        }

        renderer.sprite = objSprites.GetSprite(name);

        if (renderer.sprite == null)
        {
            Debug.Log("MISSING: " + name);
        }

        renderer.color = colour;
        renderer.sortingLayerName = "Foreground"; 
    }
    public void UpdateSpriteRotation(InstalledObject obj)
    {
        //do
    }
    public void Uninstall(InstalledObject obj)
    {
        obj.RemoveOnUpdateCallback(OnInstalledObjectChanged);
    }
}
