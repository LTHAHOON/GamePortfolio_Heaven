using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class MPData
{
    public float MP_ConsValue = 0f;
    
    public void SetMPConsValue(float mpConsValue)
    {
        MP_ConsValue = mpConsValue;
    }
}
[System.Serializable]
public class StatusUnitMpData : MPData
{
    public AllStatusNames statusName;
}

public class UnitMPDataComponent : MonoBehaviour
{
    [SerializeField]
    private MPData _mpData;
    

    public MPData UnitMPData => _mpData;
    public float MP_ConsValue => _mpData.MP_ConsValue;
}
