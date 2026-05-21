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
    
    public List<T> Spawn<T>(T unit, int unitCount) where T : Unit
    {
        List<T> spawnedUnitList = new();
        for (int i = 0; i < unitCount; i++)
        {
            if(Spawn(unit) is T spawnedUnit)
            {
                spawnedUnitList ??= new List<T>(unitCount);
                spawnedUnitList.Add(spawnedUnit);
            }
        }
        return spawnedUnitList;
    }
    public T Spawn<T>(T unit, Vector3? pos = null, Vector3? direction = null) where T : Unit
    {
        UnitStorageManager.Instance.TryGetUnitParent(out GameObject instantiateParentObj, Faction.Ally,unit.UnitType);
        if(!pos.HasValue)
        {
            pos = Vector3.zero;
        }    
        
        T spawnedUnit = Instantiate(unit, pos.Value, Quaternion.identity, instantiateParentObj.transform);
        if (direction.HasValue)
        {
            spawnedUnit.transform.rotation = Quaternion.LookRotation(direction.Value);
        }
        UnitStorageManager.Instance.AddUnitToStorageList(Faction.Ally, spawnedUnit.UnitType, spawnedUnit);
        bool bGetPos = _dicSpawnPos.TryGetValue(unit.ID, out Vector3 spawnPos);
        if (bGetPos)
        {
            spawnedUnit.transform.position += spawnPos;
        }
        else
        {
            spawnedUnit.transform.position += _baseSpawnPos;
        }
        return spawnedUnit;
    }
}
