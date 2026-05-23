using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : ScriptableObject
{
    [SerializeField]
    private GameObject _hitEffectPrefab;
    [SerializeField]
    private float _fixedDamage = 1.5f;
    public float FixedDamage => _fixedDamage;
    public GameObject HitEffectPrefab => _hitEffectPrefab;
}

