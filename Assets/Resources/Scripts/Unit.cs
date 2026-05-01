using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Unit : MonoBehaviour
{
    [SerializeField]
    protected Collider _clickCollider;
    [SerializeField]
    protected Health _health;
    [SerializeField]
    private StatusComponent _statusComponent;
    [SerializeField]
    private UnitInfo _unitInfo;
    private MPDataComponent _mpDataComponent;
    private RuntimeUnitStatus _status;
    private DragSelectable _dragSelectable;
    private Selectable _selectable;

    protected virtual void Awake()
    {
        TryGetComponent(out _mpDataComponent);
        #region Selectable 컴포넌트 할당(없으면 NULL)
        TryGetComponent(out _dragSelectable);
        TryGetComponent(out _selectable);
        #endregion
    }

    protected void SetUp()
    {
        _status = StatusDataMng.Instance.FindStatusData(_unitInfo.ID);
        _health.InitHealth(_status);
    }

    public MPData MPData => _mpDataComponent.GetMPData();
    public DragSelectable DragSelectable => _dragSelectable;
    public Selectable Selectable => _selectable;
    public StatusComponent StatusComponent => _statusComponent;
    public RuntimeUnitStatus Status =>  _status;
    public Collider GetClickCollider() => _clickCollider;
    public long ID => _unitInfo.ID;
    public UnitInfo UnitInfo => _unitInfo;
    public UnitType UnitType => _unitInfo.Type;
    public Health GetHealth() => _health;
}
