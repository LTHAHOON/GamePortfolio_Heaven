using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseLaserController : WeaponController
{
    //사격 전술
    public enum LaserDelayPattern
    {
        [InspectorName("=========")]
        Salvo,
        [Header("LaserPoint 최소 2개 이상")]
        [InspectorName("_-_-_-_-_-")]
        Ripple,
        [Header("LaserPoint 최소 2개 이상")]
        [InspectorName("_ _-- _ _-- _ _--")]
        Ripple2,
    }
    [SerializeField]
    private LaserDelayPattern _laserDelayPattern = LaserDelayPattern.Salvo;
    [SerializeField]
    private Transform[] _laserPoints;
    [SerializeField]
    private Unit _owner;
    private DamageableColliderGroup _lastDamageableColliderGroup;
    private RaycastHit _lastHit;
    private PoolComponent<PulseLaser> _laserPoolComponent;
    private PulseLaser pulseLaserPrefab;
    //한 프레임에 RaserPoint 몇개 
    private int _laserCount;
    //Salvo일 경우 사용하지 않음
    private int _curRaserPointIndex = 0;
    private int _rippleCount = 0;
    private float _delayTime = 0f;
    private float _curTime = 0f;
    public override Unit Owner => _owner;

    private void Awake()
    {
        Init();
    }

    public void SetCountAndIndexByPattern()
    {
        
        switch(_laserDelayPattern)
        {
            case LaserDelayPattern.Salvo:
                {
                    _laserCount = _laserPoints.Length;
                    _curRaserPointIndex = -1;
                    break;
                }
            case LaserDelayPattern.Ripple:
                {
                    _laserCount = 1;
                    if ((_curRaserPointIndex + 1) >= _laserPoints.Length)
                    {
                        _curRaserPointIndex = 0;
                        return;
                    }
                    ++_curRaserPointIndex;
                    break;
                }
            case LaserDelayPattern.Ripple2:
                {
                    _laserCount = 1;
                   
                    if (_rippleCount > 0 && _rippleCount % 2 == 0)
                    {
                        _rippleCount = 0;
                        ++_curRaserPointIndex;
                    }
                    if ((_curRaserPointIndex) >= _laserPoints.Length)
                    {
                        _rippleCount = 0;
                        _curRaserPointIndex = 0;
                    }
                    ++_rippleCount;

                    break;
                }
        }
    }


    public override void Init()
    {
        pulseLaserPrefab = _weaponPrefab as PulseLaser;
        _delayTime = pulseLaserPrefab.DelayTime;
        Transform weaponPropParent = UnitStorageManager.Instance.GetUnitWeaponPropParent();
        int instanceID = GetInstanceID();
        PoolManager.Instance.AddPoolUsingID(instanceID, pulseLaserPrefab, poolParent : weaponPropParent);
        PoolManager.Instance.TryGetPoolUsingID(instanceID, out _laserPoolComponent);
    }

    public override void Attack()
    {
        _curTime += Time.deltaTime;
        if(_curTime > _delayTime)
        {
            _curTime = 0f;
            SetCountAndIndexByPattern();
            for (int i = 0; i < _laserCount; i++)
            {
                if (_laserPoints.Length <= i)
                    continue;
                int curLaserPointIndex = _curRaserPointIndex;
                if (curLaserPointIndex < 0)
                {
                    curLaserPointIndex = i;
                }
                RaycastHit targetHit = InputManager.Instance.GetBySphereCast(_laserPoints[curLaserPointIndex].position,
                                                            _laserPoints[curLaserPointIndex].forward, pulseLaserPrefab.RayRadius, pulseLaserPrefab.RayDistance, GameLayerMask.DamageableColliderMask);
                if (!targetHit.collider)
                    continue;
                if(targetHit.collider == _lastHit.collider)
                {
                    OnFire(curLaserPointIndex, _lastDamageableColliderGroup);
                }
                else if (targetHit.collider.CompareTag(GameTags.Enemy) && targetHit.collider.transform.parent.TryGetComponent(out DamageableColliderGroup damagableColliderGroup))
                {
                    OnFire(curLaserPointIndex, damagableColliderGroup);
                    _lastHit = targetHit;
                    _lastDamageableColliderGroup = damagableColliderGroup;
                }
            }
        }
    }

    private void OnFire(int curLaserPointIndex, DamageableColliderGroup damagableColliderGroup)
    {
        PulseLaser raser = _laserPoolComponent.PopPoolObject();
        if (!raser)
            return;
        _delayTime = raser.DelayTime;
        raser.OnFire(this, damagableColliderGroup.GetHealth(), _laserPoolComponent, _laserPoints[curLaserPointIndex].position);
    }
    private void OnDestroy()
    {
        PoolManager.Instance.RemovePoolUsingID(GetInstanceID());
    }
}
