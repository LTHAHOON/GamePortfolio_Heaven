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
    private RuntimeUnitStatus _status;

    public void InitializeStatus(UnitInfo unitInfo)
    {
        if (unitInfo.Type is UnitType.Nexus or UnitType.Home)
        {
            InitializeStatus<UnitHealhStatusData>(unitInfo);
        }
        else
        {
            InitializeStatus<UnitStatusData>(unitInfo);
        }
    }
    
    private void InitializeStatus<T>(UnitInfo unitInfo) where T : BaseUnitStatusData
    {
        var statusData = _statusDataBase.Get<T>(unitInfo, _useIDForInteract);
        _status = new RuntimeUnitStatus(statusData);
    }
    
    public RuntimeUnitStatus GetStatus()
    {
        return _status;
    }
    
}
