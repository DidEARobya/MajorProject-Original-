using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject buildPanel;
    public GameObject deconstructPanel;
    public GameObject itemPanel;
    public GameObject tasksPanel;

    public BuildMode buildMode;
    public FurnitureTypes toBuild;
    public ItemTypes toGenerate;

    public void ShowBuildPanel()
    {
        buildPanel.SetActive(true);
        deconstructPanel.SetActive(false);
        itemPanel.SetActive(false);
        tasksPanel.SetActive(false);
    }
    public void ShowDeconstructPanel()
    {
        deconstructPanel.SetActive(true);
        buildPanel.SetActive(false);
        itemPanel.SetActive(false);
        tasksPanel.SetActive(false);
    }
    public void ShowItemPanel()
    {
        itemPanel.SetActive(true);
        buildPanel.SetActive(false);
        deconstructPanel.SetActive(false);
        tasksPanel.SetActive(false);
    }
    public void ShowTasksPanel()
    {
        tasksPanel.SetActive(true);
        buildPanel.SetActive(false);
        deconstructPanel.SetActive(false);
        itemPanel.SetActive(false);
    }
}
