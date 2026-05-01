using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MPData
{
    public float MP_ConsValue;
}
[System.Serializable]
public struct ConsumeMPValue
{
    public AllStatusNames statusName;
    public int consumeMPValue;
}

public class MPDataComponent : MonoBehaviour
{
    [SerializeField]
    private MPData _mpData = new() { MP_ConsValue = 0};


    public MPData GetMPData()
    {
        return _mpData;
    }
}
