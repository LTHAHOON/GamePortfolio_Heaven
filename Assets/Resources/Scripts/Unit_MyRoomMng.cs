using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.CanvasScaler;

public class Unit_MyRoomMng : Singleton<Unit_MyRoomMng>
{
    [Header("모든 유닛칩")]
    public List<UnitChipState> _allUnitChip; //모든 유닛칩

    public List<UnitChipState> _unDoAllUnitChip; //이전 모든 유닛칩
    [System.Serializable]
    public struct RemainingUnitChips
    {
        public List<UnitChipState> _remainingCreatureChips;
        public List<UnitChipState> _remainingSpacecraftChips;
        public List<UnitChipState> _remainingHomeChips;
        public List<UnitChipState> _remainingShieldChips;

        public List<UnitChipState>[] ToList()
        {
            return new List<UnitChipState>[] 
            {
                _remainingCreatureChips,
                _remainingSpacecraftChips,
                _remainingHomeChips,
                _remainingShieldChips
            };
        }

        public void Clear()
        {
            _remainingCreatureChips.Clear();
            _remainingSpacecraftChips.Clear();
            _remainingHomeChips.Clear();
            _remainingShieldChips.Clear();
        }
    }
    [System.Serializable]
    public struct AddedUnitChips
    {
        public List<UnitChipState> _addedCreatureChips;
        public List<UnitChipState> _addedSpacecraftChips;
        public List<UnitChipState> _addedHomeChips;
        public List<UnitChipState> _addedShieldChips;

        public List<UnitChipState>[] ToList()
        {
            return new List<UnitChipState>[]
            {
                _addedCreatureChips,
                _addedSpacecraftChips,
                _addedHomeChips,
                _addedShieldChips
            };
        }


        public void Clear()
        {
            _addedCreatureChips.Clear();
            _addedSpacecraftChips.Clear();
            _addedHomeChips.Clear();
            _addedShieldChips.Clear();
        }
    }

    [Header("이전 남은 유닛칩")]
    public RemainingUnitChips _unDoRemainingUnitChips; //이전 남은 유닛칩
    [Header("이전 추가된 유닛칩")]
    public AddedUnitChips _unDoAddedUnitChips; //이전 추가된 유닛칩

    [Header("남은 유닛칩")]
    public RemainingUnitChips _remainingUnitChips; //남은 유닛칩
    [Header("추가된 유닛칩")]
    public AddedUnitChips _addedUnitChips; // 추가된 유닛칩


    public List<UnitChipState> FindAddedUnitChipWithType(UnitType unitType)
    {
        switch (unitType)
        {
            case UnitType.Creature:
                return _addedUnitChips._addedCreatureChips;

            case UnitType.Spacecraft:
                return _addedUnitChips._addedSpacecraftChips;

            case UnitType.Home:
                return _addedUnitChips._addedHomeChips;

            case UnitType.Shield:
                return _addedUnitChips._addedShieldChips;

            default:
                return null;
        }
    }
    public List<UnitChipState> FindRemaingUnitChipWithType(UnitType unitType)
    {
        switch (unitType)
        {
            case UnitType.Creature:
                return _remainingUnitChips._remainingCreatureChips;

            case UnitType.Spacecraft:
                return _remainingUnitChips._remainingSpacecraftChips;

            case UnitType.Home:
                return _remainingUnitChips._remainingHomeChips;

            case UnitType.Shield:
                return _remainingUnitChips._remainingShieldChips;

            default:
                return null;
        }
    }

    //TODO: 분리된 유닛칩 딕셔너리 가져오기
    public Dictionary<int, UnitChipState[]> GetDicUnitChip(List<UnitChipState>[] arrUnitChipList)
    {
        Dictionary<int, UnitChipState[]> dicUnitChip = new Dictionary<int, UnitChipState[]>(4);

        for (int i = 0; i < arrUnitChipList.Length; i++)
        {
            dicUnitChip[i] = arrUnitChipList[i].ToArray();
        }

        return dicUnitChip;
    }

    public void AddUnitChip(UnitChipState unitChipToAdd, Func<UnitType, List<UnitChipState>> findUnitChipWithType)
    {
        List<UnitChipState> unitPrefabList = findUnitChipWithType(unitChipToAdd.GetUnitData().Type);
        if (unitPrefabList != null)
        {
           

            unitPrefabList.Add(unitChipToAdd);
        }
        else
        {
            Debug.Log("ERROR: unitPrefabList == null");
        }
    }

    public void RemoveUnitChip(UnitChipState unitChipToAdd, Func<UnitType, List<UnitChipState>> findUnitChipWithType)
    {
        List<UnitChipState> unitPrefabList = findUnitChipWithType(unitChipToAdd.GetUnitData().Type);
        if (unitPrefabList != null)
        {
            unitPrefabList.Remove(unitChipToAdd);
        }
        else
        {
            Debug.Log("ERROR: unitPrefabList == null");
        }
    }

    public void RemoveUnitOfAllUnitChip(UnitChipState unitChipToDel)
    {
        _allUnitChip.Remove(unitChipToDel);
    }

    //TODO: 유닛칩 생성하기
    public UnitChipState[] GetAllUnitChips(bool isUnDo)
    {
        UnitChipState[] allUnitPrefabTemp = isUnDo ? _unDoAllUnitChip.ToArray() : _allUnitChip.ToArray(); 
        return allUnitPrefabTemp;
    }

    public void AddToUnDoAllUnitChip()
    {
        AddToUnDoAllUnitChip(_addedUnitChips.ToList());
        AddToUnDoAllUnitChip(_remainingUnitChips.ToList());
    }

    private void AddToUnDoAllUnitChip(List<UnitChipState>[] unitChips)
    {
        for (int i = 0; i < unitChips.Length; i++)
        {
            UnitChipState[] arrUnitChips = unitChips[i].ToArray();
            _unDoAllUnitChip.AddRange(arrUnitChips);
        }
    }

    public void ClearUnDoAllUnitChip()
    {
        _unDoAllUnitChip.Clear(); 
    }
}
