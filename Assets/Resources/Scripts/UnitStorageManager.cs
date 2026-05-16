using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnitStorageManager;

public class UnitStorageManager : Singleton<UnitStorageManager>
{
    //GameObject 또는 Component가 아닌 T로 지정한 이유는 GetComonent를 하지 않기 위해서 입니다.
    [Serializable]
    public struct StorageChild<T> where T : Unit
    {
        public UnitType _unitType;
        public GameObject _child;
        public List<T> _unitList;
    }
    [Header("생성된 본인 유닛 저장소")]
    [SerializeField]
    private StorageChild<Unit>[] _arrStorageUnit;
    [Header("생성된 상대 유닛 저장소")]
    [SerializeField]
    private StorageChild<Unit>[] _arrStorageUnit_Enemy;

    private readonly Dictionary<UnitType, StorageChild<Unit>> _dicUnitStorage = new();
    private readonly Dictionary<UnitType, StorageChild<Unit>> _dicUnitStorage_Enemy = new();
    private void Awake()
    {
        for (int i = 0; i < _arrStorageUnit.Length; i++)
        {
            _dicUnitStorage.Add(_arrStorageUnit[i]._unitType, _arrStorageUnit[i]);
        }
        for (int i = 0; i < _arrStorageUnit.Length; i++)
        {
            _dicUnitStorage_Enemy.Add(_arrStorageUnit[i]._unitType, _arrStorageUnit_Enemy[i]);
        }
    }

    private Dictionary<UnitType, StorageChild<Unit>> GetDictionaryStorage(Faction faction)
    {
        return faction switch
        {
            Faction.Ally => _dicUnitStorage,
            Faction.Enemy => _dicUnitStorage_Enemy,
            _ => null
        };
    }

    public void AddUnitToStorageList(Faction faction, UnitType unitType, Unit unitprefab)
    {
        bool bGetChild = TryGetUnitList(out List<Unit> unitList, faction, unitType);
        if(bGetChild)
        {
            unitList.Add(unitprefab);
        }
    }
    public void RemoveUnitToStorageList(Faction faction, UnitType unitType, Unit unitprefab, float dieDelayTime = 0f, CreatureAnimatorStatData animatorStatData = null)
    {
        if (ContainsUnitPrefab(faction, unitprefab,  unitType))
        {
            var storage = GetDictionaryStorage(faction);
            storage[unitType]._unitList.Remove(unitprefab);
            unitprefab.GetClickCollider().enabled = false;
            animatorStatData?._animator.SetTrigger(animatorStatData._dicAnimParameterHash[CreatureAnimParameter.Die]);
            StartCoroutine(IEDestroyUnit(unitprefab, dieDelayTime));
        }

    }
    public IEnumerator IEDestroyUnit(Unit unitPrefab, float dieDelayTime)
    {
        yield return new WaitForSeconds(dieDelayTime);
        Destroy(unitPrefab.gameObject);
    }

    public bool TryGetUnitList(out List<Unit> unitList, Faction faction, UnitType unitType)
    {
        var storage = GetDictionaryStorage(faction);
        if (storage.ContainsKey(unitType))
        {
            unitList = storage[unitType]._unitList;
            return true;
        }
        unitList = default;
        return false;
    }

    public bool ContainsUnitPrefab(Faction faction, Unit unitPrefab, UnitType unitType)
    {
        var storage = GetDictionaryStorage(faction);
        return storage[unitType]._unitList.Contains(unitPrefab);
    }
    public bool ContainsUnitType(Faction faction, UnitType unitType)
    {
        var storage = GetDictionaryStorage(faction);
        return storage.ContainsKey(unitType);
    }

    public bool TryGetChild(out GameObject child, Faction faction, UnitType unitType)
    {
        if(ContainsUnitType(faction, unitType))
        {
            var storage = GetDictionaryStorage(faction);
            child = storage[unitType]._child;
            return true;
        }
        child = default;
        return false;
    }
}
