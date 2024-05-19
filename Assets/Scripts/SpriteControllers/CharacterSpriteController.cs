using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterSpriteController : MonoBehaviour
{
    [SerializeField]
    public Sprite characterSprite;

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
        GameObject obj = new GameObject();

        obj.name = "Character";
        obj.transform.position = new Vector3(character.x, character.y);
        obj.transform.SetParent(this.transform, true);

        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = characterSprite;
        renderer.sortingLayerName = "Foreground";

        character.SetCharacterObj(obj);
        character.AddCharacterUpdate(OnCharacterUpdate);
    }

    void OnCharacterUpdate(CharacterController character)
    {
        character.characterObj.transform.position = new Vector2(character.x, character.y);
    }
}
