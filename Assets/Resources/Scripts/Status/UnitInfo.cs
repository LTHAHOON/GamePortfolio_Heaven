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
    [SerializeField] 
    private StatusDBType _statusDBType;
    [SerializeField]
    private StatusSldiersType _sliderType = StatusSldiersType.Base;
    //MP 공유 데이터
    public MPData MPData { get; set; }

    //Status 공유 데이터
    public RuntimeUnitStatus Status { get; set; }
    public StatusDBType StatusDBType => _statusDBType;
    public StatusSldiersType SliderType => _sliderType;
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