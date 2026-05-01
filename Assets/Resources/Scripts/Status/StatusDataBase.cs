using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/StatusDB")]
public class StatusDatabase : ScriptableObject
{
    [SerializeField]
    private List<BaseUnitStatusData> dataList = new();


    public T Get<T>(UnitInfo unitInfo, bool useIDForInteract) where T : BaseUnitStatusData
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            if(useIDForInteract)
            {
                if (dataList[i].UnitData.ID == unitInfo.ID && dataList[i].UnitData.Type == unitInfo.Type && dataList[i].UnitData.Property == unitInfo.Property)
                {
                    return dataList[i] as T;
                }
            }
            else
            {
                if (dataList[i].UnitData.Type == unitInfo.Type && dataList[i].UnitData.Property == unitInfo.Property)
                {
                    return dataList[i] as T;
                }
            }
            
        }
        Debug.LogError("StatusDatabase: No data found for " + unitInfo.Type + " " + unitInfo.Property);
        return null;
    }
}