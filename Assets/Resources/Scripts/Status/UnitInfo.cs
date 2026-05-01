using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum UnitType
{
    Creature,
    Spacecraft,
    Home,
    Nexus,
    Shield,
    None
}
public enum UnitProperty
{
    FireProperty,
    WaterProperty,
    GroundProperty,
    ElectricityProperty,
    None,
}

public class UnitInfo : MonoBehaviour
{
    [SerializeField]
    private UnitData _unitData;
    
    public UnitType Type => _unitData.Type;
    public UnitProperty Property => _unitData.Property;
    public long ID => _unitData.ID;
}

[System.Serializable]
public class UnitData
{
    public UnitType Type;
    public UnitProperty Property;
    public long ID;
}