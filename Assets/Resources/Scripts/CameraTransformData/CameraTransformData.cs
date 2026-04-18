using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "new CameraTransformData", menuName = "CameraTransformData", order = 2)]
public class CameraTransformData : ScriptableObject
{
    [Header("Clamp_X")]
    public float max_x;
    public float min_x;

    [Space(3)]
    [Header("Clamp_Y")]
    public float max_y;
    public float min_y;

    [Space(3)]
    [Header("Clamp_Z")]
    public float max_z;
    public float min_z;

}
