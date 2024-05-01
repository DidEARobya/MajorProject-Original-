using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    [SerializeField]
    public Sprite characterSprite;

    WorldGrid grid;
    public void Init()
    {
        grid = GameManager.GetWorldGrid();
        CharacterManager.SetCharacterCreatedCallback(OnCharacterCreated);

        int length = grid.mapWidth;
        int halfLength = grid.worldCentre.x;

        CharacterController test = CharacterManager.CreateCharacter(grid.GetTile(halfLength, halfLength));
        CharacterController test2 = CharacterManager.CreateCharacter(grid.GetTile(halfLength + 1, halfLength + 1));
        CharacterController test3 = CharacterManager.CreateCharacter(grid.GetTile(halfLength + 2, halfLength - 1));
        //CharacterController test4 = CharacterManager.CreateCharacter(grid.GetTile(halfLength + 2, halfLength - 4));
        //CharacterController test5 = CharacterManager.CreateCharacter(grid.GetTile(halfLength + 5, halfLength - 1));

    }
    public void OnCharacterCreated(CharacterController character)
    {
        GameObject obj = new GameObject();

        obj.name = "Character";
        obj.transform.position = new Vector3(character.x, character.y);
        obj.transform.SetParent(this.transform, true);

        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = characterSprite;
        renderer.sortingLayerName = "Characters";

        character.SetCharacterObj(obj);
        character.AddCharacterUpdate(OnCharacterUpdate);
    }

    void OnCharacterUpdate(CharacterController character)
    {
        character.characterObj.transform.position = new Vector2(character.x, character.y);
    }
}
