using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

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
    private Vector3 _gridBasePosition;

    private List<BoxCollider> placeUnitBoxColliderList = new List<BoxCollider>();

    private void Awake()
    {
        _gridBasePosition = _grid.transform.position;
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
        Camera camera = Camera.main;
        Vector3? worldMousePos = InputManager.Instance.GetWolrdMousePosByRaycast(camera, _gridLayerMask, offset:_gridOffset);
        if(worldMousePos.HasValue)
        {
            indicator.SetActive(worldMousePos.HasValue);
            Vector3Int gridPosition = _grid.WorldToCell(worldMousePos.Value);
            Vector3 indicatorPos = _grid.CellToWorld(gridPosition);
            indicatorPos.y += heightOffset;
            indicator.transform.position = indicatorPos;
        }
    }

    private GameObject _curUnitIndicator;
    [HideInInspector]
    public bool _isCellCalculate = false;

    
    public void CalculateIndicator(GameObject unitIndicator)
    {
        if(_isCellCalculate == false)
        {
            _grid.transform.localPosition = _gridBasePosition;
            CalculateCellIndicator(unitIndicator);
        }
    }


    [Header("Grid Settings")]
    [SerializeField] private float cellSizeUnit = 10f; // 한 칸을 추가할 기준 단위 크기
    private void CalculateCellIndicator(GameObject unitIndicator)
    {
        int countX = 1;
        int countZ = 1;
        if (unitIndicator.TryGetComponent(out BoxCollider unitBoxCollider))
        {
            // 2. 월드 공간에서의 실제 크기 계산 (LossyScale 사용이 더 정확함)
            Vector3 worldSize = Vector3.Scale(unitBoxCollider.size, unitIndicator.transform.lossyScale);

             countX += Mathf.CeilToInt(worldSize.x / cellSizeUnit);

             countZ += Mathf.CeilToInt(worldSize.z / cellSizeUnit);
            Vector3 gridPosition = _grid.transform.localPosition;
            if (countX % 2 == 0)
            {
                gridPosition.x = 0;
            }
            if(countZ % 2 == 0)
            {
                gridPosition.z = 0;
            }
            _grid.transform.localPosition = gridPosition;
            cellIndicator.ChangeGridIndicator(countX, countZ);
            _isCellCalculate = true;
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
