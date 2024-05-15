using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RightClickButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnRightClick;

    public Color rightClickColour;
    public float rightClickColourDurtation;

    private Button button;
    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();

        rightClickColour = Color.gray;
        rightClickColourDurtation = 0.1f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick?.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            StartCoroutine(FadeToRightClickColour());
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            StartCoroutine(FadeToNormalColour());
        }
    }

    private IEnumerator FadeToRightClickColour()
    {
        Color  originalColour = button.targetGraphic.color;
        float timeElapsed = 0;

        while(timeElapsed < rightClickColourDurtation)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / rightClickColourDurtation;
            button.targetGraphic.color = Color.Lerp(originalColour, rightClickColour, t);
            yield return null;
        }

        button.targetGraphic.color = originalColour;
    }
    private IEnumerator FadeToNormalColour()
    {
        Color originalColour = button.targetGraphic.color;
        float timeElapsed = 0;

        while (timeElapsed < rightClickColourDurtation)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / rightClickColourDurtation;
            button.targetGraphic.color = Color.Lerp(originalColour, button.colors.normalColor, t);
            yield return null;
        }

        button.targetGraphic.color = button.colors.normalColor;
    }
}
