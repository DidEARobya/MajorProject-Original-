using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterManager
{
    static List<CharacterController> characters = new List<CharacterController>();
    static Action<CharacterController> characterCreatedCallback;

    public static void Update(float deltaTime)
    {
        foreach (CharacterController character in characters)
        {
            character.Update(deltaTime);
        }
    }
    public static CharacterController CreateCharacter(Tile tile)
    {
        CharacterController test = new CharacterController(tile);
        characters.Add(test);

        if (characterCreatedCallback != null)
        {
            characterCreatedCallback(test);
        }

        return test;
    }

    public static void ResetCharacterTaskIgnores()
    {
        foreach (CharacterController character in characters)
        {
            character.ignoredTasks.Clear();
        }
    }
    public static void SetCharacterCreatedCallback(Action<CharacterController> callback)
    {
        characterCreatedCallback += callback;
    }
    public static void RemoveCharacterCreatedCallback(Action<CharacterController> callback)
    {
        characterCreatedCallback -= callback;
    }
}
