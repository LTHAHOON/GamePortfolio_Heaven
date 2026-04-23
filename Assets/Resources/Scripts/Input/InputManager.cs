using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    public int TrySelectionByUnitType(out bool bOnClick, Camera camera, 
        LayerMask targetLayerMask, UnitType unitType, bool bFindUsingTargetParent = false, Action<Unit> OnSelected = null)
    {
        if (TrySelectBySphereCast(0, camera, targetLayerMask, out GameObject target))
        {
            bOnClick = true;
            _subDragStartPosition = Vector3.zero;
            _dragStartPosition = Vector3.zero;
            _isDragging = false;
            if(bFindUsingTargetParent)
            {
                target = target.transform.parent.gameObject;
            }
            if (target.TryGetComponent(out Unit unit) && target.TryGetComponent(out Selectable selectable))
            {
                bool doGetCharacter = MyUnitPrefabDataControl.Instance.ContainsUnitPrefab(unit, unitType);
                if (doGetCharacter)
                {
                    selectable.OnSelected();
                    OnSelected?.Invoke(unit);
                    OnDebugSelection(1);
                    return 1;
                }
            }
            OnDebugSelection(0);
        }
        bOnClick = false;
        return 0;
    }

    public int TryDragSelectionByUnitType(out bool bOnDrag, Camera camera, UnitType unitType, Action<Unit> OnSelected = null)
    {
        if (Input.GetMouseButtonDown(0) && _isDragging == false)
        {
            bOnDrag = true;
            _dragStartPosition = Input.mousePosition;
            _subDragStartPosition = Input.mousePosition;
            _isDragging = true;
            return 0;
        }
        else if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            bOnDrag = true;
            Vector3 dragEndPosition = Input.mousePosition;
            Rect selectionRect = GetDragSelectionRect(_subDragStartPosition, dragEndPosition);
            _subDragStartPosition = Vector3.zero;
            _isDragging = false; 
            if (MyUnitPrefabDataControl.Instance.TryGetUnitList(out List<Unit> unitList, unitType))
            {
                int unitCount = 0;
                for (int i = 0; i < unitList.Count; i++)
                {
                    Vector3 screenPos = camera.WorldToScreenPoint(unitList[i].transform.position);
                    if (selectionRect.Contains(screenPos))
                    {
                        bool hasDragSelectable = unitList[i].TryGetComponent(out DragSelectable dragSelectable);
                        if (hasDragSelectable)
                        {
                            ++unitCount;
                            OnSelected?.Invoke(unitList[i]);
                            dragSelectable.OnDragSelected();
                        }
                    }
                }
                OnDebugSelection(unitCount);
                return unitCount;
            }
        }
        bOnDrag =  false;
        return 0;
    }

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

    public GameObject SelectBySphereCast(int mouseButton, Camera camera, LayerMask targetLayerMask)
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            return SelectBySphereCast(camera, targetLayerMask);
        }
        return null;
    }
    public bool TrySelectBySphereCast(int mouseButton, Camera camera, LayerMask targetLayerMask, out GameObject target)
    {
        target = SelectBySphereCast(mouseButton, camera, targetLayerMask);
        if (target != null)
        {
            return true;
        }
        return false;
    }
}
