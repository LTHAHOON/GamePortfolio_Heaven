using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "WeaponData/BulletData")]
public class BulletData : WeaponData
{
    [SerializeField]
    private float _rayRadius = 5f;
    [Header("추적할 수 있는 최대 거리")]
    [SerializeField]
    private float _rayDistance = 200f;
    [Header("딜레이")]
    [SerializeField]
    private float _dealyTime = 0.1f;
    [Header("속도")]
    [SerializeField]
    private float _pulseSpeed = 100f;

    public float RayRadius => _rayRadius;
    public float RayDistance => _rayDistance;
    public float DealyTime => _dealyTime;
    public float PulseSpeed => _pulseSpeed;

}
