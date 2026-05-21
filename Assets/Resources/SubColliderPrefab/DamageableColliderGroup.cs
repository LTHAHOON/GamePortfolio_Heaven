using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableColliderGroup : MonoBehaviour
{
    [SerializeField]
    private Health _health;
    
    public Health GetHealth()
    {
        return _health;
    }
}
