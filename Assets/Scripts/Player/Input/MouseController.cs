using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using Cinemachine;
using TMPro.EditorUtilities;
using Cinemachine.Utility;
using System.ComponentModel;
using static TMPro.Examples.TMP_ExampleScript_01;
using System;
using UnityEngine.UIElements;

public enum MouseMode
{
    AREA,
    ROW,
    SINGLE
}
public enum BuildMode
{
    SELECTED,
    FLOOR,
    OBJECT,
    CLEAR_FLOOR,
    DESTROY,
    MINE,
    HARVEST,
    SPAWNCHARACTER,
    CANCEL,
    ZONE
}
public class MouseController : MonoBehaviour
{
    public new Camera camera;
    public CinemachineVirtualCamera vCamera;
    CinemachineConfiner2D confiner;

    WorldController worldController;
    public WorldGrid grid;
    BuildModeController buildModeController;

    public TileDetails tileDetails;

    public MouseMode mouseMode = MouseMode.SINGLE;
    public BuildMode buildMode = BuildMode.SELECTED;

    public FurnitureTypes toBuild;
    public ItemTypes toBuildMaterial;
    public FloorTypes floorType;

    protected Tile tileUnderMouse;
    HashSet<Tile> selected = new HashSet<Tile>();

    public GameObject tileOutline;

    protected Vector2 selectionDragStart;
    protected Vector2 selectionDragEnd;

    private Vector3 currentMousePos;
    private Vector3 lastMousePos;

    private int startX;
    private int startY;
    private int endX;
    private int endY;

    private bool isReady = false;

    bool toAdd;
    ZoneType zoneType;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        vCamera = camera.GetComponentInChildren<CinemachineVirtualCamera>();
        confiner = vCamera.GetComponent<CinemachineConfiner2D>();

        worldController = GameManager.GetWorldController();
        grid = worldController.worldGrid;
        buildModeController = BuildModeController.instance;
    }

    public void Init(WorldGrid _grid)
    {
        grid = _grid;
        isReady = true;
    }
    public void SetToSelect()
    {
        mouseMode = MouseMode.SINGLE;
        buildMode = BuildMode.SELECTED;
    }
    public void SetObject(FurnitureTypes obj, ItemTypes material, MouseMode mode)
    {
        buildMode = BuildMode.OBJECT;
        mouseMode = mode;
        toBuild = obj;
        toBuildMaterial = material;
    }
    public void SetToDestroy()
    {
        mouseMode = MouseMode.AREA;
        buildMode = BuildMode.DESTROY;
    }
    public void SetToMine()
    {
        mouseMode = MouseMode.AREA;
        buildMode = BuildMode.MINE;
    }
    public void SetToHarvest()
    {
        mouseMode = MouseMode.AREA;
        buildMode = BuildMode.HARVEST;
    }
    public void SetFloor(FloorTypes floor, ItemTypes material, MouseMode mode)
    {
        buildMode = BuildMode.FLOOR;
        mouseMode = mode;
        floorType = floor;
        toBuildMaterial = material;
    }
    public void SetToGrowZoneMode(bool _toAdd)
    {
        buildMode = BuildMode.ZONE;
        mouseMode = MouseMode.AREA;
        zoneType = ZoneType.GROW;
        toAdd = _toAdd;
    }
    public void SetToStorageZoneMode(bool _toAdd)
    {
        buildMode = BuildMode.ZONE;
        mouseMode = MouseMode.AREA;
        zoneType = ZoneType.STORAGE;
        toAdd = _toAdd;
    }
    public void SetToClearFloor()
    {
        mouseMode = MouseMode.AREA;
        buildMode = BuildMode.CLEAR_FLOOR;
    }
    public void SetToCancel()
    {
        mouseMode= MouseMode.AREA;
        buildMode = BuildMode.CANCEL;
    }
    public void SetToSpawnCharacter()
    {
        mouseMode = MouseMode.SINGLE;
        buildMode = BuildMode.SPAWNCHARACTER;
    }
    // Update is called once per frame
    void Update()
    {
        if(isReady == false) 
        {
            return;
        }

        UpdateMousePos();

        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            if(tileUnderMouse != null)
            {
                tileOutline.transform.position = tileUnderMouse.tileObj.transform.position;
                tileDetails.SetDisplayedTile(tileUnderMouse);
            }

            MouseInputs();
        }
    }
    private void UpdateMousePos()
    {
        currentMousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        currentMousePos.z = 0;

        tileUnderMouse = grid.GetTile(currentMousePos.x, currentMousePos.y);

        if (Input.GetMouseButton(2))
        {
            Vector3 difference = lastMousePos - currentMousePos;
            camera.transform.Translate(difference);
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            if(tileUnderMouse.region != null)
            {
                tileUnderMouse.region.SetTiles(TerrainTypes.GOOD_SOIL, false);
            }
        }
        lastMousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        lastMousePos.z = 0;

        float val = Input.GetAxis("Mouse ScrollWheel");

        if (val != 0)
        {
            vCamera.m_Lens.OrthographicSize -= vCamera.m_Lens.OrthographicSize * val;
            vCamera.m_Lens.OrthographicSize = Mathf.Clamp(vCamera.m_Lens.OrthographicSize, 4, 15);

            confiner.InvalidateCache();
        }
    }
    private void MouseInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionDragStart = worldController.GetWorldToCell(currentMousePos);
        }

        selectionDragEnd = worldController.GetWorldToCell(currentMousePos);

        switch(mouseMode)
        {
            case MouseMode.AREA:
                AreaBuildMode();
                break;
            case MouseMode.ROW:
                RowBuildMode();
                break;
            case MouseMode.SINGLE:
                SingleBuildMode();
                break;
        }
    }
    private void AreaBuildMode()
    {
        startX = Mathf.FloorToInt(selectionDragStart.x);
        endX = Mathf.FloorToInt(selectionDragEnd.x);

        if (endX < startX)
        {
            int temp = endX;
            endX = startX;
            startX = temp;
        }

        startY = Mathf.FloorToInt(selectionDragStart.y);
        endY = Mathf.FloorToInt(selectionDragEnd.y);

        if (endY < startY)
        {
            int temp = endY;
            endY = startY;
            startY = temp;
        }

        if(selected.Count > 0)
        {
            foreach(Tile tile in selected)
            {
                tile.SetSelected(false);
            }

            selected.Clear();
        }

        if (Input.GetMouseButton(0))
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Tile temp = grid.GetTile(x, y);

                    if (temp != null)
                    {
                        temp.SetSelected(true);
                        selected.Add(temp);
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Tile temp = grid.GetTile(x, y);

                    if (temp != null)
                    {
                        selected.Add(temp);
                    }
                }
            }

            SelectedFunctions(selected);
        }
    }
    private void RowBuildMode()
    {
        bool xRow = false;
        bool yRow = false;

        startX = Mathf.FloorToInt(selectionDragStart.x);
        endX = Mathf.FloorToInt(selectionDragEnd.x);

        startY = Mathf.FloorToInt(selectionDragStart.y);
        endY = Mathf.FloorToInt(selectionDragEnd.y);

        if (endX != startX && yRow == false)
        {
            xRow = true;
        }
        else
        {
            xRow = false;
        }

        if (endY != startY && xRow == false)
        {
            yRow = true;
        }
        else
        {
            yRow = false;
        }

        if (selected.Count > 0)
        {
            foreach (Tile tile in selected)
            {
                tile.SetSelected(false);
            }

            selected.Clear();
        }

        if (xRow == true && yRow == false)
        {
            if (endX < startX)
            {
                int temp = endX;
                endX = startX;
                startX = temp;
            }

            if (Input.GetMouseButton(0))
            {
                for (int x = startX; x <= endX; x++)
                {
                    Tile temp = grid.GetTile(x, startY);

                    if (temp != null)
                    {
                        temp.SetSelected(true);
                        selected.Add(temp);
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                for (int x = startX; x <= endX; x++)
                {
                    Tile temp = grid.GetTile(x, startY);

                    if (temp != null)
                    {
                        selected.Add(temp);
                    }
                }

                SelectedFunctions(selected);
            }
        }
        else if (xRow == false && yRow == true)
        {
            if (endY < startY)
            {
                int temp = endY;
                endY = startY;
                startY = temp;
            }

            if (Input.GetMouseButton(0))
            {
                for (int y = startY; y <= endY; y++)
                {
                    Tile temp = grid.GetTile(startX, y);

                    if (temp != null)
                    {
                        temp.SetSelected(true);
                        selected.Add(temp);
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                for (int y = startY; y <= endY; y++)
                {
                    Tile temp = grid.GetTile(startX, y);

                    if (temp != null)
                    {
                        selected.Add(temp);
                    }
                }

                SelectedFunctions(selected);
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                for (int x = startX; x <= endX; x++)
                {
                    Tile temp = grid.GetTile(x, startY);

                    if (temp != null)
                    {
                        temp.SetSelected(true);
                        selected.Add(temp);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Tile temp = grid.GetTile(currentMousePos.x, currentMousePos.y);

                if (temp != null)
                {
                    selected.Add(temp);
                    SelectedFunctions(selected);
                }
            }
        }
    }
    private void SingleBuildMode()      
    {
        if (selected.Count > 0)
        {
            foreach (Tile tile in selected)
            {
                tile.SetSelected(false);
            }

            selected.Clear();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Tile temp = grid.GetTile(currentMousePos.x, currentMousePos.y);

            if (temp != null)
            {
                temp.SetSelected(true);
                selected.Add(temp);
                SelectedFunctions(selected);
            }
        }
    }
    void SelectedFunctions(HashSet<Tile> temp)
    {
        if (temp != null)
        {
            switch (buildMode)
            {
                case BuildMode.CLEAR_FLOOR:
                    buildModeController.ClearFloor(temp);
                    break;

                case BuildMode.ZONE:
                    if (toAdd == true)
                    {
                        foreach (Tile tile in temp)
                        {
                            ZoneManager.AddTile(tile, zoneType);
                        }
                    }
                    else
                    {
                        foreach (Tile tile in temp)
                        {
                            ZoneManager.RemoveTile(tile, zoneType);
                        }
                    }
                    break;

                case BuildMode.OBJECT:
                    buildModeController.Build(temp, buildMode, toBuild, toBuildMaterial);
                    break;

                case BuildMode.FLOOR:
                    buildModeController.BuildFloor(temp, floorType);
                    break;

                case BuildMode.DESTROY:
                    buildModeController.DestroyObject(temp);
                    break;

                case BuildMode.MINE:
                    buildModeController.MineOre(temp);
                    break;

                case BuildMode.HARVEST:
                    buildModeController.Harvest(temp);
                    break;

                case BuildMode.CANCEL:
                    buildModeController.CancelTask(temp);
                    break;
                case BuildMode.SPAWNCHARACTER:
                    buildModeController.SpawnCharacter(temp);
                    break;
            }
        }
    }
}
