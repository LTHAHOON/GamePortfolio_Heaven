using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BaseUnitStatusData : ScriptableObject
{
    public abstract AllStatusNames[] GetAllStatusNames();
    [SerializeField]
    private UnitData _unitData;
    
    public UnitData UnitData => _unitData;
}
