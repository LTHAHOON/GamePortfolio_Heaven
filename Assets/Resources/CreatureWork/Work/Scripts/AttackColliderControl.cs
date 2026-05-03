using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;



public class AttackColliderControl : HitColliderControl
{
    protected override void Hit(GameObject target)
    {
        if(target.transform.gameObject.TryGetComponent(out Health health))
        {
            health.ModifyHealth(-(_owner.Status.ATK * _fixedHitValue));
        }
    }
}
