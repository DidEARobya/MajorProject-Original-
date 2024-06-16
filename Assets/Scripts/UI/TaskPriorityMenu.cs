using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TaskPriorityMenu : MonoBehaviour
{
    [SerializeField]
    public List<Button> priorityButtons;
    List<PriorityLevel> priorityLevels;
    CharacterController characterController;

    public void Init(CharacterController character)
    {
        characterController = character;
        priorityLevels = character.priorityDict.levelList;

        for(int i = 0; i < priorityButtons.Count; i++)
        {
            Button button = priorityButtons[i];

            button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = ((int)priorityLevels[i] + 1).ToString();
            button.onClick.AddListener(() => { UpdatePriority(button); }) ;
        }
    }
    public void Display()
    {
        for (int i = 0; i < priorityButtons.Count; i++)
        {
            priorityButtons[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = ((int)priorityLevels[i] + 1).ToString();
        }
    }
    void UpdatePriority(Button button)
    {
        TextMeshProUGUI textGUI = button.gameObject.GetComponentInChildren<TextMeshProUGUI>();

        switch (textGUI.text)
        {
            case ("1"):
                textGUI.text = "2";
                priorityLevels[priorityButtons.IndexOf(button)] = PriorityLevel.TWO;
                break;
            case ("2"):
                textGUI.text = "3";
                priorityLevels[priorityButtons.IndexOf(button)] = PriorityLevel.THREE;
                break;
            case ("3"):
                textGUI.text = "4";
                priorityLevels[priorityButtons.IndexOf(button)] = PriorityLevel.FOUR;
                break;
            case ("4"):
                textGUI.text = "1";
                priorityLevels[priorityButtons.IndexOf(button)] = PriorityLevel.ONE;
                break;
        }

        characterController.priorityDict.levelList = priorityLevels;
    }
}
