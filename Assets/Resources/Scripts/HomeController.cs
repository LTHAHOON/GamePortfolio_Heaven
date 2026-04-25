using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(StatusComponent))]
[RequireComponent(typeof(BoxCollider))]
public class HomeController : Unit
{
    [SerializeField]
    private Health _health;
    [SerializeField]
    private CreateLoad _createLoad;
    [SerializeField]
    private LayerMask _targetLayerMask;
    private int _targetLayer;
    [SerializeField]
    private float gravity = -9.81f; // �߷� ���ӵ�
    [SerializeField]
    private int _hitMaxCount = 5;
    [SerializeField]
    private float _healInterval = 2f;
    [SerializeField]
    private HomeParticleSystem _homeParticleSystem;

    private RuntimeUnitStatus _status;

    private BoxCollider _collider;
    private Rigidbody _rigidbody;
    private UnitType _unitType;
    protected override void Awake()
    {
        base.Awake();
        hit = new RaycastHit[_hitMaxCount];
        _targetLayer = (int)Mathf.Log(_targetLayerMask, 2);
        _collider = GetComponent<BoxCollider>();
        _status = GetComponent<StatusComponent>().GetStatus();
        _unitType = GetComponent<StatusComponent>().GetUnitData().Type;
    }
    public void SetStatus(RuntimeUnitStatus status)
    {
        _status = status;
    }
    private void FixedUpdate()
    {
        if (_isGravity)
        {
            GravityMove();
        }
    }

    [SerializeField]
    private float _distanceFromCenter;

    public float _fixedHealingPower = 0.05f;
    private bool _bOnceInit = true;
    private void Update()
    {
        if (_createLoad.IsLoadReady)
        {
            return;
        }
        if (_bOnceInit)
        {
            _health.InitHealth();
            TransparentMaterialControl.SetQpaqueOrTransparentControl(gameObject, _unitType, TransparentMaterialControl.SurfaceType.Opaque, new Color32(255, 255, 255, 255));
            _bOnceInit = false;
        }
        HomeFunction();
    }


    private RaycastHit[] hit;
    private void HomeFunction()
    {
        if (_status != null && !_isGravity && hit != null)
        {
            int hitCount = Physics.SphereCastNonAlloc(transform.position, _distanceFromCenter, transform.up, hit, 0f, _targetLayerMask);
            if (hitCount > 0)
            {
                for (int i = 0; i < hitCount; i++)
                {
                    if (transform.CompareTag(hit[i].transform.gameObject.tag) && hit[i].transform.gameObject.layer == _targetLayer)
                    {
                        if (hit[i].transform.gameObject.TryGetComponent(out Health health))
                        {
                            Debug.Log("Healing " + hit[i].transform.gameObject.name);
                            if (health._isModifyingHealth) return;
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
    }


    private void GravityMove()
    {
        transform.position += gravity * Time.fixedDeltaTime * transform.up;
    }


    public bool _isGravity = false;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(GameTags.Ground) && _isGravity)
        {
            _createLoad.StartCreateLoad();
            _collider.isTrigger = false;
            _isGravity = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(GameTags.Ground) && _isGravity)
        {
            //  _collider.isTrigger = false;
            // _isGravity = false;
        }
    }
    public CreateLoad GetCreateLoad() => _createLoad;

}
