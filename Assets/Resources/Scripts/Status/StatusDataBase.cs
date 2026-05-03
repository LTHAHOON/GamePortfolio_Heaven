using System.Collections.Generic;
using UnityEngine;

public enum StatusDBType
{
    IDType,
    PropertyType,
}

[CreateAssetMenu(menuName = "Database/StatusDB")]
public class StatusDatabase : ScriptableObject
{
    [SerializeField]
    private List<BaseUnitStatusData> _statusDataList = new();
    private readonly Dictionary<int, BaseUnitStatusData> _dicStatusData = new();
    [SerializeField]
    private StatusDBType _dbType;
    
    public void InitStatusDatas()
    {
        _dicStatusData.Clear();
        for (int i = 0; i < _statusDataList.Count; i++)
        {
            _dicStatusData[_statusDataList[i].GetHashCode()] = _statusDataList[i];
        }
    }
    
    public BaseUnitStatusData Get(UnitInfo unitInfo)
    {
        for (int i = 0; i < _statusDataList.Count; i++)
        {
            int index = _statusDataList[i].GetHashCode();
            if(_dbType == StatusDBType.IDType)
            {
                if (_dicStatusData[index].UnitData.ID == unitInfo.ID && _dicStatusData[index].UnitData.Type == unitInfo.Type && _dicStatusData[index].UnitData.Property == unitInfo.Property)
                {
                    return _dicStatusData[index];
                }
            }
            else if(_dbType == StatusDBType.PropertyType)
            {
                if (_dicStatusData[index].UnitData.Type == unitInfo.Type && _dicStatusData[index].UnitData.Property == unitInfo.Property)
                {
                    return _dicStatusData[index];
                }
            }
            
        }
        Debug.LogError("StatusDatabase: No data found for " + unitInfo.Type + " " + unitInfo.Property);
        return null;
    }
    
    public StatusDBType DBType => _dbType;
}