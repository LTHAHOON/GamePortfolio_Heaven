using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeUnitStatus
{
    public int ATK;
    public int DEF;
    public int HIT;
    public int Critical;
    public int DEX;
    public int CON;
    public int HealingPower;

    private BaseUnitStatusData _baseStatusData;
    public BaseUnitStatusData GetBaseStatusData()
    {
        return _baseStatusData;
    }
    public int GetStatusValue(AllStatusNames statusName)
    {
        switch (statusName)
        {
            case AllStatusNames.ATK: return ATK;
            case AllStatusNames.DEF: return DEF;
            case AllStatusNames.HIT: return HIT;
            case AllStatusNames.Critical: return Critical;
            case AllStatusNames.DEX: return DEX;
            case AllStatusNames.CON: return CON;
            case AllStatusNames.HealingPower: return HealingPower;
            default: return 0;
        }
    }
    public void SetStatusValue(AllStatusNames statusName, int value)
    {
        switch (statusName)
        {
            case AllStatusNames.ATK: ATK = value; break;
            case AllStatusNames.DEF: DEF = value; break;
            case AllStatusNames.HIT: HIT = value; break;
            case AllStatusNames.Critical: Critical = value; break;
            case AllStatusNames.DEX: DEX = value; break;
            case AllStatusNames.CON: CON = value; break;
            case AllStatusNames.HealingPower: HealingPower = value; break;
        }
    }
    public RuntimeUnitStatus(BaseUnitStatusData data)
    {
        if(data is UnitStatusData unitStatusData)
        {
            _baseStatusData = unitStatusData;
            ATK = unitStatusData.ATK;
            DEF = unitStatusData.DEF;
            HIT = unitStatusData.HIT;
            Critical = unitStatusData.Critical;
            DEX = unitStatusData.DEX;
            CON = unitStatusData.CON;
            HealingPower = unitStatusData.HealingPower;
        }
        else if(data is UnitHealhStatusData healhStatusData)
        {
            _baseStatusData = healhStatusData;
            CON = healhStatusData.CON;
            HealingPower = healhStatusData.HealingPower;
        }
    }
    
}
