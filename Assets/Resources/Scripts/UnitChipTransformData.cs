using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New UnitTransformData", menuName = "UnitTransformData", order = 1)]
public class UnitChipTransformData : ScriptableObject
{
    public  float _posX;
    public  float _posY;
    public  float _posZ;
    public float _width;
    public float _height;
    public float _scaleX;
    public float _scaleY;
    public float _scaleZ;
}
