using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    List<GameObject> panels = new List<GameObject>();

    public GameObject buildPanel;
    public GameObject structuresPanel;
    public GameObject floorsPanel;
    public GameObject agriculturePanel;
    public GameObject tasksPanel;
    public GameObject testPanel;
    public GameObject zonePanel;

    public GameObject visualsPanel;
    public TextMeshProUGUI visualsText;

    public GameObject growZones;
    public GameObject regions;
    public GameObject tileDetailsPanel;
    public GameObject tileActionsPanel;

    public GameObject materialsMenu;
    public GameObject plantMenu;
    public TextMeshProUGUI devToggleText;

    private void Start()
    {
        panels.Add(structuresPanel);
        panels.Add(floorsPanel);
        panels.Add(agriculturePanel);
        panels.Add(tasksPanel);
        panels.Add(zonePanel);
        panels.Add(testPanel);

        TogglePanels(null);
        buildPanel.SetActive(false);
    }
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            buildPanel.SetActive(false);
            TogglePanels(null);
        }
    }
    void TogglePanels(GameObject toActivate)
    {
        materialsMenu.SetActive(false);
        plantMenu.SetActive(false);

        if(toActivate == buildPanel)
        {
            buildPanel.SetActive(!buildPanel.activeSelf);
        }

        if(buildPanel.activeSelf == true)
        {
            tileDetailsPanel.SetActive(false);
            tileActionsPanel.SetActive(false);
        }

        for(int i = 0; i < panels.Count; i++)
        {
            if(toActivate != null && panels[i] == toActivate)
            {
                panels[i].SetActive(!panels[i].activeSelf);
                continue;
            }

            panels[i].SetActive(false);
        }
    }
    public void ToggleBuildPanel()
    {
        TogglePanels(buildPanel);
    }
    public void ToggleStructuresPanel()
    {
        TogglePanels(structuresPanel);
    }
    public void ToggleFloorsPanel()
    {
        TogglePanels(floorsPanel);
    }
    public void ToggleAgriculturePanel()
    {
        TogglePanels(agriculturePanel);
    }
    public void ToggleTasksPanel()
    {
        TogglePanels(tasksPanel);
    }
    public void ToggleTestPanel()
    {
        TogglePanels(testPanel);
    }
    public void ToggleZonePanel()
    {
        TogglePanels(zonePanel);
    }
    public void ToggleGrowZones()
    {
        growZones.SetActive(!growZones.activeSelf);
    }
    public void ToggleRegions()
    {
        GameManager.instance.mouseController.ToggleDisplayRegions();
        regions.SetActive(!regions.activeSelf);
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
    public void ToggleTileDetailsPanel()
    {
        tileDetailsPanel.SetActive(!tileDetailsPanel.activeSelf);

        if(tileDetailsPanel.activeSelf == true)
        {
            buildPanel.SetActive(false);
            TogglePanels(null);
        }
    }
    public void DisplayTileActionsPanel(Tile tile)
    {
        tileActionsPanel.SetActive(true);
        tileActionsPanel.GetComponent<TileActionsPanel>().Init(tile);
        tileDetailsPanel.SetActive(true);
        tileDetailsPanel.GetComponent<TileDetails>().SetDisplayedTile(tile);

        if (tileDetailsPanel.activeSelf == true)
        {
            buildPanel.SetActive(false);
            TogglePanels(null);
        }
    }
    public void ToggleDevMode()
    {
        GameManager.instance.devMode = !GameManager.instance.devMode;

        if(GameManager.instance.devMode == false)
        {
            devToggleText.text = "Dev: OFF";
        }
        else
        {
            devToggleText.text = "Dev: ON";
        }
    }
}
