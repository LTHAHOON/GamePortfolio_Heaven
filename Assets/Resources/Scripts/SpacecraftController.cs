using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public enum SpacecraftState
{
    Idle,
    Boarding,
    Drive,
    Trace,
    Attack,
    Landing,
    GetOff,
    Die,
}

[RequireComponent(typeof(Rigidbody))]
public class SpacecraftController : PassengerController, ISelectableOwner
{
    #region State 데이터
    [Header("무기 데이터")]
    [SerializeField]
    private WeaponStatData _weaponStatData;
    [Header("우주선 Attack/Trace Distance 데이터")]
    [SerializeField]
    private BaseFSMStatData _fsmStatData;
    [Header("탑승시킬때 필요한 데이터")]
    [SerializeField]
    private BoardingStatData _boardingStatData;
    [Header("SurroundPos 데이터")]
    [SerializeField]
    private SurroundPosStatData _surroundPosStatData;
    [Header("행성, 우주 레이어 전환 데이터")]
    [SerializeField]
    private LayerTargetStatData _layerTargetStatData;
    [Header("Die 데이터")]
    [SerializeField]
    private DieStatData _dieStatData;
    private BezierCurveStatData _curveStatData = new();
    
    private StateMachine<SpacecraftState, SpacecraftController> _stateMachine;
    public StateMachine<SpacecraftState, SpacecraftController> StateMachine => _stateMachine;
    #endregion
    #region Physics 데이터
    [SerializeField]
    private float gravity = -9.81f;
    [HideInInspector]
    public bool _isGravity = false;
    private Rigidbody _rigidbody;
    private Collider _collider;
    #endregion
    #region AttackMark 데이터
    public event Action<GameObject> OnReturnDestMark;
    private GameObject _attackMark;
    #endregion

    [SerializeField]
    private CreateLoad _createLoad;
    public MonoBehaviour Owner => this;
    #region 이벤트 함수
    protected override void Awake()
    {
        base.Awake();
        #region StateMachine �ʱ�ȭ
        _stateMachine = new(this, new IStateData[]
        {
            _surroundPosStatData,
            _boardingStatData,
            _layerTargetStatData,
            _dieStatData,
            _curveStatData,
            _fsmStatData,
            _weaponStatData,
        });
        _stateMachine.AddState(new SpacecraftIdleState());
        _stateMachine.AddState(new SpacecraftTraceState());
        _stateMachine.AddState(new SpacecraftAttackState());
        _stateMachine.AddState(new SpacecraftBoardingState());
        _stateMachine.AddState(new SpacecraftDriveState());
        _stateMachine.AddState(new SpacecraftLandingState());
        _stateMachine.AddState(new SpacecraftGetOffState());
        _stateMachine.AddState(new SpacecraftDieState());
        _stateMachine.ChangeState(SpacecraftState.Idle);
        #endregion
        _clickCollider.enabled = false;
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
        _health.OnDie += Die;
    }
    
    private void Update()
    {
        if (_stateMachine.CurrentState != null)
        {
            _stateMachine.UpdateCurrentState();
        }
    }

    private void FixedUpdate()
    {
        if (_isGravity && !_rigidbody.isKinematic)
        {
            GravityMove();
        }

    }
    #endregion

    private void Initialize()
    {
        MasterMaterialMng.Instance.SetQpaqueOrTranslucent(this, SurfaceType.Opaque);
        SetUpUnit();
        _clickCollider.enabled = true;
        _health.SetActiveHealthBar(true);
    }

    public IEnumerator IEBoading(float elapsedTime)
    {
        yield return new WaitForSeconds(elapsedTime);
        _stateMachine.ChangeState(SpacecraftState.Boarding);
    }

    private void GravityMove()
    {
        _rigidbody.velocity += gravity * Time.fixedDeltaTime * Vector3.up;
    }

    public void SetGoal(Vector3 startPoint, Vector3 endPoint, Vector3 middlePoint, Goal goalData)
    {
        Initialize();
        _collider.isTrigger = true;
        _rigidbody.isKinematic = false;
        _curveStatData._startPoint = startPoint;
        _curveStatData._endPoint = endPoint;
        _goalData = goalData;
        _curveStatData._middlePoint = middlePoint;
        _layerTargetStatData._layerTargetList.SetLayerList(gameObject, true, GameLayer.OutPlanetLayer);
        _stateMachine.ChangeState(SpacecraftState.Drive);
    }
    
    #region 죽을 때 호출되는 함수

    private void Die()
    {
        _health.HealthBar.StopAllCoroutines();
        _stateMachine.ChangeState(SpacecraftState.Die);
    }

    #endregion
    
    #region Set DestMark
    public void SetDestMark(GameObject destMark, Action<GameObject> returnDestMark)
    {
        _attackMark = destMark;
        OnReturnDestMark += returnDestMark;
    }
    public void SetDestMarkToCreature(CreatureController creature)
    {
        creature.SetDestMark(_attackMark, OnReturnDestMark);
    }
    #endregion
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameTags.Ground) && _isGravity)
        {
            if (!HasPassenger)
            {
                _createLoad.StartCreateLoad(() =>
                { 
                    Initialize();
                    _stateMachine.ChangeState(SpacecraftState.Idle);
                });
            }
            _collider.isTrigger = false;
            _rigidbody.isKinematic = true;
            _isGravity = false;
        }
    }
    
    public CreateLoad GetCreateLoad() => _createLoad;
}
