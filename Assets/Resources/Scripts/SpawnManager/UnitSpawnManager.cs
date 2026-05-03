using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnManager : Singleton<UnitSpawnManager>
{
    private Dictionary<long, Vector3> _dicSpawnPos = new Dictionary<long, Vector3>();
    [SerializeField]
    private Vector3 _baseSpawnPos = new Vector3(0f, 5f, 0f);
    
    public bool TryAddSpawnHeightOffset(long unitId, float offset)
    {
        if (_dicSpawnPos.ContainsKey(unitId))
            return false;
        Vector3 spawnPos = _baseSpawnPos + new Vector3(0f, offset, 0f);
        return _dicSpawnPos.TryAdd(unitId, spawnPos);
    }
    
    public Unit Spawn(Unit unit)
    {
        MyUnitPrefabDataManager.Instance.TryGetChild(out GameObject instantiateParentObj, unit.UnitType);
        Unit spawnedUnit = Instantiate(unit, instantiateParentObj.transform);
        MyUnitPrefabDataManager.Instance.AddUnitPrefabToList(spawnedUnit.UnitType, spawnedUnit);
        bool bGetPos = _dicSpawnPos.TryGetValue(unit.ID, out Vector3 spawnPos);
        if (bGetPos)
        {
            spawnedUnit.transform.position = spawnPos;
        }
        else
        {
            spawnedUnit.transform.position = _baseSpawnPos;
        }
        return spawnedUnit;
    }
}
