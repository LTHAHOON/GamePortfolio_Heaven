using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InputManager : Singleton<InputManager>
{
    private Vector3 _dragStartPosition;
    private Vector3 _subDragStartPosition;
    private bool _isDragging = false;
    private Bounds dragBounds;
    private void OnGUI()
    {
        if (_isDragging && UIManager.Instance.IsSubCameraActive)
        {
            Vector3 dragEndPosition = Input.mousePosition;
            var rect = Utils.GetScreenRect(_dragStartPosition, dragEndPosition);
            Utils.DrawScreenRect(rect, UIManager.Instance.GetColorOfDrawScreen());
            Utils.DrawScreenRectBorder(rect, 2, UIManager.Instance.GetColorOfDrawScreenBorder());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(dragBounds.center, dragBounds.size);

    }
    public void OnDebugSelection(int count)
    {
        Debug.Log($"Selected {count}개 Unit");
    }

    #region 원클릭으로 Selection 구하는 함수
    public ISelection TrySelection(out bool bOnClick, Camera camera,
    LayerMask targetLayerMask, bool bFindUsingTargetParent = false)
    {
        if (TrySelectBySphereCast(out bOnClick, 0, camera, targetLayerMask, out GameObject target))
        {
            _subDragStartPosition = Vector3.zero;
            _dragStartPosition = Vector3.zero;
            _isDragging = false;
            if (bFindUsingTargetParent)
            {
                target = target.transform.parent.gameObject;
            }
            if (target.TryGetComponent(out Selectable selectable))
            {
                ISelection selection = SelectionManager.Instance.GetSelection(selectable);
                if(selection != null)
                {
                    selectable.OnSelected();
                    selection.AddToSelectedList(selectable.Owner);
                    OnDebugSelection(1);
                    return selection;
                }
            }
            OnDebugSelection(0);
            return null;
        }
        return null;
    }
    #endregion

    #region 드래그로 Selection 구하는 함수
    //유닛 캐싱된 데이터로 Selection을 구하는 함수 
    public List<ISelection> TryDragSelectionByUnitType(out bool bOnDrag, Camera camera, UnitType unitType)
    {
        if (MyUnitPrefabDataControl.Instance.TryGetUnitList(out List<Unit> unitList, unitType))
        {
            List<ISelection> selection = TryDragSelection(out bOnDrag, unitList, camera);
            return selection;
        }
        bOnDrag = false;
        return null;
    }
    private readonly List<ISelection> _selectionList = new();
    public List<ISelection> TryDragSelection<T>(out bool bOnDrag, List<T> selectableList, Camera camera) where T :MonoBehaviour
    {
        if (Input.GetMouseButtonDown(0) && _isDragging == false)
        {
            bOnDrag = true;
            _selectionList.Clear();
            _dragStartPosition = Input.mousePosition;
            _subDragStartPosition = Input.mousePosition;
            _isDragging = true;
            return null;
        }
        else if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            bOnDrag = true;
            Vector3 dragEndPosition = Input.mousePosition;
            Rect selectionRect = GetDragSelectionRect(_subDragStartPosition, dragEndPosition);
            _subDragStartPosition = Vector3.zero;
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
                        dragSelectable = unit._dragSelectable;
                    }
                    else if (selectableList[i].TryGetComponent(out dragSelectable)) { }

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
        bOnDrag = false;
        return null;
    }
    #endregion

    private Rect GetDragSelectionRect(Vector3 dragStartPosition, Vector3 dragEndPosition)
    {
        float minX = Mathf.Min(_subDragStartPosition.x, dragEndPosition.x);
        float minY = Mathf.Min(_subDragStartPosition.y, dragEndPosition.y);
        float width = Mathf.Abs(_subDragStartPosition.x - dragEndPosition.x);
        float height = Mathf.Abs(_subDragStartPosition.y - dragEndPosition.y);

        Rect selectionRect = new Rect(minX, minY, width, height);
        return selectionRect;
    }


    public Vector3? GetWolrdMousePosByRaycast(Camera camera, LayerMask targetLayerMask, float maxDistance = 1000, Vector3? offset = null)
    {
        if(!offset.HasValue)
        {
            offset = Vector3.zero;
        }
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, maxDistance, targetLayerMask))
        {
            return hit.point + offset.Value;
        }
        return null;
    }

    public GameObject SelectBySphereCast(Camera camera, LayerMask targetLayerMask)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * camera.farClipPlane, Color.red);
        if (Physics.SphereCast(ray, 1.2f, out RaycastHit hit, camera.farClipPlane, targetLayerMask))
        {
            return hit.collider.gameObject;
        }
        return null;
    }
    public GameObject SelectBySphereCast(KeyCode keyCode, Camera camera, LayerMask targetLayerMask)
    {
        if (Input.GetKeyDown(keyCode))
        {
            return SelectBySphereCast(camera, targetLayerMask);
        }
        return null;
    }

    public bool TrySelectUnitBySphereCast(KeyCode keyCode, Camera camera, LayerMask targetLayerMask, UnitType unitType, out GameObject target, bool bFindUsingTargetParent = false)
    {
        target = SelectBySphereCast(keyCode, camera, targetLayerMask);
        if(target)
        {
            if (MyUnitPrefabDataControl.Instance.TryGetUnitList(out List<Unit> unitList, unitType))
            {
                if(bFindUsingTargetParent)
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

    public bool TrySelectBySphereCast(KeyCode keyCode, Camera camera, LayerMask targetLayerMask, out GameObject target)
    {
        target = SelectBySphereCast(keyCode, camera, targetLayerMask);
        if (target != null)
        {
            return true;
        }
        return false;
    }

    public GameObject SelectBySphereCast(out bool bOnClick, int mouseButton, Camera camera, LayerMask targetLayerMask)
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            bOnClick = true;
            return SelectBySphereCast(camera, targetLayerMask);
        }
        bOnClick = false;
        return null;
    }
    public bool TrySelectBySphereCast(out bool bOnClick, int mouseButton, Camera camera, LayerMask targetLayerMask, out GameObject target)
    {
        target = SelectBySphereCast(out bOnClick, mouseButton, camera, targetLayerMask);
        if (target != null)
        {
            return true;
        }
        return false;
    }
}
