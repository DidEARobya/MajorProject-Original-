using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    [SerializeField]
    public Sprite characterSprite;

    WorldGrid world;
    public void Init()
    {
        world = GameManager.GetWorldController().worldGrid;
        world.SetCharacterCreatedCallback(OnCharacterCreated);

        CharacterController test = world.CreateCharacter(world.GetTile(world.mapWidth / 2, world.mapHeight / 2));
        CharacterController test2 = world.CreateCharacter(world.GetTile(world.mapWidth / 2 + 5, world.mapHeight / 2));
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
        GameObject charObj = character.characterObj;

        charObj.transform.position = new Vector2(character.x, character.y);
    }
}
