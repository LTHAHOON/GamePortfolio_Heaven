using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{

}

public class Weapon<TData> : WeaponBase where TData : WeaponData
{
    [SerializeField]
    protected TData _weaponData;
}
