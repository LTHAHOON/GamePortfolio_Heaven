using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseLaser : Weapon<BulletData>
{
    [SerializeField]
    private TrailRenderer _trailRenderer;
    [Header("추적할 수 있는 최대 지름")]
    [SerializeField]
    private PoolComponent<PulseLaser> _raserPoolComponent;
    private float _maxTime = 1f;
    private float _curTime = 0f;
    private bool _isFiring = false;
    private Vector3 _startPoint;
    private Vector3 _fireDirection;
    private Health _targetHealth;
    private Unit _owner;

    private void Awake()
    {
        Init();
    }
    private void Update()
    {
        if(_isFiring)
        {
            Fire();
            if(IsWeakRaser())
            {
                float damage = _owner.Status.ATK * _weaponData.FixedDamage;
                _targetHealth.ModifyHealth(-damage);
                Init();
                _raserPoolComponent?.ReturnPoolObject(this);
            }
        }
    }

    private void OnDisable()
    {
        SetActiveTrail(false);
    }

    public void Init()
    {
        SetActiveTrail(false);
        _curTime = 0f;
        _isFiring = false;
        _owner = null;
        _targetHealth = null;
        _fireDirection = Vector3.zero;
    }

    public void Fire()
    {
        _curTime += Time.deltaTime;
        float movePos = _curTime * _weaponData.PulseSpeed;
        transform.position = _startPoint + (_fireDirection * movePos);
        SetActiveTrail(true);
    }
    
    public void SetActiveTrail(bool active)
    {
        if(_trailRenderer.enabled != active)
        {
            _trailRenderer.enabled = active;
        }
    }

    public void OnFire<T>(T caller, Health targetHealth, PoolComponent<PulseLaser> raserPoolComponent, Vector3 startPoint) where T : WeaponController
    {
        SetActiveTrail(false);
        _trailRenderer.Clear();
        _isFiring = true;
        if(_raserPoolComponent == null)
        {
            _raserPoolComponent = raserPoolComponent;
        }
        _startPoint = startPoint;
        _targetHealth = targetHealth;
        _owner = caller.Owner;
        _fireDirection = targetHealth.transform.position - startPoint;
        //실제 거리 계산
        float distance = _fireDirection.magnitude;
        _fireDirection.Normalize();
        _maxTime = distance / _weaponData.PulseSpeed;
    }
    public bool IsWeakRaser() => _curTime >= _maxTime;
    public float RayRadius => _weaponData.RayRadius;
    public float RayDistance => _weaponData.RayDistance;
    public float DelayTime => _weaponData.DealyTime;
}
