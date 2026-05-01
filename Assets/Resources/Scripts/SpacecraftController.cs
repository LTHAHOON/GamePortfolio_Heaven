using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static UnityEngine.UI.GridLayoutGroup;
using Random = UnityEngine.Random;
public struct Goal
{
    public Vector3 _spacecraftGoalPos;
    public Vector3 _passengerGoalPos;
    public Vector3 _enemytNexusPos;
    public RespawnPositionType _respawnPositionType;
}
public enum SpacecraftState
{
    Idle,
    Landing,
    Drive,
    GetOff,
    Boarding,
}

[RequireComponent(typeof(StatusComponent))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class SpacecraftController : PassengerController, ISelectableOwner
{
    #region ���� ������ �� ���¸ӽ�
    private BezierCurveStatData _curveStatData = new();
    [Header("���� ž�¿� �ʿ��� ������")]
    [SerializeField]
    private BoardingStatData _boardingStatData;
    [Header("SurroundPos�� �ʿ��� ������")]
    [SerializeField]
    private SurroundPosStatData _surroundPosStatData;
    [Header("���̾� Ÿ�� ������")]
    [SerializeField]
    private LayerTargetStatData _layerTargetStatData;
    [Header("Die ���� ������")]
    [SerializeField]
    private DieStatData _dieStatData;
    private StateMachine<SpacecraftState, SpacecraftController> _stateMachine;
    public StateMachine<SpacecraftState, SpacecraftController> StateMachine => _stateMachine;
    #endregion
    #region Physics ������
    [SerializeField]
    private float gravity = -9.81f;
    [HideInInspector]
    public bool _isGravity = false;
    [HideInInspector]
    public Vector3 colliderSizeData;
    private Rigidbody _rigidbody;
    private BoxCollider _collider;
    #endregion
    #region AttackMark ������
    public event Action<GameObject> OnReturnAttackMark;
    private GameObject _attackMark;
    #endregion

    [SerializeField]
    private CreateLoad _createLoad;
    private Goal _goalData;
    private int _unitTypeLayer;
    private Transform _passengerParent;
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
        });
        _stateMachine.AddState(new SpacecraftIdleState());
        _stateMachine.AddState(new SpacecraftBoardingState());
        _stateMachine.AddState(new SpacecraftDriveState());
        _stateMachine.AddState(new SpacecraftLandingState());
        _stateMachine.AddState(new SpacecraftGetOffState());

        #endregion
        _clickCollider.enabled = false;
        _unitTypeLayer = LayerMask.NameToLayer(UnitType.Spacecraft.ToString());
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
        colliderSizeData = _collider.size;
        _collider.isTrigger = true;
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

    public void Initialize()
    {
        SetUp();
        _clickCollider.enabled = true;
        _health.SetActiveHealthBar(true);
        MyUnitPrefabDataControl.Instance.AddUnitPrefabToList(UnitType, this);
        TransparentMaterialControl.SetQpaqueOrTransparentControl(gameObject, UnitType, TransparentMaterialControl.SurfaceType.Opaque, new Color32(255, 255, 255, 255));
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

    public void SetGoal(Vector3 startPoint, Vector3 endPoint, Vector3 middlePoint, Goal goalData, Vector3 enemyNexusPos)
    {
        Initialize();
        _curveStatData._startPoint = startPoint;
        _curveStatData._endPoint = endPoint;
        _goalData = goalData;
        _goalData._spacecraftGoalPos.y = 5f;
        _goalData._enemytNexusPos = enemyNexusPos;
        _curveStatData._middlePoint = middlePoint;
        _layerTargetStatData._layerTargetList.SetLayerList(gameObject, true, GameLayer.OutPlanetLayer);
        _stateMachine.ChangeState(SpacecraftState.Drive);
    }

    public void AddPassenger(Creature creature, int passengerCount, Transform parent)
    {
        _passengerParent = parent;
        AddPassengerInData(creature, passengerCount);
    }

    #region AttackMark ����
    public void SetAttackMark(GameObject attckMark, Action<GameObject> returnAttackmark)
    {
        _attackMark = attckMark;
        OnReturnAttackMark += returnAttackmark;
    }
    public void SetAttackMarkToCreature(Creature creature)
    {
        creature._attackMark = _attackMark;
        creature.OnReturnAttackMark += OnReturnAttackMark;
    }
    #endregion

    public int GetPassengerCount(long id)
    {
        return GetPassengerCountInData(id);
    }

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
            _stateMachine.ChangeState(SpacecraftState.GetOff);
            ClearPassengerDatas();
        }
    }

    public Transform PassengerParent => _passengerParent;
    public int UnitTypeLayer => _unitTypeLayer;
    public Goal GoalData => _goalData;
    public CreateLoad GetCreateLoad() => _createLoad;
}
