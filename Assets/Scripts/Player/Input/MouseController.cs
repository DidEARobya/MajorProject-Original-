using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using Cinemachine;
using TMPro.EditorUtilities;
using Cinemachine.Utility;

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
    CLEAR,
    DESTROY
}
public class MouseController : MonoBehaviour
{
    public new Camera camera;
    public new CinemachineVirtualCamera vCamera;

    WorldController worldController;
    public WorldGrid grid;
    BuildModeController buildModeController;

    public MouseMode mouseMode = MouseMode.AREA;
    public BuildMode buildMode = BuildMode.FLOOR;
    public InstalledObjectTypes toBuild;

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
        selectionPool = new ObjectPool(200, tileBoxPrefab, this.transform);

        worldController = GameManager.GetWorldController();
        grid = worldController.worldGrid;
        buildModeController = BuildModeController.instance;
    }

    public void Init(WorldGrid _grid)
    {
        grid = _grid;

        isReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isReady == false) 
        {
            return;
        }

        if(Input.GetKeyUp(KeyCode.I))
        {
            mouseMode = MouseMode.AREA;
        }
        if(Input.GetKeyUp(KeyCode.O))
        {
            mouseMode = MouseMode.SINGLE;
        }

        if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            Debug.Log("ClearMode");
            buildMode = BuildMode.CLEAR;
        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            Debug.Log("FloorMode");
            buildMode = BuildMode.FLOOR;
        }
        if(Input.GetKeyUp(KeyCode.Alpha2))
        {
            Debug.Log("ObjectMode");
            buildMode = BuildMode.OBJECT;
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            Debug.Log("DestroyMode");
            buildMode = BuildMode.DESTROY;
        }

        if(buildMode == BuildMode.OBJECT)
        {
            if(Input.GetKeyUp(KeyCode.N))
            {
                Debug.Log("Wall");
                toBuild = InstalledObjectTypes.WALL;
            }
            if (Input.GetKeyUp(KeyCode.M))
            {
                Debug.Log("Door");
                toBuild = InstalledObjectTypes.DOOR;
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

        lastMousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        lastMousePos.z = 0;

        vCamera.m_Lens.OrthographicSize -= vCamera.m_Lens.OrthographicSize * Input.GetAxis("Mouse ScrollWheel");
        vCamera.m_Lens.OrthographicSize = Mathf.Clamp(vCamera.m_Lens.OrthographicSize, 4, 15);
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
                        buildModeController.Build(temp, buildMode, toBuild);
                    }

                }
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
                buildModeController.Build(temp, buildMode, toBuild);
            }
        }
    }
}
