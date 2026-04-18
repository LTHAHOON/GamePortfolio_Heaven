using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUnitStatusData : ScriptableObject
{
    public abstract AllStatusNames[] GetAllStatusNames();
    public UnitData _unitData;
    
}
