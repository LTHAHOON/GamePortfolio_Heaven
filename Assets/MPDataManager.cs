using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPDataManager : Singleton<MPDataManager>
{
    private Dictionary<long, MPData> _unitMPData = new();
    [SerializeField]
    private List<StatusUnitMpData> _statusMPData;
    

    public bool TryAddMPData(long unitID, MPData unitMpDataComponent)
    {
        return _unitMPData.TryAdd(unitID, unitMpDataComponent);
    }

    public MPData FindUnitMPData(long unitID)
    {
        bool bGetMPData = _unitMPData.TryGetValue(unitID, out MPData unitMPData);
        if (bGetMPData)
        {
            return unitMPData;
        }
        return null;
    }
    //StatusName을 통해 StatusMPData를 가져오는 메서드
    public StatusUnitMpData FindStatusMPData(AllStatusNames statusName)
    {
        for (int i = 0; i < _statusMPData.Count; i++)
        {
            if (_statusMPData[i].statusName == statusName)
            {
                return _statusMPData[i];
            }
        }
        return null;
    }

    public List<StatusUnitMpData> GetStatusMPDatas() => _statusMPData;
}
