using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType 
{ 
    Right_Melee = 0,
    Left_Melee = 1,
    Projectile = 2, 
}
public class HitAnimationEvent : MonoBehaviour
{
    [Serializable] 
    private struct HitCollider 
    { 
        public HitType _hitKType; 
        public HitColliderControl _hitColliderControl; 
    }
    [SerializeField] private HitCollider[] _hitColliders;
    private Dictionary<int, HitColliderControl> _dicHitColliders = new();

    private void Awake()
    {
        for (int i = 0; i < _hitColliders.Length; i++)
        {
            _dicHitColliders[(int)_hitColliders[i]._hitKType] = _hitColliders[i]._hitColliderControl;
        }
    }
    public void EnableHit(int hitKey)
    {
        if (_dicHitColliders.TryGetValue(hitKey, out HitColliderControl hitCollider))
        {
            hitCollider.EnableHit();
        }
    }
    public void DisableHit(int hitKey)
    {
        if (_dicHitColliders.TryGetValue(hitKey, out HitColliderControl hitCollider))
        {
            hitCollider.DisableHit();
        }
    }
}
