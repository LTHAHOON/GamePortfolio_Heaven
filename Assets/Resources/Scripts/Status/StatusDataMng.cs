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


public class StatusDataMng : Singleton<StatusDataMng>
{

    private readonly Dictionary<long, RuntimeUnitStatus> _statusDatas = new();

    public bool AddStatusData(long unitID, StatusComponent statusComponent)
    {
        if (!_statusDatas.ContainsKey(unitID))
        {
            _statusDatas.Add(unitID, statusComponent.GetStatus());
            return true;
        }
        return false;
    }


    private static readonly int _maxStatus = 20;
    private static readonly int _minStatus = 0;
    public void AddStatus(RuntimeUnitStatus status,AllStatusNames statusName)
    {
        bool hasConsumeMpValue = MPDataController.Instance.TryGetConsumeMPValue(statusName, out ConsumeMPValue consumeMPValue);
        if(hasConsumeMpValue)
        {
            bool isUseUPMP = MPDataController.Instance.UseUpMP(consumptionMPValue: consumeMPValue.consumeMPValue, count: 1);
            if (isUseUPMP)
            {
                int value = status.GetStatusValue(statusName);
                value = Mathf.Clamp(value + 1, _minStatus, _maxStatus);
                status.SetStatusValue(statusName, value);

                StatusSliderController.ReloadStatus();
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
