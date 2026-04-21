using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(StatusComponent))]
public class CreatureFSM : Unit, ISelectable, IDragSelectable
{
    public enum CreatureState
    {
        Idle,
        Trace,
        Attack,
        Boarding,
        Die,
    }
    private enum AnimParameter
    {
        IsWalk,
        WalkSpeed,
        NormalAttack,
        GetHit,
        Die,
    }
    [Header("스킬 데이터 저장공간")]
    [SerializeField]
    private SkillData[] _skillDatas;
    [Header("이동 가능한 거리")]
    [SerializeField]
    private float _traceDistance;
    [Header("적 추적 가능한 사이즈")]
    [SerializeField]
    private float _traceRaidus;
    [Header("적 넥서스 공격 가능한 거리")]
    [SerializeField]
    private float _nexusAttackDistance;
    [Header("기본 공격 발동 확률")]
    [SerializeField]
    private float _normalAttackActivationRate = 80;
    [Header("사망 후 오브젝트 제거 딜레이 시간")]
    [SerializeField]
    private float _dieDelayTime = 4f;
    [SerializeField]
    private CharacterController _characterController;
    [SerializeField]
    private NavMeshObstacle _navMeshObstacle;
    [SerializeField]
    private NavMeshAgent _navMeshAgent;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Collider _clickCollider;
    [SerializeField]
    private HPMaterialInstance _hpMaterialInstance;

    [Header("상대 레이어")]
    [SerializeField]
    private LayerMask _enemyTargetLayer;
    [Header("애니메이션 속도 보완값")]
    [SerializeField]
    private float animatorSpeedMultiplier = 0.2f;
    [Header("생물체 속성")]
    [SerializeField]
    private LayerMask _property;
    [Header("중력 세기")]
    [SerializeField]
    private float _gravity = -12f;
    [SerializeField]
    private Health _health;
    [SerializeField]
    private StatusComponent _statusComponent;
    [Header("선택 여부")]
    public bool _isSelected = false;

    //AttackMark 반환 함수
    public event Action<GameObject> OnReturnAttackMark;
    [HideInInspector]
    public GameObject _attackMark;
    private Vector3 _enemyNexusPos;
    private RaycastHit _enemy = default;
    private bool _isMoveGravity = true;
    private bool _isAttackMode = false;
    private bool _isAttackTarget = false;
    private RuntimeUnitStatus _status;
    private CreatureState _creatureState = CreatureState.Idle;
    private RandomProb _attackRandomProb = new();
    bool _isChoice = true;

    private static Dictionary<int, float> _dicAttackActivationRate = new();
    private static readonly Dictionary<AnimParameter, int> _dicAnimParameterHash = new()
    {
        { AnimParameter.IsWalk, Animator.StringToHash(AnimParameter.IsWalk.ToString())},
        { AnimParameter.NormalAttack, Animator.StringToHash(AnimParameter.NormalAttack.ToString())},
        { AnimParameter.WalkSpeed, Animator.StringToHash(AnimParameter.WalkSpeed.ToString())},
        { AnimParameter.GetHit, Animator.StringToHash(AnimParameter.GetHit.ToString())},
        { AnimParameter.Die, Animator.StringToHash(AnimParameter.Die.ToString())}
    };

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

    private void Awake()
    {
        Debug.Log(GetInstanceID());
        _health.OnHealHit += HealHit;
        _health.OnDamageHit += DamageHit;
        _health.OnDie += Die;
        _dicAttackActivationRate.TryAdd(_dicAnimParameterHash[AnimParameter.NormalAttack], _normalAttackActivationRate);
        for (int i = 0; i < _skillDatas.Length; ++i)
        {
            _dicAttackActivationRate.TryAdd(_skillDatas[i].GetSkillKeyToHash(), _skillDatas[i].ActivationRate);
        }
        _status = _statusComponent.GetStatus();
        _navMeshAgent.enabled = true;
    }

    private void Update()
    {
        if (!CheckGround()) return;
        if (_creatureState == CreatureState.Die) return;
        UpdateFSM();
    }

    private void FixedUpdate()
    {
        if (_isMoveGravity)
        {
            GravityMove();
        }
    }

    public void OnSelected()
    {
        _hpMaterialInstance.GetCreatureHP().SetActive(true);
    }
    public void OnDeSelected()
    {
        _hpMaterialInstance.GetCreatureHP().SetActive(false);
    }
    public void OnDragSelected()
    {
        _hpMaterialInstance.GetCreatureHP().SetActive(true);
    }
    public void OnDragDeSelected()
    {
        _hpMaterialInstance.GetCreatureHP().SetActive(false);
    }

    public void SetCreatureState(CreatureState state)
    {
        _creatureState = state;
    }

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

    private RaycastHit MinDistanceEnemy(RaycastHit[] enemys, int enemyCount)
    {
        Dictionary<Vector3, float> dicDistances = new();
        if (enemys.Length < enemyCount)
        {
            return enemys[0];
        }

        for (int i = 0; i < enemyCount; ++i)
        {
            dicDistances.Add(enemys[i].transform.position, Vector3.Distance(transform.position, enemys[i].transform.position));
        }

        float distance = dicDistances.Values.Min();
        for (int i = 0; i < enemyCount; ++i)
        {
            if (dicDistances[enemys[i].transform.position] == distance)
            {
                return enemys[i];
            }
        }
        return enemys[0];
    }
    public float _distanceFromUnit = 7f;
    public float _radiusFromCenter = 8f;
    public int _firstRingCount = 15;
    private void Idle()
    {
        try
        {
            SetEnableNavMeshObstacle();
            _animator.SetBool(_dicAnimParameterHash[AnimParameter.IsWalk], false);
            Vector3 origin = transform.position;
            origin.y += 30f;
            if (TargetPosition != null)
            {
                _creatureState = CreatureState.Trace;//사용자가 지정한 위치로 이동
            }
            else if (TryGetAroundEnemy(out _enemy, _traceRaidus))
            {
                _navMeshAgent.stoppingDistance = 2f;
                SetCreatureState(CreatureState.Trace);
            }
            else
            {
                if (!SurroundPosManager.IsContainTargetPos(gameObject))
                {
                    _navMeshAgent.stoppingDistance = 0.5f;
                    SurroundPosManager.AssignTargetPosition(gameObject, _enemyNexusPos, _radiusFromCenter, _distanceFromUnit, _firstRingCount);
                    if (SurroundPosManager.TryGetAssignedTargetPositionAround(gameObject, out Vector3 assigendPos))
                    {
                        TargetPosition = assigendPos;
                    }
                    SetCreatureState(CreatureState.Trace);
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

    }

    private RaycastHit[] _enemys = new RaycastHit[25];
    private bool TryGetAroundEnemy(out RaycastHit enemy, float radius)
    {
        int enemyCount = Physics.SphereCastNonAlloc(transform.position, radius, transform.up, _enemys, 0, _enemyTargetLayer);
        if (enemyCount > 0)
        {
            enemy = MinDistanceEnemy(_enemys, enemyCount);
            return true;
        }
        enemy = default;
        return false;
    }


    private void Trace()
    {
        SetEnableNavMeshAgent();
        if (!_navMeshAgent.enabled)
        {
            return;
        }
        float distanceToTarget = GetDistanceFromThisToTarget();
        float attackDistance = GetAttackDistance();
        if (distanceToTarget <= (attackDistance * attackDistance) && !_navMeshAgent.pathPending
            && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.ResetPath();
            _isAttackTarget = false;
            HandleAttacktarget();
            return;
        }
        else if (distanceToTarget <= (_traceDistance * _traceDistance) || _isAttackTarget)
        {
            _navMeshAgent.destination = TargetPosition.Value;
            _navMeshAgent.speed = _status.DEX * 1.5f;
            float currentWalkSpeed = _navMeshAgent.desiredVelocity.magnitude;
            _animator.SetFloat(_dicAnimParameterHash[AnimParameter.WalkSpeed], currentWalkSpeed * animatorSpeedMultiplier);
            _animator.SetBool(_dicAnimParameterHash[AnimParameter.IsWalk], true);
        }
        if (SurroundPosManager.IsContainTargetPos(gameObject))
        {
            _navMeshAgent.destination = TargetPosition.Value;
            _navMeshAgent.speed = _status.DEX * 1.5f;
            float currentWalkSpeed = _navMeshAgent.desiredVelocity.magnitude;
            _animator.SetFloat(_dicAnimParameterHash[AnimParameter.WalkSpeed], currentWalkSpeed * animatorSpeedMultiplier);
            _animator.SetBool(_dicAnimParameterHash[AnimParameter.IsWalk], true);
            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                TargetPosition = null;
                SetCreatureState(CreatureState.Idle);
            }
        }

    }
    private void HandleAttacktarget()
    {
        if (_enemy.collider != null || SurroundPosManager.IsContainTargetPos(gameObject)) //사용자가 지정한 위치가 아닐경우
        {
            _animator.SetBool(_dicAnimParameterHash[AnimParameter.IsWalk], false);
            SetCreatureState(CreatureState.Attack);
        }
        else if (TryGetAroundEnemy(out _enemy, _traceRaidus)) //사용자가 지정한 위치에 도달 했을 때 적 추적
        {

            _isAttackTarget = false;
            _navMeshAgent.stoppingDistance = 2f;
        }
        else
        {
            if (_isAttackMode && _attackMark)
            {
                OnReturnAttackMark?.Invoke(_attackMark);
                OnReturnAttackMark = null;
            }

            _enemy = default;
            _navMeshAgent.stoppingDistance = 0.5f;
            _isAttackTarget = false;
            SurroundPosManager.AssignTargetPosition(gameObject, _enemyNexusPos, _radiusFromCenter, _distanceFromUnit, _firstRingCount);
            if (SurroundPosManager.TryGetAssignedTargetPositionAround(gameObject, out Vector3 assigendPos))
            {
                TargetPosition = assigendPos;
                Debug.Log(assigendPos);
            }
        }
    }

    public void DamageHit()
    {
        if (_hpMaterialInstance.isActiveAndEnabled && _health)
        {
            _hpMaterialInstance.ChangeHP(_health.CurrentHealth, _health.MaxHealth);
            _animator.SetTrigger(_dicAnimParameterHash[AnimParameter.GetHit]);
        }
    }
    public void HealHit()
    {
        if (_hpMaterialInstance.isActiveAndEnabled && _health)
        {
            _hpMaterialInstance.ChangeHP(_health.CurrentHealth, _health.MaxHealth);
        }
    }

    public void Die()
    {
        MyUnitPrefabDataControl.Instance.RemoveUnitPrefabToList(UnitType.Creature, this);
        CreatureSelection.RemoveToSelectedCharacters(this);
        _animator.SetTrigger(_dicAnimParameterHash[AnimParameter.Die]);
        _creatureState = CreatureState.Die;
        _hpMaterialInstance.gameObject.SetActive(false);
        _clickCollider.enabled = false;
        _navMeshAgent.enabled = false;
        _navMeshObstacle.enabled = false;
        _characterController.enabled = false;
        StartCoroutine(IEDie());
    }

    private void Attack()
    {
        SetEnableNavMeshObstacle();
        try
        {
            float attackDistance = GetAttackDistance();
            if (GetDistanceFromThisToTarget() > (attackDistance * attackDistance))
            {
                SetCreatureState(CreatureState.Trace);
            }
            Quaternion newRotation;
            if (SurroundPosManager.IsContainTargetPos(gameObject))
            {
                newRotation = Quaternion.LookRotation(GetEnemyNexusDirectgion());
            }
            else
            {
                newRotation = Quaternion.LookRotation(GetMoveDirection());
            }
            _navMeshAgent.transform.rotation = Quaternion.Slerp(_navMeshAgent.transform.rotation, newRotation, Time.deltaTime * 10f);
            if (_isChoice)
            {
                _isChoice = false;
                StartCoroutine(IEAttackChoose());
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e);
            SetCreatureState(CreatureState.Trace);
        }

    }

    private float GetDistanceFromThisToTarget()
    {
        Vector3 moveDirection;
        if (SurroundPosManager.IsContainTargetPos(gameObject))
        {
            moveDirection = GetEnemyNexusDirectgion();
        }
        else
        {
            moveDirection = GetMoveDirection();

        }


        moveDirection.y = 0;
        return moveDirection.sqrMagnitude;
    }

    private IEnumerator IEDie()
    {
        yield return new WaitForSeconds(_dieDelayTime);
        Destroy(gameObject);
    }

    private IEnumerator IEAttackChoose()
    {
        int animatorHash = _attackRandomProb.Choose<int>(_dicAttackActivationRate);
        _animator.ResetTrigger(animatorHash); //연속 Trigger 보완
        _animator.SetTrigger(animatorHash);
        var state = _animator.GetCurrentAnimatorStateInfo(1);
        yield return new WaitForSeconds(state.length);
        _isChoice = true;
    }

    private IEnumerator IEStopNavMeshAgent()
    {
        _navMeshObstacle.enabled = false;
        _navMeshObstacle.carving = false;

        //한 프레임 대기
        yield return null;

        _navMeshAgent.enabled = true;

    }

    private bool CheckGround()
    {
        if (_characterController.isGrounded)
        {
            _isMoveGravity = false;
        }
        return _characterController.isGrounded;
    }

    private void Boarding()
    {
        Debug.Log("우주선 탑승함수 실행");
        if(!TargetPosition.HasValue)
        {
            SetCreatureState(CreatureState.Idle);
            return;
        }

    }


    void UpdateFSM()
    {
        if (TargetPosition == null)
        {
            if (_isSelected)
            {
                SetEnableNavMeshAgent();
            }
            else
            {
                SetEnableNavMeshObstacle();
            }
        }
        if ((_isSelected == false && _isAttackMode) || _isAttackTarget)
        {

            switch (_creatureState)
            {
                case CreatureState.Idle:
                    Idle();
                    break;
                case CreatureState.Trace:
                    Trace();
                    break;
                case CreatureState.Attack:
                    Attack();
                    break;
                case CreatureState.Boarding:
                    Boarding();
                    break;
            }
        }
        else
        {
            _navMeshAgent.stoppingDistance = 0.5f;
            ResetTargetAndState();
        }
    }

    private void ResetTargetAndState()
    {
        _enemy = default;
        TargetPosition = null;
        _isChoice = true;
        _creatureState = CreatureState.Idle;
        SurroundPosManager.ReleaseTargetPosition(gameObject);
        if (_hpMaterialInstance.GetCreatureHP().activeSelf == false && _navMeshAgent.desiredVelocity == Vector3.zero)
        {
            if (_isSelected)
            {
                _isSelected = false;
                CreatureControl.RemoveOldCreature(this);
            }

        }
    }

    private void SetEnableNavMeshAgent()
    {
        if (!_navMeshAgent.enabled)
        {
            StartCoroutine(IEStopNavMeshAgent());
        }
    }
    private void SetEnableNavMeshObstacle()
    {
        if (!_navMeshObstacle.enabled)
        {
            _animator.SetBool(_dicAnimParameterHash[AnimParameter.IsWalk], false);
            _navMeshAgent.enabled = false;
            _navMeshObstacle.enabled = true;
            _navMeshObstacle.carving = true;
        }
    }

    private float GetAttackDistance() => (_enemy.collider != null) ? _enemy.collider.bounds.extents.magnitude : _nexusAttackDistance;
    private Vector3 GetEnemyNexusDirectgion() => (_enemyNexusPos - transform.position);
    private Vector3 GetMoveDirection() => (TargetPosition.Value - transform.position);
    public GameObject GetCreatureHP() => _hpMaterialInstance.GetCreatureHP();
    public void SetIsAttackMode(bool isAttackMode) => _isAttackMode = isAttackMode;
    public void SetIsAttackTarget(bool isAttackTarget) => _isAttackTarget = isAttackTarget;
    public RuntimeUnitStatus GetStatus() => _status;
    public void SetStatus(RuntimeUnitStatus status) => _status = status;
    public long GetID() => _statusComponent.GetUnitData().ID;
    public Animator GetAnimator() => _animator;
    public NavMeshAgent GetNavMeshAgent() => _navMeshAgent;
}
