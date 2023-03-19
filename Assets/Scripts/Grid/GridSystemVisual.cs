using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [Serializable]
    public struct GridVisualMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualMaterial> gridVisualMaterialList;

    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow,
    }

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GridSystemVisual! " + transform + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
            ];
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform =
                    Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;

        UpdateGridVisual();
    }

    public void HideAllGridPosition()
    {
        foreach (GridSystemVisualSingle gridSystemVisualSingle in gridSystemVisualSingleArray)
        {
            gridSystemVisualSingle.Hide();
        }
    }


    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= +range; x++)
        {
            for (int z = -range; z <= +range; z++)
            {
                GridPosition gridPositionInRange = gridPosition + new GridPosition(x, z);
                if (!LevelGrid.Instance.IsValidGridPosition(gridPositionInRange))
                {
                    // Invalid grid position
                    continue;
                }

                float testDistance = Mathf.Sqrt(x * x + z * z);

                if (testDistance > range)
                {
                    // Out of range.
                    continue;
                }
                gridPositionList.Add(gridPositionInRange);
            }
        }
        ShowGridPositionList(gridPositionList, gridVisualType);
    }


    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= +range; x++)
        {
            for (int z = -range; z <= +range; z++)
            {
                GridPosition gridPositionInRange = gridPosition + new GridPosition(x, z);
                if (!LevelGrid.Instance.IsValidGridPosition(gridPositionInRange))
                {
                    // Invalid grid position
                    continue;
                }

                gridPositionList.Add(gridPositionInRange);
            }
        }
        ShowGridPositionList(gridPositionList, gridVisualType);
    }


    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;

        switch (selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(
                    selectedUnit.GetGridPosition(),
                    shootAction.GetMaxShootDistance(),
                    GridVisualType.RedSoft
                );
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Yellow;       
                break;       
            case SwordAction swordAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeSquare(
                    selectedUnit.GetGridPosition(),
                    swordAction.GetMaxSwordDistance(),
                    GridVisualType.RedSoft
                );
                break;                
        }
        ShowGridPositionList(
            selectedAction.GetValidActionGridPositionList(), gridVisualType
        );
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualMaterial gridVisualMaterial in gridVisualMaterialList)
        {
            if (gridVisualMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualMaterial.material;
            }
        }

        Debug.LogError("GridVisualType " + gridVisualType + " not found in gridVisualMaterialList! " + transform);
        return null;
    }
}
