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

public enum MouseMode
{
    AREA,
    ROW,
    SINGLE
}
public enum BuildMode
{
    FLOOR,
    OBJECT,
    CLEAR_FLOOR,
    DESTROY,
    GENERATE,
    SPAWNCHARACTER,
    CANCEL
}
public class MouseController : MonoBehaviour
{
    public new Camera camera;
    public CinemachineVirtualCamera vCamera;
    CinemachineConfiner2D confiner;

    WorldController worldController;
    public WorldGrid grid;
    BuildModeController buildModeController;

    public MouseMode mouseMode = MouseMode.AREA;
    public BuildMode buildMode = BuildMode.FLOOR;

    public FurnitureTypes toBuild;
    public ItemTypes toBuildMaterial;
    public FloorTypes floorType;
    public ItemTypes toGenerate;

    protected Tile tileUnderMouse;

    public GameObject tileBoxPrefab;
    private List<GameObject> selectionPrefabs = new List<GameObject>();
    private ObjectPool selectionPool;

    protected Vector2 selectionDragStart;
    protected Vector2 selectionDragEnd;

    private Vector3 currentMousePos;
    private Vector3 lastMousePos;

    private int startX;
    private int startY;
    private int endX;
    private int endY;

    private bool isReady = false;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        vCamera = camera.GetComponentInChildren<CinemachineVirtualCamera>();
        confiner = vCamera.GetComponent<CinemachineConfiner2D>();

        selectionPool = new ObjectPool(200, tileBoxPrefab, transform);

        worldController = GameManager.GetWorldController();
        grid = worldController.worldGrid;
        buildModeController = BuildModeController.instance;
    }

    public void Init(WorldGrid _grid)
    {
        grid = _grid;

        isReady = true;
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
        toBuild = null;
        toBuildMaterial = null;
    }
    public void SetFloor(FloorTypes floor, ItemTypes material, MouseMode mode)
    {
        buildMode = BuildMode.FLOOR;
        mouseMode = mode;
        floorType = floor;
        toBuildMaterial = material;
    }
    public void SetToClearFloor()
    {
        mouseMode = MouseMode.AREA;
        buildMode = BuildMode.CLEAR_FLOOR;
    }
    // Update is called once per frame
    void Update()
    {
        if(isReady == false) 
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            Debug.Log("CancelMode");
            buildMode = BuildMode.CANCEL;
        }
        if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            Debug.Log("GenerateMode");
            buildMode = BuildMode.GENERATE;
        }
        if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            Debug.Log("SpawnCharacter");
            buildMode = BuildMode.SPAWNCHARACTER;
        }

        if (buildMode == BuildMode.GENERATE)
        {
            toBuild = null;

            if (Input.GetKeyUp(KeyCode.B))
            {
                Debug.Log("Wood");
                toGenerate = ItemTypes.WOOD;
            }
            if (Input.GetKeyUp(KeyCode.N))
            {
                Debug.Log("Stone");
                toGenerate = ItemTypes.STONE;
            }
            if (Input.GetKeyUp(KeyCode.M))
            {
                Debug.Log("Iron");
                toGenerate = ItemTypes.IRON;
            }
        }
        UpdateMousePos();

        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
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

        if(Input.GetKeyUp(KeyCode.Z))
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

        while (selectionPrefabs.Count > 0)
        {
            selectionPool.DespawnObject(selectionPrefabs[0]);
            selectionPrefabs.RemoveAt(0);
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
                        GameObject tileBox = selectionPool.SpawnObject(new Vector3(x, y, 0), Quaternion.identity);
                        selectionPrefabs.Add(tileBox);
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
                        buildModeController.Build(temp, buildMode, toBuild, floorType, toBuildMaterial, toGenerate);
                    }

                }
            } 
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

        while (selectionPrefabs.Count > 0)
        {
            selectionPool.DespawnObject(selectionPrefabs[0]);
            selectionPrefabs.RemoveAt(0);
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
                        GameObject tileBox = selectionPool.SpawnObject(new Vector3(x, startY, 0), Quaternion.identity);
                        selectionPrefabs.Add(tileBox);
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
                        buildModeController.Build(temp, buildMode, toBuild, floorType, toBuildMaterial, toGenerate);
                    }
                }
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
                        GameObject tileBox = selectionPool.SpawnObject(new Vector3(startX, y, 0), Quaternion.identity);
                        selectionPrefabs.Add(tileBox);
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
                        buildModeController.Build(temp, buildMode, toBuild, floorType, toBuildMaterial, toGenerate);
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                Tile temp = grid.GetTile(currentMousePos.x, currentMousePos.y);

                if (temp != null)
                {
                    buildModeController.Build(temp, buildMode, toBuild, floorType, toBuildMaterial, toGenerate);
                }

                temp = null;
            }
        }
    }
    private void SingleBuildMode()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Tile temp = grid.GetTile(currentMousePos.x, currentMousePos.y);

            if (temp != null)
            {
                buildModeController.Build(temp, buildMode, toBuild, floorType, toBuildMaterial, toGenerate);
            }
        }
    }
}
