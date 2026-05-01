using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnManager : Singleton<UnitSpawnManager>
{
    public Unit Spawn(Unit unit)
    {
        MyUnitPrefabDataControl.Instance.TryGetChild(out GameObject instantiateParentObj, unit.UnitType);
        return Instantiate(unit, instantiateParentObj.transform);
    }
    
    public Unit Spawn(UnitInfo unitInfo)
    {
        MyUnitPrefabDataControl.Instance.TryGetChild(out GameObject instantiateParentObj, unitInfo.Type);
        Unit unit = UnitPrefabDataManager.Instance.GetUnitPrefab(unitInfo);
        return Instantiate(unit, instantiateParentObj.transform);
    }
}
