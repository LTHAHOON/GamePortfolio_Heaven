using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [SerializeField]
    protected Collider _clickCollider;
    [SerializeField]
    protected Health _health;
    [HideInInspector]
    public DragSelectable _dragSelectable;
    [HideInInspector]
    public Selectable _selectable;
    public UnitType _unitType;
    protected virtual void Awake()
    {
        _unitType = GetComponent<StatusComponent>().GetUnitData().Type;
        #region Selectable 컴포넌트 할당(없으면 NULL)
        TryGetComponent(out _dragSelectable);
        TryGetComponent(out _selectable);
        #endregion
    }

    public Collider GetClickCollider() => _clickCollider;
    public UnitType UnitType => _unitType;
    public Health GetHealth() => _health;
}
