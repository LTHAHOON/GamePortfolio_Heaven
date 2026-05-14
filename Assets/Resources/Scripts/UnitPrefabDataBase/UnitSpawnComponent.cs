using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitSpawnComponent : MonoBehaviour
{
    [SerializeField]
    private float _spawnHeightOffset = 0f;
    private Unit _unitSpawnPrefab;
    
    public Unit GetSpawnPrefab(UnitInfo unitInfo)
    {
        if(!_unitSpawnPrefab)
        {
            _unitSpawnPrefab = UnitPrefabDataManager.Instance.GetUnitPrefab(unitInfo);
        }
        return _unitSpawnPrefab;
    }
    
    public float SpawnHeightOffset => _spawnHeightOffset;
}
