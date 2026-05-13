using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoorTriggerBox : MonoBehaviour
{
    public delegate void DoorTrigger(bool isClose);
    public DoorTrigger OnAutoDoorTrigger;
    private readonly HashSet<Collider> _triggerColliders = new();
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == GameLayer.CreatureLayer)
        {
            _triggerColliders.Add(other);
            OnAutoDoorTrigger?.Invoke(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == GameLayer.CreatureLayer)
        {
            _triggerColliders.Remove(other);
            if (_triggerColliders.Count <= 0)
            {
                OnAutoDoorTrigger?.Invoke(true);
            }
        }
    }
}
