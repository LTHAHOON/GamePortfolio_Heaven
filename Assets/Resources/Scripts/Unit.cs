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
    private UnitInfo _unitInfo;
    [SerializeField]
    //MP 초기 데이터
    private UnitMPDataComponent _unitMpDataComponent;
    private ModeType _curModeType = ModeType.DefenseMode;
    private DragSelectable _dragSelectable;
    private Selectable _selectable;

    protected virtual void Awake()
    {
        #region Selectable 컴포넌트 할당(없으면 NULL)
        TryGetComponent(out _dragSelectable);
        TryGetComponent(out _selectable);
        #endregion
    }
    /// <summary>
    /// SetUp은 UnitInfo를 설정하고 Health를 초기화하는 메서드입니다.(Awake()에 두면 Manager초기화와 겹칩니다.)
    /// </summary>
    protected void SetUpUnit()
    {
        _unitInfo.MPData = MPDataManager.Instance.FindUnitMPData(_unitInfo.ID);
        _unitInfo.Status = StatusManager.Instance.FindStatusData(_unitInfo.ID);
        _health.InitHealth(_unitInfo.Status);
    }

    public void SetModeType(ModeType modeType)
    {
        _curModeType = modeType;
    }
    public ModeType CurrentModeType => _curModeType;
    public ModeType OppositeModeType => _curModeType == ModeType.AttackMode ? ModeType.DefenseMode : ModeType.AttackMode;
    public MPData UnitMPInitData => _unitMpDataComponent.UnitMPData;
    public DragSelectable DragSelectable => _dragSelectable;
    public Selectable Selectable => _selectable;
    public RuntimeUnitStatus Status =>  _unitInfo.Status;
    public Collider GetClickCollider() => _clickCollider;
    public long ID => _unitInfo.ID;
    public UnitInfo UnitInfo => _unitInfo;
    public UnitType UnitType => _unitInfo.Type;
    public Health GetHealth() => _health;
}
