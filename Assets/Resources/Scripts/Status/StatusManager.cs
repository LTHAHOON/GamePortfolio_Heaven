using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum AllStatusNames
{
    ATK, DEF, HIT, Critical, DEX, CON, HealingPower
};


public class StatusManager : Singleton<StatusManager>
{
    [SerializeField]
    private List<StatusDatabase> _statusDataBaseList;
    [SerializeField]
    private List<StatusSlidersController> _statusSlidersControllers;
    private readonly Dictionary<long, RuntimeUnitStatus> _statusDatas = new();
    private readonly Dictionary<int, StatusDatabase> _dicStatusDataBase = new();
    private static readonly int _maxStatus = 20;
    private static readonly int _minStatus = 0;
    private bool _bInitedDataBase = false;
    private void Awake()
    {
        if(!_bInitedDataBase)
            InitStatusDataBases();
    }
    
    private void InitStatusDataBases()
    {
        for (int i = 0; i < _statusDataBaseList.Count; i++)
        {
            _dicStatusDataBase[(int)_statusDataBaseList[i].DBType] = _statusDataBaseList[i];
            _statusDataBaseList[i].InitStatusDatas();
        }
        _bInitedDataBase = true;
    }

    private StatusDatabase GetStatusDataBase(StatusDBType statusDBType)
    {
        if (_dicStatusDataBase.TryGetValue((int)statusDBType, out StatusDatabase statusDataBase))
            return statusDataBase;
        return null;
    }
    
    public bool TryAddStatusData(UnitInfo unitInfo)
    {
        if (!_bInitedDataBase)
            InitStatusDataBases();
        if(unitInfo.Status == null)
            InitializeStatus(unitInfo);
        return _statusDatas.TryAdd(unitInfo.ID,unitInfo.Status);
    }
    
    public StatusSlidersController GetStatusSlidersController(StatusSldiersType sliderType)
    {
        for (int i = 0; i < _statusSlidersControllers.Count; i++)
        {
            if (_statusSlidersControllers[i].SliderType == sliderType)
                return _statusSlidersControllers[i];
        }
        return null;
    }
    
    private void InitializeStatus(UnitInfo unitInfo)
    {
        StatusDatabase statusDB = GetStatusDataBase(unitInfo.StatusDBType);
        BaseUnitStatusData statusData = statusDB.Get(unitInfo);
        unitInfo.Status = new RuntimeUnitStatus(statusData);
    }

    public void AddStatus(AllStatusNames statusName)
    {
        UnitInfo unitInfo = UnitButtonController.GetSelectedUnitPrefab().UnitInfo;
        RuntimeUnitStatus status = FindStatusData(unitInfo.ID);
        StatusUnitMpData statusUnitMpData = MPDataManager.Instance.FindStatusMPData(statusName);
        StatusSlidersController statusSlidersController = GetStatusSlidersController(unitInfo.SliderType);
        if(statusUnitMpData != null)
        {
            bool isUseUPMP = MPDataController.Instance.UseUpMP(statusUnitMpData.MP_ConsValue, count: 1);
            if (isUseUPMP)
            {
                int value = status.GetStatusValue(statusName);
                value = Mathf.Clamp(value + 1, _minStatus, _maxStatus);
                status.SetStatusValue(statusName, value);
                
                statusSlidersController.ReloadStatus();
            }
        }



    }

    public RuntimeUnitStatus FindStatusData(long unitID)
    {
        if(_statusDatas.TryGetValue(unitID, out RuntimeUnitStatus status))
        {
            return status;
        }
        return null;
    }
}
