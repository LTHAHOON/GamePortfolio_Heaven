using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(StatusComponent))]
[RequireComponent(typeof(BoxCollider))]
public class HomeController : Unit
{
    #region Heal 데이터
    [SerializeField]
    private int _hitMaxCount = 5;
    [SerializeField]
    private float _healInterval = 2f;
    [SerializeField]
    private float _distanceFromCenter;
    [SerializeField]
    private HomeParticleSystem _homeParticleSystem;
    private readonly float _fixedHealingPower = 0.05f;
    private RaycastHit[] hit;
    #endregion
    #region 중력 데이터
    [SerializeField]
    private float gravity = -9.81f;
    [HideInInspector]
    public bool _isGravity = false;
    #endregion
    [SerializeField]
    private CreateLoad _createLoad;
    [SerializeField]
    private LayerMask _targetLayerMask;
    private int _targetLayer;
    private RuntimeUnitStatus _status;
    private BoxCollider _collider;

    #region 이벤트 함수
    protected override void Awake()
    {
        base.Awake();
        hit = new RaycastHit[_hitMaxCount];
        _targetLayer = (int)Mathf.Log(_targetLayerMask, 2);
        _collider = GetComponent<BoxCollider>();

    }
    private void Update()
    {
        HomeFunction();
    }
    private void FixedUpdate()
    {
        if (_isGravity)
        {
            GravityMove();
        }
    }
    #endregion

    private void Initialize()
    {
        SetStatus();
        _health.InitHealth(_status);
        TransparentMaterialControl.SetQpaqueOrTransparentControl(gameObject, _unitType, TransparentMaterialControl.SurfaceType.Opaque, new Color32(255, 255, 255, 255));
    }
    private void SetStatus()
    {
        _status = GetComponent<StatusComponent>().GetStatus();
    }

    private void HomeFunction()
    {
        if (_status != null || hit != null || !_isGravity)
            return;

        int hitCount = Physics.SphereCastNonAlloc(transform.position, _distanceFromCenter, transform.up, hit, 0f, _targetLayerMask);
        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; i++)
            {
                if (transform.CompareTag(hit[i].transform.gameObject.tag) && hit[i].transform.gameObject.layer == _targetLayer)
                {
                    if (hit[i].transform.gameObject.TryGetComponent(out Health health))
                    {
                        if (health._isModifyingHealth || health.CurrentHealth <= 0) return;
                        float healAmount = _status.HealingPower * _fixedHealingPower;

                        health.ModifyHealth(-healAmount, _healInterval);
                        _homeParticleSystem.PlayHealingCircle();
                    }
                    else
                    {
                        Debug.Log("ERROR: You can't find Health Component");
                    }
                }
            }
        }
        else
        {
            _homeParticleSystem.StopHealingCircle();
        }
    }

    private void GravityMove()
    {
        transform.position += gravity * Time.fixedDeltaTime * transform.up;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(GameTags.Ground) && _isGravity)
        {
            _createLoad.StartCreateLoad(() => Initialize());
            _collider.isTrigger = false;
            _isGravity = false;
        }
    }

    public CreateLoad GetCreateLoad() => _createLoad;

}
