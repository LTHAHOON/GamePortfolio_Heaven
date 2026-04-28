using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum CreatureState
{
    Selection,
    DeSelection,
    Idle,
    Trace,
    Attack,
    Boarding,
    Die,
}

[RequireComponent(typeof(StatusComponent))]
public class Creature : Unit, ISelectableOwner
{
    #region 상태 데이터 및 상태머신
    [Header("NavMesh 데이터")]
    [SerializeField]
    private NavMeshStatData _navMeshStatData;
    [Header("애니메이터 데이터")]
    [SerializeField]
    private AnimatorStatData _animatorStatData;
    [Header("SurroundPos에 필요한 데이터")]
    [SerializeField]
    private SurroundPosStatData _surroundPosData;
    [Header("공격 발동 확률 데이터")]
    [SerializeField]
    private AttackActivationStatData _attackActivationStatData;
    [Header("Die 스탯 데이터")]
    [SerializeField]
    private DieStatData _dieStatData;

    private StateMachine<CreatureState, Creature> _stateMachine;
    public StateMachine<CreatureState, Creature> StateMachine => _stateMachine;
    #endregion
    #region Physics 데이터

    [Header("중력 세기")]
    [SerializeField]
    private float _gravity = -12f;
    private bool _isGravity = true;
    #endregion
    #region 체력 MaterialInstance
    [SerializeField]
    private HPMaterialInstance _hpMaterialInstance;
    #endregion
    #region 스테이터스 컴포넌트
    [SerializeField]
    private StatusComponent _statusComponent;
    private RuntimeUnitStatus _status;
    #endregion
    #region TargetPos 데이터
    [HideInInspector]
    public RaycastHit _enemy = default;
    [HideInInspector]
    public Vector3 _enemyNexusPos;
    private Vector3? _targetPosition;
    public Vector3? TargetPosition
    {
        get
        {
            if (_enemy.collider != null)
            {
                return _enemy.transform.position;
            }
            return _targetPosition;
        }
        set { _targetPosition = value; }
    }
    #endregion
    #region 스킬 데이터
    [Header("스킬 데이터 저장공간")]
    [SerializeField]
    private SkillData[] _skillDatas;
    #endregion
    #region 공격마크 데이터
    public event Action<GameObject> OnReturnAttackMark;
    [HideInInspector]
    public GameObject _attackMark;
    #endregion
    [SerializeField]
    private CharacterController _characterController;
    private bool _isAttackMode = false;
    public bool IsAttackMode => _isAttackMode;
    private bool _isAttackTarget = false;
    public bool IsAttackTarget => _isAttackTarget;
    [HideInInspector]
    public bool _isChoice = true;
    public MonoBehaviour Owner => this;
    #region 유니티 이벤트 함수
    protected override void Awake()
    {
        base.Awake();
        #region StateMachine 초기화
        _stateMachine = new(this, new IStateData[] 
        {
            _navMeshStatData,
            _animatorStatData,
            _surroundPosData,
            _attackActivationStatData,
            _dieStatData,
        });
        #endregion
        #region State 추가
        _stateMachine.AddState(new CreatureIdleState());
        _stateMachine.AddState(new CreatureTraceState());
        _stateMachine.AddState(new CreatureAttackState());
        _stateMachine.AddState(new CreatureBoardingState());
        _stateMachine.AddState(new CreatureDieState());
        _stateMachine.AddState(new CreatureSelectionState());
        _stateMachine.AddState(new CreatureDeSelectionState());
        _stateMachine.ChangeState(CreatureState.Idle);
        #endregion
        #region 공격 발동 확률 데이터 초기화
        _attackActivationStatData._dicAttackActivationRate.TryAdd(_animatorStatData._dicAnimParameterHash[AnimParameter.NormalAttack],
                                                                                    _attackActivationStatData._normalAttackActivationRate);
        for (int i = 0; i < _skillDatas.Length; ++i)
        {
            _attackActivationStatData._dicAttackActivationRate.TryAdd(_skillDatas[i].GetSkillKeyToHash(), _skillDatas[i].ActivationRate);
        }
        #endregion
        _health.OnHealHit += HealHit;
        _health.OnDamageHit += DamageHit;
        _health.OnDie += Die;
        _status = _statusComponent.GetStatus();
    }
    private void Update()
    {
        UpdateState();
    }
    private void FixedUpdate()
    {
        if (_isGravity)
        {
            GravityMove();
        }
    }
    #endregion

    //생물체 FSM
    private void UpdateState()
    {
        if (!CheckGround()) return;
        _stateMachine.UpdateCurrentState();
    }

    #region 주변 적 구하는 함수
    //가까운 적 구하는 함수
    private RaycastHit MinDistanceEnemy(RaycastHit[] enemies, int enemyCount)
    {
        int index = 0;
        float minDistance = float.MaxValue;
        Vector3 myPos = transform.position;

        for (int i = 0; i < enemyCount; ++i)
        {
            float dist = Vector3.SqrMagnitude(enemies[i].transform.position - myPos);
            if (dist < minDistance)
            {
                minDistance = dist;
                index = i;
            }
        }
        return enemies[index];
    }

    //SphereCast에 의한 가까운 적 구하는 함수
    private readonly RaycastHit[] _enemies = new RaycastHit[25];
    public bool TryGetAroundEnemy(out RaycastHit enemy, float radius)
    {
        int enemyCount = Physics.SphereCastNonAlloc(transform.position, radius, transform.up, _enemies, 0, GameLayer.EnemyTargetLayer);
        if (enemyCount > 0)
        {
            enemy = MinDistanceEnemy(_enemies, enemyCount);
            return true;
        }
        enemy = default;
        return false;
    }
    #endregion

    #region 목적지에 이동 및 멈추는 함수
    public void MoveToDestination(out float currentWalkSpeed ,NavMeshAgent navMeshAgent, Animator animator, Vector3? targetPosition)
    {
        if(TargetPosition != targetPosition)
        {
            TargetPosition = targetPosition;
        }
        navMeshAgent.destination = TargetPosition.Value;
        navMeshAgent.speed = _status.DEX * 1.5f;
        currentWalkSpeed = navMeshAgent.desiredVelocity.magnitude;
        animator.SetFloat(_animatorStatData._dicAnimParameterHash[AnimParameter.WalkSpeed], currentWalkSpeed * _animatorStatData._animatorSpeedMultiplier);
        animator.SetBool(_animatorStatData._dicAnimParameterHash[AnimParameter.IsWalk], true);
    }
    public void StopToMove(NavMeshAgent navMeshAgent, Animator animator)
    {
        //TargetPosition = null;
        animator.SetBool(_animatorStatData._dicAnimParameterHash[AnimParameter.IsWalk], false);
        if(navMeshAgent.enabled)
        {
            navMeshAgent.ResetPath();
        }
    }
    #endregion

    #region Attack상태에 들아가기 전 한번 더 추적하는 함수
    public bool HandleAttacktarget(NavMeshAgentStatData navMeshAgentData, AnimatorStatData animatorStatData, 
                                                    SurroundPosStatData surroundPosData)
    {
        //둘러쌓은 위치를 가졌는데 적 타겟을 가지고 있을 경우
        if (_enemy.collider != null || SurroundPosManager.IsContainTargetPos(gameObject))
        {
            animatorStatData._animator.SetBool(animatorStatData._dicAnimParameterHash[AnimParameter.IsWalk], false);
            return true;
        }
        //사용자가 지정한 위치에 도달했을 때 적 추적을 실패할 경우 적 넥서스 타겟 지정
        else if (!TryGetAroundEnemy(out _enemy, navMeshAgentData._traceRaidus))
        {
            if (_isAttackMode && _attackMark)
            {
                OnReturnAttackMark?.Invoke(_attackMark);
                OnReturnAttackMark = null;
            }

            _enemy = default;
            navMeshAgentData._navMeshAgent.stoppingDistance = 0.5f;
            SurroundPosManager.AssignTargetPosition(gameObject, _enemyNexusPos, surroundPosData._radiusFromCenter,
                surroundPosData._distanceFromUnit, surroundPosData._firstRingCount);
            if (SurroundPosManager.TryGetAssignedTargetPositionAround(gameObject, out Vector3 assigendPos))
            {
                TargetPosition = assigendPos;
                Debug.Log(assigendPos);
            }

        }
        return false;
    }
    #endregion

    #region 데미지 및 힐될 때 호출되는 함수
    public void DamageHit()
    {
        if (_hpMaterialInstance.isActiveAndEnabled && _health && _health.HealthBar)
        {
            StartCoroutine(_health.HealthBar.IEShowUI());
            _hpMaterialInstance.ChangeHP(_health.CurrentHealth, _health.MaxHealth);
            _animatorStatData._animator.SetTrigger(_animatorStatData._dicAnimParameterHash[AnimParameter.GetHit]);
        }
        else
        {
            _animatorStatData._animator.ResetTrigger(_animatorStatData._dicAnimParameterHash[AnimParameter.GetHit]);
        }
    }
    public void HealHit()
    {
        if (_hpMaterialInstance.isActiveAndEnabled && _health && _health.HealthBar)
        {
            StartCoroutine(_health.HealthBar.IEShowUI());
            _hpMaterialInstance.ChangeHP(_health.CurrentHealth, _health.MaxHealth);
        }
    }
    #endregion

    #region 죽을 때 호출되는 함수
    public void Die()
    {
        _health.HealthBar.

        StopAllCoroutines();
        _stateMachine.ChangeState(CreatureState.Die);
    }


    #endregion

    #region 확률에 의한 랜덤 공격 함수
    public IEnumerator IEAttackChoose(AnimatorStatData animatorStatData, AttackActivationStatData attackActivationStatData)
    {
        _isChoice = false;
        int animatorHash = attackActivationStatData._attackRandomProb.Choose<int>(attackActivationStatData._dicAttackActivationRate);
        animatorStatData._animator.ResetTrigger(animatorHash); //연속 Trigger 보완
        animatorStatData._animator.SetTrigger(animatorHash);
        var state = animatorStatData._animator.GetCurrentAnimatorStateInfo(1);
        yield return new WaitForSeconds(state.length);
        _isChoice = true;
    }
    #endregion

    private void GravityMove()
    {
        Vector3 _gravityDirection = Vector3.up;
        Vector3 _gravityMotion = _gravityDirection * _gravity * Time.fixedDeltaTime;
        _characterController.Move(_gravityMotion);
    }
    public void SetEnemyNexusTargetPos(Vector3 nexusTargetPos)
    {
        _enemyNexusPos = nexusTargetPos;
    }

    //타겟과의 거리를 구하는 함수
    public float GetDistanceFromThisToTarget()
    {
        Vector3 moveDirection;
        if (SurroundPosManager.IsContainTargetPos(gameObject))
        {
            moveDirection = GetEnemyNexusDirection();
        }
        else
        {
            moveDirection = GetMoveDirection();

        }
        moveDirection.y = 0;
        return moveDirection.sqrMagnitude;
    }

    //땅에 있는지 체크
    private bool CheckGround()
    {
        if (_characterController.isGrounded)
        {
            _isGravity = false;
        }
        return _characterController.isGrounded;
    }

    //타겟과 상태를 초기화(리셋)하는 함수
    public void ResetTargetAndState()
    {
        _enemy = default;
        TargetPosition = null;
        _isChoice = true;
        _stateMachine.ChangeState(CreatureState.Idle);
    }

    #region NavMeshAgent, NavMeshObstacle Enable 컨트롤
    private IEnumerator IEStopNavMeshAgent(NavMeshStatData navMeshStatData)
    {
        navMeshStatData._navMeshObstacle.enabled = false;
        navMeshStatData._navMeshObstacle.carving = false;
        //한 프레임 대기
        yield return null;
        navMeshStatData._navmeshAgentData._navMeshAgent.enabled = true;
    }

    public void SetEnableNavMeshAgent(NavMeshStatData navMeshStatData)
    {
        if (!navMeshStatData._navmeshAgentData._navMeshAgent.enabled)
        {
            StartCoroutine(IEStopNavMeshAgent(navMeshStatData));
        }
    }

    public void SetEnableNavMeshObstacle(NavMeshStatData navMeshStatData, AnimatorStatData animatorStatData)
    {
        if (!navMeshStatData._navMeshObstacle.enabled)
        {
            animatorStatData._animator.SetBool(animatorStatData._dicAnimParameterHash[AnimParameter.IsWalk], false);
            navMeshStatData._navmeshAgentData._navMeshAgent.enabled = false;
            navMeshStatData._navMeshObstacle.enabled = true;
            navMeshStatData._navMeshObstacle.carving = true;
        }
    }
    #endregion

    #region 선택 및 선택해제될 때 호출되는 함수
    public void OnSelected()
    {
        _hpMaterialInstance.GetCreatureHP().SetActive(true);
        TargetPosition = null;
        _stateMachine.ChangeState(CreatureState.Selection);
    }
    public void OnDeSelected()
    {
        _hpMaterialInstance.GetCreatureHP().SetActive(false);
        if (_stateMachine.CurrentState.EState == CreatureState.Boarding)
            return;
        _stateMachine.ChangeState(CreatureState.DeSelection);
    }
    public void OnDragSelected()
    {
        OnSelected();
    }
    public void OnDragDeSelected()
    {
        OnDeSelected();
    }
    #endregion

    #region 데이터 반환 함수
    public int GetAnimParameterHash(AnimParameter animParameter) => _animatorStatData._dicAnimParameterHash[animParameter];

    public float GetAttackDistance(NavMeshAgentStatData data) => (_enemy.collider != null) ? _enemy.collider.bounds.extents.magnitude : data._nexusAttackDistance;
    public Vector3 GetEnemyNexusDirection() => (_enemyNexusPos - transform.position);
    public Vector3 GetMoveDirection() => (TargetPosition.Value - transform.position);

    public GameObject GetCreatureHP() => _hpMaterialInstance.GetCreatureHP();
    public void SetIsAttackMode(bool isAttackMode) => _isAttackMode = isAttackMode;
    public void SetIsAttackTarget(bool isAttackTarget) => _isAttackTarget = isAttackTarget;
    public RuntimeUnitStatus GetStatus() => _status;
    public void SetStatus(RuntimeUnitStatus status) => _status = status;
    public long GetID() => _statusComponent.GetUnitData().ID;
    public Animator GetAnimator() => _animatorStatData._animator;
    public NavMeshAgent GetNavMeshAgent() => _navMeshStatData._navmeshAgentData._navMeshAgent;
    #endregion
}

