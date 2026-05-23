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
    [SerializeField]
    private Transform _weaponPropParent;
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

    public void AddUnitToStorageList(Faction faction, UnitType unitType, Unit unit)
    {
        bool bGetChild = TryGetUnitList(out List<Unit> unitList, faction, unitType);
        if(bGetChild)
        {
            unitList.Add(unit);
        }
    }
    public void RemoveUnitToStorageList(Faction faction, UnitType unitType, Unit unit, float dieDelayTime = 0f, Animator animator = null, int animParaID = -1)
    {
        if (ContainsUnit(faction, unit,  unitType))
        {
            var storage = GetDictionaryStorage(faction);
            storage[unitType]._unitList.Remove(unit);
            unit.GetClickCollider().enabled = false;
            animator?.SetTrigger(animParaID);
            StartCoroutine(IEDestroyUnit(unit, dieDelayTime));
        }

    }
    public IEnumerator IEDestroyUnit(Unit unit, float dieDelayTime)
    {
        yield return new WaitForSeconds(dieDelayTime);
        Destroy(unit.gameObject);
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

    public bool ContainsUnit(Faction faction, Unit unit, UnitType unitType)
    {
        var storage = GetDictionaryStorage(faction);
        return storage[unitType]._unitList.Contains(unit);
    }
    public bool ContainsUnitType(Faction faction, UnitType unitType)
    {
        var storage = GetDictionaryStorage(faction);
        return storage.ContainsKey(unitType);
    }

    public Transform GetUnitWeaponPropParent() => _weaponPropParent;

    public bool TryGetUnitParent(out GameObject parent, Faction faction, UnitType unitType)
    {
        if(ContainsUnitType(faction, unitType))
        {
            var storage = GetDictionaryStorage(faction);
            parent = storage[unitType]._child;
            return true;
        }
        parent = default;
        return false;
    }
}
