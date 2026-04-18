using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/StatusDB")]
public class StatusDatabase : ScriptableObject
{
    [SerializeField]
    private List<BaseUnitStatusData> dataList = new();


    public T Get<T>(UnitData id, bool useIDForInteract) where T : BaseUnitStatusData
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            if(useIDForInteract)
            {
                if (dataList[i]._unitData.ID == id.ID && dataList[i]._unitData.Type == id.Type && dataList[i]._unitData.Property == id.Property)
                {
                    return dataList[i] as T;
                }
            }
            else
            {
                if (dataList[i]._unitData.Type == id.Type && dataList[i]._unitData.Property == id.Property)
                {
                    return dataList[i] as T;
                }
            }
            
        }
        Debug.LogError("StatusDatabase: No data found for " + id.Type + " " + id.Property);
        return null;
    }
}