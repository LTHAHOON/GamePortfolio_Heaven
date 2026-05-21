using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private float _dragStartDistance = 10f;
    private Vector3 _dragStartPosition;
    private bool _isDragging = false;
    private Bounds _dragBounds;

    private void OnGUI()
    {
        if (UIManager.Instance.IsSubCameraActive && !ModeButtonManager.Instance.IsUpdateMode)
        {
            if (_isDragging && CanDragSelection(_dragStartPosition, Input.mousePosition))
            {
                Vector3 dragEndPosition = Input.mousePosition;
                var rect = Utils.GetScreenRect(_dragStartPosition, dragEndPosition);
                Utils.DrawScreenRect(rect, UIManager.Instance.GetColorOfDrawScreen());
                Utils.DrawScreenRectBorder(rect, 2, UIManager.Instance.GetColorOfDrawScreenBorder());
            }
        }
        else if (_isDragging)
        {
            _isDragging = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_dragBounds.center, _dragBounds.size);
    }

    public void OnDebugSelection(int count)
    {
        Debug.Log($"Selected {count}개 Unit");
    }

    public Vector3 GetNormalByRaycast(Vector3 raycastStart, Vector3 raycastDir)
    {
        if (Physics.Raycast(raycastStart, raycastDir, out RaycastHit hit))
        {
            // 레이캐스트로 부터 얻은 경사면의 법선 벡터
            Vector3 slopeNormal = hit.normal;

            return slopeNormal;
        }

        return Vector3.up;
    }


    #region 원클릭으로 Selection 구하는 함수

    public ISelection TrySelection(out bool bOnClick, Camera camera,
        LayerMask targetLayerMask, bool bFindUsingTargetParent = false)
    {
        if (!CanDragSelection(_dragStartPosition, Input.mousePosition))
        {
            if (TrySelectBySphereCast(out bOnClick ,0 ,1.2f , camera,  targetLayerMask, out GameObject target))
            {
                _isDragging = false;
                _dragStartPosition = Vector3.zero;
                if (bFindUsingTargetParent)
                {
                    target = target.transform.parent.gameObject;
                }

                if (target.TryGetComponent(out Selectable selectable))
                {
                    ISelection selection = SelectionManager.Instance.GetSelection(selectable);
                    if (selection != null)
                    {
                        selectable.OnSelected();
                        selection.AddToSelectedList(selectable.Owner);
                        //OnDebugSelection(1);
                        return selection;
                    }
                }
            }
            if (bOnClick)
            {
                _isDragging = false;
                _dragStartPosition = Vector3.zero;
            }
            // OnDebugSelection(0);
            return null;
        }

        bOnClick = false;
        return null;
    }

    #endregion

    #region 드래그로 Selection 구하는 함수

    //유닛 캐싱된 데이터로 Selection을 구하는 함수 
    public List<ISelection> TryDragSelectionByUnitType(out bool bOnDrag, Camera camera, UnitType unitType)
    {
        if (UnitStorageManager.Instance.TryGetUnitList(out List<Unit> unitList, Faction.Ally, unitType))
        {
            List<ISelection> selection = TryDragSelection(out bOnDrag, unitList, camera);
            return selection;
        }

        bOnDrag = false;
        return null;
    }

    private bool CanDragSelection(Vector2 dragStartPosition, Vector2 dragEndPosition)
    {
        Vector2 dragDirection = dragEndPosition - dragStartPosition;
        return dragDirection.magnitude >= _dragStartDistance * _dragStartDistance;
    }

    private readonly List<ISelection> _selectionList = new();

    private List<ISelection> TryDragSelection<T>(out bool bOnDrag, List<T> selectableList, Camera camera)
        where T : MonoBehaviour
    {
        if (Input.GetMouseButtonDown(0) && _isDragging == false)
        {
            _selectionList.Clear();
            _dragStartPosition = Input.mousePosition;
            _isDragging = true;
        }

        if (CanDragSelection(_dragStartPosition, Input.mousePosition))
        {
            bOnDrag = true;
            if (Input.GetMouseButtonUp(0) && _isDragging)
            {
                Vector3 dragEndPosition = Input.mousePosition;
                Rect selectionRect = GetDragSelectionRect(_dragStartPosition, dragEndPosition);
                _dragStartPosition = Vector3.zero;
                _isDragging = false;
                int unitCount = 0;
                ISelection dragSelection;
                DragSelectable dragSelectable;
                for (int i = 0; i < selectableList.Count; i++)
                {
                    Vector3 screenPos = camera.WorldToScreenPoint(selectableList[i].transform.position);
                    if (selectionRect.Contains(screenPos))
                    {
                        if (selectableList[i] is Unit unit)
                        {
                            dragSelectable = unit.DragSelectable;
                        }
                        else if (selectableList[i].TryGetComponent(out dragSelectable))
                        {
                        }

                        dragSelection = SelectionManager.Instance.GetSelection(dragSelectable);
                        if (dragSelection != null)
                        {
                            ++unitCount;
                            dragSelectable.OnDragSelected();
                            dragSelection.AddToSelectedList(dragSelectable.Owner);
                            _selectionList.Add(dragSelection);
                        }
                    }
                }

                OnDebugSelection(unitCount);
                return _selectionList;
            }
        }

        bOnDrag = false;
        return null;
    }

    #endregion

    private Rect GetDragSelectionRect(Vector3 dragStartPosition, Vector3 dragEndPosition)
    {
        float minX = Mathf.Min(_dragStartPosition.x, dragEndPosition.x);
        float minY = Mathf.Min(_dragStartPosition.y, dragEndPosition.y);
        float width = Mathf.Abs(_dragStartPosition.x - dragEndPosition.x);
        float height = Mathf.Abs(_dragStartPosition.y - dragEndPosition.y);

        Rect selectionRect = new Rect(minX, minY, width, height);
        return selectionRect;
    }


    public Vector3? GetWolrdMousePosByRaycast(Camera camera, LayerMask targetLayerMask, float maxDistance = 1000,
        Vector3? offset = null, int blockLayer = -1)
    {
        if (!offset.HasValue)
        {
            offset = Vector3.zero;
        }

        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, targetLayerMask))
        {
            if (blockLayer > 0)
            {
                if (hit.collider.gameObject.layer == blockLayer)
                {
                    return null;
                }
            }

            return hit.point + offset.Value;
        }

        return null;
    }

    private readonly Collider[] _hitResults = new Collider[500];

    public Collider[] GetByOverlapCast(out int count, Vector3 position, float radius,
        LayerMask targetLayerMask)
    {
        count = Physics.OverlapSphereNonAlloc(position, radius, _hitResults, targetLayerMask);
        if (count > 0)
        {
            return _hitResults;
        }

        return null;
    }

    public bool TryGetByRaycast(out RaycastHit targetHit, Ray ray, float maxDistance, LayerMask targetLayerMask,
        int exceptedLayer = -1)
    {
        return TryGetByRaycast(out targetHit, ray.origin, ray.direction, maxDistance, targetLayerMask, exceptedLayer);
    }

    public bool TryGetByRaycast(out RaycastHit targetHit, Vector3 raycastStart, Vector3 raycastDirection, float maxDistance,
        LayerMask targetLayerMask, int exceptedLayer = -1)
    {
        if (exceptedLayer > 0)
        {
            targetLayerMask = GameLayerManager.Instance.GetExceptedLayerMask(targetLayerMask, exceptedLayer);
        }
        return Physics.Raycast(raycastStart, raycastDirection, out targetHit, maxDistance, targetLayerMask);
    }

    public RaycastHit GetBySphereCastUsingMouse(Camera camera, float radius, LayerMask targetLayerMask)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        return GetBySphereCast(ray, radius, camera.farClipPlane, targetLayerMask);
    }

    public RaycastHit GetBySphereCast(Ray ray, float radius, float maxDistance, LayerMask targetLayerMask)
    {
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
        if (Physics.SphereCast(ray, 1.2f, out RaycastHit hit, maxDistance, targetLayerMask))
        {
            return hit;
        }

        return default;
    }
    public RaycastHit GetBySphereCast(Vector3 startPosition, Vector3 direction, float radius, float maxDistance, LayerMask targetLayerMask)
    {
        Debug.DrawRay(startPosition, direction * maxDistance, Color.red);
        if (Physics.SphereCast(startPosition, radius, direction, out RaycastHit hit, maxDistance, targetLayerMask))
        {
            return hit;
        }

        return default;
    }

    public GameObject SelectBySphereCast(KeyCode keyCode, Camera camera, float radius, LayerMask targetLayerMask)
    {
        if (Input.GetKeyDown(keyCode))
        {
            RaycastHit hit = GetBySphereCastUsingMouse(camera, radius, targetLayerMask);
            return hit.collider.gameObject;
        }

        return null;
    }

    public bool TrySelectUnitBySphereCast(KeyCode keyCode, Camera camera, float radius, LayerMask targetLayerMask, UnitType unitType,
        out GameObject target, bool bFindUsingTargetParent = false)
    {
        target = SelectBySphereCast(keyCode, camera, radius, targetLayerMask);
        if (target)
        {
            if (UnitStorageManager.Instance.TryGetUnitList(out List<Unit> unitList, Faction.Ally, unitType))
            {
                if (bFindUsingTargetParent)
                {
                    target = target.transform.parent.gameObject;
                }

                for (int i = 0; i < unitList.Count; i++)
                {
                    if (unitList[i].gameObject == target)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool TrySelectBySphereCast(KeyCode keyCode, Camera camera, float radius, LayerMask targetLayerMask, out GameObject target)
    {
        target = SelectBySphereCast(keyCode, camera, radius, targetLayerMask);
        if (target != null)
        {
            return true;
        }

        return false;
    }

    public GameObject SelectBySphereCast(out bool bOnClick, int mouseButton, float radius, Camera camera, LayerMask targetLayerMask)
    {
        if (Input.GetMouseButtonUp(mouseButton))
        {
            bOnClick = true;
            RaycastHit hit = GetBySphereCastUsingMouse(camera, radius,targetLayerMask);
            if (hit.collider)
            {
                return hit.collider.gameObject;
            }
            return null;
        }

        bOnClick = false;
        return null;
    }

    public bool TrySelectBySphereCast(out bool bOnClick, int mouseButton, float radius, Camera camera, LayerMask targetLayerMask,
        out GameObject target)
    {
        target = SelectBySphereCast(out bOnClick, mouseButton, radius, camera, targetLayerMask);
        if (target != null)
        {
            return true;
        }

        return false;
    }
}