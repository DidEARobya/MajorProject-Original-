using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject buildPanel;
    public GameObject structuresPanel;
    public GameObject floorsPanel;

    public void ToggleBuildPanel()
    {
        buildPanel.SetActive(!buildPanel.activeSelf);
        structuresPanel.SetActive(false);
        floorsPanel.SetActive(false);
    }
    public void ToggleStructuresPanel()
    {
        structuresPanel.SetActive(!structuresPanel.activeSelf);
        floorsPanel.SetActive(false);
    }
    public void ToggleFloorsPanel()
    {
        floorsPanel.SetActive(!floorsPanel.activeSelf);
        structuresPanel.SetActive(false);
    }
}
