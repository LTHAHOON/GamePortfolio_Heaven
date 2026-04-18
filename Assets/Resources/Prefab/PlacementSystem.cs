using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GridIndicatorController cellIndicator;
    [SerializeField]
    private InputManager _inputManager;
    [SerializeField]
    private LayerMask _gridLayerMask;
    [SerializeField]
    private Vector3 _gridOffset;

    [SerializeField]
    private Grid _grid;

    private List<BoxCollider> placeUnitBoxColliderList = new List<BoxCollider>();

    private void Awake()
    {
        _cellInitalScale = cellIndicator.transform.parent.localScale;
    }

    private void Update()
    {
        FollowIndicatorByMouse(cellIndicator.gameObject, 0);
    }

    public bool ComparePosInBoxCollider(Vector3 pos)
    {
        if (placeUnitBoxColliderList.Count <= 0) return false;

        for (int i = 0; i < placeUnitBoxColliderList.Count; i++)
        {
            Bounds bounds = placeUnitBoxColliderList[i].bounds;
            if(bounds.Contains(pos))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsEqualToPos(Vector3 pos)
    {
        if (placeUnitBoxColliderList.Count <= 0) return false;

        Vector3Int posInt = new Vector3Int((int)pos.x, 1, (int)pos.z);
        for (int i = 0; i < placeUnitBoxColliderList.Count; i++)
        {
            Vector3Int newPosInt = new Vector3Int((int)placeUnitBoxColliderList[i].transform.position.x, 1,
                                                    (int)placeUnitBoxColliderList[i].transform.position.z);
            if (posInt == newPosInt)
            {
                return true;
            }
        }
        return false;
    }


    public void FollowIndicatorByMouse(GameObject indicator, float heightOffset)
    {
        Camera camera = UIManager.Instance.CurrentUICamera;
        Vector3? worldMousePos = InputManager.Instance.GetWolrdMousePosByRaycast(camera, _gridLayerMask, offset:_gridOffset);
        indicator.SetActive(worldMousePos.HasValue);
        Vector3Int gridPosition = _grid.WorldToCell(worldMousePos.Value);
        Vector3 indicatorPos  = _grid.CellToWorld(gridPosition);
        indicatorPos.y += heightOffset;
        indicator.transform.position = indicatorPos;
    }

    private GameObject _curUnitIndicator;
    [HideInInspector]
    public bool _isCellCalculate = false;

    
    public void CalculateIndicator(GameObject unitIndicator)
    {
        if(_isCellCalculate == false)
        {
            CalculateCellIndicator(unitIndicator);
        }
        CalculateUnitIndicator(unitIndicator);
    }

    private Vector3 _cellInitalScale;
    private void InitalCellIndicator()
    {
        countX = 1;
        countZ = 1;
        compareValueX = _initalCompareValue;
        compareValueZ = _initalCompareValue;
        cellIndicator.transform.parent.localScale = _cellInitalScale;
    }

    private float GetVectorPosAverage(Vector3 vector)
    {
        if (vector == Vector3.zero) return 0;

        int posCount = 3;
        float sum = (vector.x + vector.y + vector.z);

        float result = sum / posCount;
        return result;
    }


    private int countX = 1;
    private int countZ = 1;
    private float compareValueX = 8f;
    private float compareValueZ = 8f;
    private float boxColliderSizeOffset;
    private void CalculateCellIndicator(GameObject unitIndicator)
    {
        InitalCellIndicator();

        if (unitIndicator.TryGetComponent(out BoxCollider unitBoxCollider))
        {
            boxColliderSizeOffset = 0.6f / GetVectorPosAverage(unitIndicator.transform.localScale);

            //TODO: Grid Indicator 세로 칸 계산하기
            for (int i = 0; i < 15; i++)
            {
                if ((unitBoxCollider.size.x  / boxColliderSizeOffset) <= compareValueX )
                {
                    float scaleX = cellIndicator.transform.parent.localScale.x + (0.45f* (countX - 1));
                    cellIndicator.transform.parent.localScale = new Vector3(scaleX, 1, cellIndicator.transform.parent.localScale.z);
                    if (countX % 2 == 0)
                    {
                        _grid.transform.localPosition = new Vector3(2.5f, _grid.transform.localPosition.y, _grid.transform.localPosition.z);
                    }
                    else
                    {
                        _grid.transform.localPosition = new Vector3(0f, _grid.transform.localPosition.y, _grid.transform.localPosition.z);
                    }
                    break;
                }
                else
                {
                    compareValueX += 8.7f;
                    countX++;
                }

            }

            //TODO: Grid Indicator 가로 칸 계산하기
            for (int i = 0; i < 15; i++)    
            {
                if ((unitBoxCollider.size.z / boxColliderSizeOffset) <= compareValueZ)
                {
                    float scaleZ = cellIndicator.transform.parent.localScale.z + (0.45f * (countZ - 1));
                    cellIndicator.transform.parent.localScale = new Vector3(cellIndicator.transform.parent.localScale.x, 1, scaleZ);
                    if (countZ % 2 == 0)
                    {
                        _grid.transform.localPosition = new Vector3(_grid.transform.localPosition.x, _grid.transform.localPosition.y, 2.5f);
                    }
                    else
                    {
                        _grid.gameObject.transform.localPosition = new Vector3(_grid.transform.localPosition.x, _grid.transform.localPosition.y, 0f);
                    }
                    break;
                }
                else
                {
                    compareValueZ += 8.7f;
                    countZ++;
                }
            }

            //TODO: Grid Indicator 가로 세로 설정하기
            cellIndicator.ChangeGridIndicator(countZ, countX);

            _isCellCalculate = true;
        }
    }

    private const float _initalCompareValue = 8f;
    private void CalculateUnitIndicator(GameObject unitIndicator)
    {
        if (unitIndicator.TryGetComponent(out BoxCollider unitBoxCollider))
        {
            unitBoxCollider.size = new Vector3(_initalCompareValue * boxColliderSizeOffset * countX, unitBoxCollider.size.y, _initalCompareValue * boxColliderSizeOffset * countZ);
        }
    }

    
    public void SetUnitIndicator(GameObject unit)
    {
        _curUnitIndicator = unit;
    }

    public GameObject GetUnitIndicator()
    {
        return _curUnitIndicator;
    }

    public void AddPlaceUnitBoxCollider(BoxCollider unitCollider)
    {
        placeUnitBoxColliderList.Add(unitCollider);
    }
}
