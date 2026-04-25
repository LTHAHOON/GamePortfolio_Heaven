using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitColliderControl : MonoBehaviour
{
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    protected StatusComponent _statusComponent;
    [SerializeField]
    private int _hitTriggerCount = 1;
    [SerializeField]
    protected float _fixedHitValue = 10;
    private int _curHitTriggerCount = 0;
    private List<int> _hitTargetList = new();

    public virtual void Awake()
    {
        DisableHit();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.CompareTag(GameTags.Enemy) || other.CompareTag(GameTags.EnemyNexus))
        {
            CheckHit(other.gameObject);
        }
    }

    private void CheckHit(GameObject target)
    {
        int id = target.GetInstanceID();
        if (!_hitTargetList.Contains(id))
        {
            if(_hitTriggerCount > 1)
            {
                _hitTargetList.Add(target.GetInstanceID());
            }
            Hit(target);
            ++_curHitTriggerCount;
            if (_hitTriggerCount <= _curHitTriggerCount)
            {
                _curHitTriggerCount = 0;
                DisableHit();
            }
        }
    }

    protected abstract void Hit(GameObject target);

    public void EnableHit()
    {
        _collider.enabled = true;
    }

    public void DisableHit()
    {
        _hitTargetList.Clear();
        _collider.enabled = false;
    }
}
