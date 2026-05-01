using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUnitChipState : UnitChipState
{
    [Header("Home Extent(면적)")]
    [SerializeField]
    private float _horizontalLength;
    [SerializeField]
    private float _verticalLength;
    [Header("Home Function(기능 설명)")]
    [SerializeField]
    private string _functionDecription;
    
    public float GetHorizontalLength()
    {
        return _horizontalLength;
    }

    public float GetVerticalLength()
    {
        return _verticalLength;
    }

    public string GetFunctionDecription()
    {
        return _functionDecription;
    }
    
}
