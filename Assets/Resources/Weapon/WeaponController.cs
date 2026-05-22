using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController :MonoBehaviour
{
    public virtual Unit Owner { get; }
    [SerializeField]
    protected WeaponBase _weaponPrefab;

    public virtual void Init() { }
    public virtual void Attack() { }

}
