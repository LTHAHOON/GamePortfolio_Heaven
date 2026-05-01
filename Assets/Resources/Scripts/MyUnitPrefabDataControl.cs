using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static MyUnitPrefabDataControl;

public class MyUnitPrefabDataControl : Singleton<MyUnitPrefabDataControl>
{
    //GameObject 또는 Component가 아닌 T로 지정한 이유는 GetComonent를 하지 않기 위해서 입니다.
    [Serializable]
    public struct StorageChild<T> where T : Unit
    {
        public UnitType _unitType;
        public GameObject _child;
        public List<T> _unitList;
    }
    //
    [SerializeField]
    private StorageChild<Unit>[] _arrStorageUnit;

    private readonly Dictionary<UnitType, StorageChild<Unit>> _dicUnitStorage = new();
    private void Awake()
    {
        for (int i = 0; i < _arrStorageUnit.Length; i++)
        {
            _dicUnitStorage.Add(_arrStorageUnit[i]._unitType, _arrStorageUnit[i]);
        }
    }
    public void AddUnitPrefabToList(UnitType unitType, Unit unitprefab)
    {
        bool bGetChild = TryGetUnitList(out List<Unit> unitList, unitType);
        if(bGetChild)
        {
            unitList.Add(unitprefab);
        }
    }
    public void RemoveUnitPrefabToList(UnitType unitType, Unit unitprefab, float dieDelayTime = 0f, AnimatorStatData animatorStatData = null)
    {
        if (ContainsUnitPrefab(unitprefab, unitType))
        {
            _dicUnitStorage[unitType]._unitList.Remove(unitprefab);
            unitprefab.GetClickCollider().enabled = false;
            animatorStatData?._animator.SetTrigger(animatorStatData._dicAnimParameterHash[AnimParameter.Die]);
            StartCoroutine(IEDestroyUnit(unitprefab, dieDelayTime));
        }

    }
    public IEnumerator IEDestroyUnit(Unit unitPrefab, float dieDelayTime)
    {
        yield return new WaitForSeconds(dieDelayTime);
        Destroy(unitPrefab.gameObject);
    }

    public bool TryGetUnitList(out List<Unit> unitList, UnitType unitType)
    {
        if (_dicUnitStorage.ContainsKey(unitType))
        {
            unitList = _dicUnitStorage[unitType]._unitList;
            return true;
        }
        unitList = default;
        return false;
    }

    public bool ContainsUnitPrefab(Unit unitPrefab, UnitType unitType)
    {
        return _dicUnitStorage[unitType]._unitList.Contains(unitPrefab);
    }
    public bool ContainsUnitType(UnitType unitType)
    {
        return _dicUnitStorage.ContainsKey(unitType);
    }

    public bool TryGetChild(out GameObject child, UnitType unitType)
    {
        if(ContainsUnitType(unitType))
        {
            child = _dicUnitStorage[unitType]._child;
            return true;
        }
        child = default;
        return false;
    }
}
