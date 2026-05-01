using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPrefabDataManager : Singleton<UnitPrefabDataManager>
{
    [SerializeField]
    private UnitPrefabDB[] _unitPrefabDatabase;
    
    public UnitPrefabDB GetUnitPrefabDatabase(UnitType unitType)
    {
        for (int i = 0; i < _unitPrefabDatabase.Length; i++)
        {
            if(_unitPrefabDatabase[i].UnitDataBaseType == unitType)
            {
                return _unitPrefabDatabase[i];
            }
        }
        return null;
    }

    public Unit GetUnitPrefab(UnitInfo unitInfo)
    {
        UnitPrefabDB unitPrefabDB = GetUnitPrefabDatabase(unitInfo.Type);
        if (!unitPrefabDB)
            return null;
        
        return unitPrefabDB.GetUnitPrefab(unitInfo.ID);
    }
}
