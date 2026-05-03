using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

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

public class CreatureController : Unit, ISelectableOwner, IPassenger
{
    #region 상태 데이터 및 상태머신
    [Header("NavMesh 데이터")] [SerializeField]
    private NavMeshStatData _navMeshStatData;
    
    [Header("애니메이터 데이터")] [SerializeField] 
    private AnimatorStatData _animatorStatData;

    [Header("SurroundPos에 필요한 데이터")] [SerializeField]
    private SurroundPosStatData _surroundPosData;

    [Header("공격 발동 확률 데이터")] [SerializeField]
    private AttackActivationStatData _attackActivationStatData;

    [Header("Die 스탯 데이터")] [SerializeField]
    private DieStatData _dieStatData;

    private StateMachine<CreatureState, CreatureController> _stateMachine;
    public StateMachine<CreatureState, CreatureController> StateMachine => _stateMachine;
    #endregion

    #region Physics 데이터
    [Header("중력 세기")] [SerializeField] private float _gravity = -12f;
    private bool _isGravity = true;
    #endregion

    #region 체력 MaterialInstance
    [SerializeField] private HPMaterialInstance _hpMaterialInstance;
    #endregion

    #region TargetPos 데이터
    private Vector3 _enemyNexusPos;
    private Vector3 _destination;
    private Collider _enemyCollider;
    #endregion

    #region 스킬 데이터
    [Header("스킬 데이터 저장공간")] [SerializeField]
    private SkillData[] _skillDatas;
    #endregion

    #region 공격마크 데이터
    private event Action<GameObject> OnReturnAttackMark;
    private GameObject _attackMark;
    #endregion

    [SerializeField]
    private CharacterController _characterController;
    
    private bool _isAttackMode = false;
    private bool _isAttackTarget = false;
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
        _attackActivationStatData._dicAttackActivationRate.TryAdd(
            _animatorStatData._dicAnimParameterHash[AnimParameter.NormalAttack],
            _attackActivationStatData._normalAttackActivationRate);
        for (int i = 0; i < _skillDatas.Length; ++i)
        {
            _attackActivationStatData._dicAttackActivationRate.TryAdd(_skillDatas[i].GetSkillKeyToHash(),
                _skillDatas[i].ActivationRate);
        }
        #endregion
        _health.OnHealHit += HealHit;
        _health.OnDamageHit += DamageHit;
        _health.OnDie += Die;
    }

    private void Start()
    {
        SetUpUnit();
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

    public void SetAttackMark(GameObject attackMark, Action<GameObject> returnAttackmark)
    {
        _attackMark = attackMark;
        OnReturnAttackMark += returnAttackmark;
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
        int enemyCount = Physics.SphereCastNonAlloc(transform.position, radius, transform.up, _enemies, 0,
            GameLayer.EnemyTargetLayer);
        if (enemyCount > 0)
        {
            enemy = MinDistanceEnemy(_enemies, enemyCount);
            _enemyCollider = enemy.collider;
            return true;
        }

        enemy = default;
        return false;
    }

    #endregion

    #region 목적지에 이동 및 멈추는 함수

    public void MoveToDestination(out float currentWalkSpeed, NavMeshAgent navMeshAgent, Animator animator)
    {
        MoveToDestination(out currentWalkSpeed, navMeshAgent, animator, _destination);
    }

    public void MoveToDestination(out float currentWalkSpeed, NavMeshAgent navMeshAgent, Animator animator,
        Vector3 destination)
    {
        SetDestination(destination);
        navMeshAgent.destination = destination;
        navMeshAgent.speed = Status.DEX * 1.5f;
        currentWalkSpeed = navMeshAgent.desiredVelocity.magnitude;
        animator.SetFloat(_animatorStatData._dicAnimParameterHash[AnimParameter.WalkSpeed],
            currentWalkSpeed * _animatorStatData._animatorSpeedMultiplier);
        animator.SetBool(_animatorStatData._dicAnimParameterHash[AnimParameter.IsWalk], true);
    }

    public void StopToMove(NavMeshAgent navMeshAgent, Animator animator)
    {
        //TargetPosition = null;
        animator.SetBool(_animatorStatData._dicAnimParameterHash[AnimParameter.IsWalk], false);
        if (navMeshAgent.enabled)
        {
            navMeshAgent.ResetPath();
        }
    }

    #endregion

    public void ReleaseAttackMark()
    {
        OnReturnAttackMark?.Invoke(_attackMark);
        _attackMark = null;
        OnReturnAttackMark = null;
    }

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
        _health.HealthBar.StopAllCoroutines();
        _stateMachine.ChangeState(CreatureState.Die);
    }

    #endregion

    #region 확률에 의한 랜덤 공격 함수

    public IEnumerator IEAttackChoose(AnimatorStatData animatorStatData,
        AttackActivationStatData attackActivationStatData)
    {
        _isChoice = false;
        int animatorHash =
            attackActivationStatData._attackRandomProb.Choose<int>(attackActivationStatData._dicAttackActivationRate);
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

    #region 거리 및 방향 구하는 함수
    public float GetDistanceTo(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        return dir.sqrMagnitude;
    }
    
    
    public Vector3 GetLookDirection(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        return dir;
    }
    #endregion
    
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
        _enemyCollider = null;
        _isChoice = true;
        _stateMachine.ChangeState(CreatureState.Idle);
    }
    public void OnUnboard(Vector3 targetPosition, Vector3 enemyPosition)
    {
        SetEnemyNexusTargetPos(enemyPosition);
        SetDestination(targetPosition);
        SetIsAttackMode(true);
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

    public bool IsAttackMode => _isAttackMode;
    public bool IsAttackTarget => _isAttackTarget;
    public int GetAnimParameterHash(AnimParameter animParameter) =>
        _animatorStatData._dicAnimParameterHash[animParameter];
    public bool IsEnemyColliderExist => _enemyCollider;
    public Collider EnemyCollider => _enemyCollider;
    public Vector3 EnemyNexusPos => _enemyNexusPos;
    public bool IsAttackMarkExist => _attackMark;
    public void SetDestination(Vector3 targetPosition) => _destination = targetPosition;

    public float GetEnemyAttackDistance(NavMeshAgentStatData data) =>
        _enemyCollider ? _enemyCollider.bounds.extents.magnitude : -1f;
    public float GetNexusAttackDistance(NavMeshAgentStatData data)=> data._nexusAttackDistance;
    public void SetIsAttackMode(bool isAttackMode) => _isAttackMode = isAttackMode;
    public void SetIsAttackTarget(bool isAttackTarget) => _isAttackTarget = isAttackTarget;
    public Animator GetAnimator() => _animatorStatData._animator;
    public NavMeshAgent GetNavMeshAgent() => _navMeshStatData._navmeshAgentData._navMeshAgent;
    #endregion
}