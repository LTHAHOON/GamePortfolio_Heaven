using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusComponent : MonoBehaviour
{
    [SerializeField]
    private StatusDatabase _statusDataBase;
    [SerializeField]
    private bool _useIDForInteract = false;
    [SerializeField]
    private UnitData _unitData;
    private RuntimeUnitStatus _status;

    private void Awake()
    {
        if (_unitData.Type == UnitType.Nexus || _unitData.Type == UnitType.Home)
        {
            InitializeStatus<HealhStatusData>();
        }
        else
        {
            InitializeStatus<UnitStatusData>();
        }
    }

    private void InitializeStatus<T>() where T : BaseUnitStatusData
    {
        T statusData = _statusDataBase.Get<T>(_unitData,_useIDForInteract);
        _status = new RuntimeUnitStatus(statusData);
    }

    public UnitData GetUnitData()
    {
        return _unitData;
    }
    public RuntimeUnitStatus GetStatus()
    {
        return _status;
    }
    public void SetStatus(RuntimeUnitStatus status)
    {
        _status = status;
    }
}
