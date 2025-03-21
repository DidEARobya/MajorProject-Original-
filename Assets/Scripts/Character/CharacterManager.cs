using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterManager
{
    public static List<CharacterController> characters = new List<CharacterController>();
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
        GameManager.instance.uiManager.CreateCharacterWorkPanel(test);
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
            character.ignoredTaskSites.Clear();
        }
    }
    public static void DisplayWorkMenu()
    {
        foreach (CharacterController character in characters)
        {
            character.priorityDisplay.GetComponent<TaskPriorityMenu>().Display();
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
