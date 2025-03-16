using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
using TMPro;
using System.Linq.Expressions;

public enum MouseMode
{
    AREA,
    ROW,
    SINGLE
}
public enum BuildMode
{
    SELECT,
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

    public UIManager uiManager;

    public TileDetails tileDetails;

    public MouseMode mouseMode;
    public BuildMode buildMode;

   public string toBuild;
    public ItemData toBuildMaterial;
    public FloorTypes floorType;

    protected Tile tileUnderMouse;
    protected Region highlightedRegion;

    protected Tile selectedTile;

    HashSet<Tile> selected = new HashSet<Tile>();

    public GameObject selectedTileDisplay;
    public GameObject tileOutline;

    public TextMeshProUGUI modeText;

    protected Vector2 selectionDragStart;
    protected Vector2 selectionDragEnd;

    private Vector3 currentMousePos;
    private Vector3 lastMousePos;

    private int startX;
    private int startY;
    private int endX;
    private int endY;

    private bool isReady = false;
    private bool displayRegions = false;

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

        SetToSelect();
    }

    public void Init(WorldGrid _grid)
    {
        grid = _grid;
        isReady = true;
    }
    public void SetToSelect()
    {
        ResetSelected();

        mouseMode = MouseMode.SINGLE;
        buildMode = BuildMode.SELECT;

        UpdateText();
    }
    void ResetSelected()
    {
        if (buildMode != BuildMode.SELECT)
        {
            return;
        }

        selectedTile = null;
        selectedTileDisplay.SetActive(false);
    }
    public void SetObject(string building, MouseMode mode)
    {
        ResetSelected();

        buildMode = BuildMode.OBJECT;
        mouseMode = mode;
        toBuild = building;

        UpdateText();
    }
    public void SetToDestroy()
    {
        ResetSelected();

        mouseMode = MouseMode.AREA;
        buildMode = BuildMode.DESTROY;

        UpdateText();
    }
    public void SetToMine()
    {
        ResetSelected();

        mouseMode = MouseMode.AREA;
        buildMode = BuildMode.MINE;

        UpdateText();
    }
    public void SetToHarvest()
    {
        ResetSelected();

        mouseMode = MouseMode.AREA;
        buildMode = BuildMode.HARVEST;

        UpdateText();
    }
    public void SetFloor(FloorTypes floor, ItemData material, MouseMode mode)
    {
        ResetSelected();

        buildMode = BuildMode.FLOOR;
        mouseMode = mode;
        floorType = floor;
        toBuildMaterial = material;

        UpdateText();
    }
    public void SetToGrowZoneMode(bool _toAdd)
    {
        ResetSelected();

        buildMode = BuildMode.ZONE;
        mouseMode = MouseMode.AREA;
        zoneType = ZoneType.GROW;
        toAdd = _toAdd;

        UpdateText();
    }
    public void SetToStorageZoneMode(bool _toAdd)
    {
        ResetSelected();

        buildMode = BuildMode.ZONE;
        mouseMode = MouseMode.AREA;
        zoneType = ZoneType.STORAGE;
        toAdd = _toAdd;

        UpdateText();
    }
    public void SetToClearFloor()
    {
        ResetSelected();

        mouseMode = MouseMode.AREA;
        buildMode = BuildMode.CLEAR_FLOOR;

        UpdateText();
    }
    public void SetToCancel()
    {
        ResetSelected();

        mouseMode = MouseMode.AREA;
        buildMode = BuildMode.CANCEL;

        UpdateText();
    }
    public void SetToSpawnCharacter()
    {
        ResetSelected();

        mouseMode = MouseMode.SINGLE;
        buildMode = BuildMode.SPAWNCHARACTER;

        UpdateText();
    }
    public void ToggleDisplayRegions()
    {
        displayRegions = !displayRegions;

        if(displayRegions == false && highlightedRegion != null)
        {
            highlightedRegion.DestroyDisplayTiles(false);
        }
    }

    void UpdateText()
    {
        modeText.text = "Mode: " + buildMode.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        if(isReady == false) 
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            SetToSelect();
        }

        UpdateMousePos();

        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            if(tileUnderMouse != null)
            {
                tileOutline.transform.position = tileUnderMouse.tileObj.transform.position;

                if (selectedTile == null)
                {
                    tileDetails.SetDisplayedTile(tileUnderMouse);
                }
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

        if(displayRegions == true && tileUnderMouse != null && tileUnderMouse.region != highlightedRegion)
        {
            if(highlightedRegion != null)
            {
                highlightedRegion.DestroyDisplayTiles(false);
            }

            highlightedRegion = tileUnderMouse.region;

            if (highlightedRegion != null)
            {
                highlightedRegion.HighlightTiles(UnityEngine.Color.yellow, false);
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
                if(buildMode == BuildMode.SELECT)
                {
                    selectedTile = temp;

                    selectedTileDisplay.transform.position = selectedTile.gameObj.transform.position;
                    selectedTileDisplay.SetActive(true);

                    uiManager.DisplayTileActionsPanel(selectedTile);
                }
                else
                {
                    temp.SetSelected(true);
                    selected.Add(temp);
                    SelectedFunctions(selected);
                }
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
                            GameManager.GetZoneManager().AddTile(tile, zoneType);
                        }
                    }
                    else
                    {
                        foreach (Tile tile in temp)
                        {
                            GameManager.GetZoneManager().RemoveTile(tile, zoneType);
                        }
                    }
                    break;

                case BuildMode.OBJECT:
                    buildModeController.Build(temp, buildMode, toBuild);
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
