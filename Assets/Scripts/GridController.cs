using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridController : MonoBehaviour
{
    public int gridWidth, gridHeight;
    public float gridScale;
    public Vector3 gridOffset;
    public BuildingController [,] buildingsGrid;
    public GameObject buildingPrefab;
    public static GridController instance;
    public void Awake()
    {
        instance = this;
        buildingsGrid = new BuildingController[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                buildingsGrid[x, y] = Instantiate(buildingPrefab, GetWorldPosition(new Vector2Int(x, y)), buildingPrefab.transform.rotation, transform).GetComponent<BuildingController>();
                buildingsGrid[x, y].gridPosition = new Vector2Int(x, y);
                buildingsGrid[x, y].GetComponent<BoxCollider>().size = new Vector3(gridScale, gridScale, 1);
            }
        }
    }

    public BuildingController GetBuilding(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight)
        {
            return buildingsGrid[x, y];
        }
        else
        {
            Debug.Log("GetGridObject: Grid position out of bounds: " + x + ", " + y);
            return null;
        }
    }

    public GameObject SetBuilding(int x, int y, BuildingController.BuildingType buildingType, Quaternion buildingRotation)
    {
        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight)
        {
            if(buildingsGrid[x, y].buildingType != BuildingController.BuildingType.NONE)
            {
                buildingsGrid[x, y].DestroyBuilding();
            }
            return buildingsGrid[x, y].BuildBuilding(buildingType, buildingRotation);
        }
        else
        {
            Debug.Log("SetBuildingTest: Grid position out of bounds: " + x + ", " + y);
            return null;
        }
    }
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt((worldPosition.x - gridOffset.x) / gridScale), Mathf.RoundToInt((worldPosition.z - gridOffset.y) / gridScale));
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * gridScale, 0, gridPosition.y * gridScale) + gridOffset;
    }
    public bool isBuilding = false;
    public Vector2Int buildingPreviewPosition;
    public void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isBuilding)
        {
            isBuilding = true;
        }
        else if(isBuilding)
        {
            RaycastHit hit;
            if(!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f))
            {
                return;
            }

            Vector3 worldPosition = hit.point;
            Vector2Int gridPosition = GetGridPosition(worldPosition);
            
            if (!(gridPosition.x >= 0 && gridPosition.y >= 0 && gridPosition.x < gridWidth && gridPosition.y < gridHeight))
            {
                return;
            }

            if(Input.GetMouseButtonDown(1))
            {
                isBuilding = false;
                buildingPreviewPosition = new Vector2Int(-1, -1);
                SetBuilding(gridPosition.x, gridPosition.y, BuildingController.BuildingType.TOWN_HALL, Quaternion.identity);
            }
            
            if(buildingsGrid[gridPosition.x, gridPosition.y].buildingType == BuildingController.BuildingType.NONE && buildingPreviewPosition != gridPosition)
            {
                SetBuilding(buildingPreviewPosition.x, buildingPreviewPosition.y, BuildingController.BuildingType.NONE, Quaternion.identity);
                buildingPreviewPosition = gridPosition;
                SetBuilding(gridPosition.x, gridPosition.y, BuildingController.BuildingType.TOWN_HALL, Quaternion.identity).transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(3, 3, 3, 0.5f);
            }
        }
    }
}