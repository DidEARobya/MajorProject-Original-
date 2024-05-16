using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject buildPanel;
    public GameObject structuresPanel;
    public GameObject floorsPanel;
    public GameObject agriculturePanel;
    public GameObject tasksPanel;
    public GameObject testPanel;

    public GameObject visualsPanel;
    public TextMeshProUGUI visualsText;

    public GameObject growZones;

    public void ToggleBuildPanel()
    {
        buildPanel.SetActive(!buildPanel.activeSelf);
        structuresPanel.SetActive(false);
        floorsPanel.SetActive(false);
        agriculturePanel.SetActive(false);
        tasksPanel.SetActive(false);
        testPanel.SetActive(false);
    }
    public void ToggleStructuresPanel()
    {
        structuresPanel.SetActive(!structuresPanel.activeSelf);
        floorsPanel.SetActive(false);
        agriculturePanel.SetActive(false);
        tasksPanel.SetActive(false);
        testPanel.SetActive(false);
    }
    public void ToggleFloorsPanel()
    {
        floorsPanel.SetActive(!floorsPanel.activeSelf);
        structuresPanel.SetActive(false);
        agriculturePanel.SetActive(false);
        tasksPanel.SetActive(false);
        testPanel.SetActive(false);
    }
    public void ToggleAgriculturePanel()
    {
        agriculturePanel.SetActive(!agriculturePanel.activeSelf);
        structuresPanel.SetActive(false);
        floorsPanel.SetActive(false);
        tasksPanel.SetActive(false);
        testPanel.SetActive(false);
    }
    public void ToggleTasksPanel()
    {
        tasksPanel.SetActive(!tasksPanel.activeSelf);
        structuresPanel.SetActive(false);
        floorsPanel.SetActive(false);
        agriculturePanel.SetActive(false);
        testPanel.SetActive(false);
    }
    public void ToggleTestPanel()
    {
        testPanel.SetActive(!testPanel.activeSelf);
        structuresPanel.SetActive(false);
        floorsPanel.SetActive(false);
        agriculturePanel.SetActive(false);
        tasksPanel.SetActive(false);
    }
    public void ToggleGrowZones()
    {
        growZones.SetActive(!growZones.activeSelf);
    }
    public void ToggleVisualsPanel()
    {
        visualsPanel.SetActive(!visualsPanel.activeSelf);

        if(visualsPanel.activeSelf == false)
        {
            visualsText.text = "Open";
        }
        else
        {
            visualsText.text = "Close";
        }
    }
}
