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

[System.Serializable]
public abstract class BaseUnitData
{

    public UnitType Type;
    public UnitProperty Property;
    public long ID;
}
