using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(menuName = "Database/PrefabDB")]
public class UnitPrefabDB : ScriptableObject
{
    [SerializeField] 
    private UnitType _unitDataBaseType;
    [SerializeField] 
    private List<Unit> _unitPrefabs = new();


    public Unit GetUnitPrefab(long id)
    {
        for (int i = 0; i < _unitPrefabs.Count; i++)
        {
            if (_unitPrefabs[i].UnitInfo.ID == id)
            {
                return _unitPrefabs[i];
            }
        }

        return null;
    }
    
    public List<Unit> UnitPrefabs => _unitPrefabs;
    public UnitType UnitDataBaseType => _unitDataBaseType;
}