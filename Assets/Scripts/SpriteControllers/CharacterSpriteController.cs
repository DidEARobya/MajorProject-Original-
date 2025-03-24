using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterSpriteController : MonoBehaviour
{
    [SerializeField]
    public GameObject characterPrefab;

    WorldGrid grid;
    public void Init()
    {
        grid = GameManager.GetWorldGrid();
        CharacterManager.SetCharacterCreatedCallback(OnCharacterCreated);

        int length = grid.mapSize;
        int halfLength = grid.worldCentre.x;

    }
    public void OnCharacterCreated(CharacterController character)
    {
        GameObject obj = Instantiate(characterPrefab);

        obj.name = "Character";
        obj.transform.position = new Vector3(character.x, character.y);
        obj.transform.SetParent(this.transform, true);

        character.SetCharacterObj(obj);
        character.AddCharacterUpdate(OnCharacterUpdate);
    }

    void OnCharacterUpdate(CharacterController character)
    {
        character.characterObj.transform.position = new Vector2(character.x + 0.5f, character.y);

        if (character.nextTile == null)
        {
            return;
        }

        if(character.nextTile.x >= character.x)
        {
            if(character.characterObj.transform.rotation != new Quaternion(0, 0, 0, 0))
            {
                character.characterObj.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
        }
        else
        {
            if (character.characterObj.transform.rotation != new Quaternion(0, 180, 0, 0))
            {
                character.characterObj.transform.rotation = new Quaternion(0, 180, 0, 0);
            }
        }
    }
}
