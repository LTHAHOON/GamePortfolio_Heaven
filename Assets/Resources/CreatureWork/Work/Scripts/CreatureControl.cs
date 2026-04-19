using System;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class CreatureControl : MonoBehaviour
{
    [SerializeField]
    private CreatureSelection _creatureSelection;
    [SerializeField]
    private LayerMask _groundLayerMask;
    [SerializeField]
    private LayerMask _obstacleLayerMask;
    [Header("장애물 판단하는 CheckSphere의 최대 거리")]
    [SerializeField]
    private float _obstacleCheckRadius = 0.2f;
    [SerializeField]
    private GameObject _moveMarkPrefab;
    [SerializeField]
    private Transform _mapMarkParent;

    private Vector3? _targetPosition;
    private PoolComponent _pcMoveMark;
    public float _distanceFromUnit = 5f;
    public float _radiusFromCenter = 5f;
    public int _firstRingCount = 10;
    public static bool _isMoving = false;
    public static bool _isSelect = false;


    private void Start()
    {
        PoolManager.Instance.AddPool(_moveMarkPrefab, 3, 5, _mapMarkParent);
        _pcMoveMark = PoolManager.Instance.GetPool(_moveMarkPrefab);
    }
    private void Update()
    {
        SetTarget();
        if (_isMoving)
        {
            SelectedCreatureLookAt();
            SelectedCreatureMoveTo();
        }
    }


    public static void RemoveOldCreature(CreatureFSM oldCreatureFSM)
    {
        _dicTargetPosition.Remove(oldCreatureFSM);
    }

    private void SetTarget()
    {
        if (Input.GetMouseButtonDown(1) && CreatureSelection.GetSelectionCharactersCount() > 0)
        {
            _targetPosition = InputManager.Instance.GetWolrdMousePosByRaycast(Camera.main, _groundLayerMask).Value;
            if (_targetPosition.HasValue)
            {
                GameObject poolPrefab = _pcMoveMark.PopPoolObject();
                poolPrefab.transform.position = _targetPosition.Value;
                _pcMoveMark.ReturnPoolObject(poolPrefab, 1.5f);
                _isMoving = true;
            }
            else
            {
                _isMoving = false;
            }
            List<CreatureFSM> selectedCreatures = CreatureSelection.GetSelectionCharacters<CreatureFSM>();
            float[] distancesArray = SurroundPosManager.DistanceArrayByCharacterCount(selectedCreatures.Count, _distanceFromUnit, _radiusFromCenter, _firstRingCount);
            int[] positionCountArray = SurroundPosManager.GetPositionCountArray(selectedCreatures.Count, _firstRingCount);
            Vector3[] targetPositions = SurroundPosManager.GetTargetPositionsAround(_targetPosition.Value, distancesArray, positionCountArray);

            for (int i = 0; i < selectedCreatures.Count; i++)
            {
                if (SurroundPosManager.IsPositionWalkable(targetPositions[i], _obstacleCheckRadius, _obstacleLayerMask))
                {
                    NavMeshAgent navMeshAgent = selectedCreatures[i].GetNavMeshAgent();
                    Animator animator = selectedCreatures[i].GetAnimator();
                    SelectedCreatureMoveStop(navMeshAgent, animator);
                    _dicTargetPosition[selectedCreatures[i]] = targetPositions[i];
                }
            }
        }
        if (_dicTargetPosition.Count <= 0)
        {
            _isMoving = false;
        }
    }




    private static Dictionary<CreatureFSM, Vector3> _dicTargetPosition = new();
    private const float animatorSpeedMultiplier = 0.2f;
    private void SelectedCreatureMoveTo()
    {
        List<CreatureFSM> selectedCreatures = CreatureSelection.GetSelectionCharacters<CreatureFSM>();
        if (selectedCreatures == null) return;
        for (int i = 0; i < selectedCreatures.Count; i++)
        {
            CreatureFSM selectedCreature = selectedCreatures[i];
            if (_dicTargetPosition.TryGetValue(selectedCreature, out Vector3 targetPosition))
            {
                Vector3 moveDirection = targetPosition - selectedCreature.transform.position;
                moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
                float distanceToTarget = moveDirection.sqrMagnitude;
                NavMeshAgent selectedNavMeshAgent = selectedCreature.GetNavMeshAgent();
                Animator selectedAnimator = selectedCreature.GetAnimator();

                if (distanceToTarget > (selectedNavMeshAgent.stoppingDistance * selectedNavMeshAgent.stoppingDistance))
                {
                    if (selectedNavMeshAgent.hasPath == false && selectedNavMeshAgent.enabled)
                    {
                        selectedNavMeshAgent.destination = _dicTargetPosition[selectedCreature];
                    }

                    selectedNavMeshAgent.speed = selectedCreature.GetStatus().DEX * 1.5f;

                    float currentWalkSpeed = selectedNavMeshAgent.desiredVelocity.magnitude;
                    if (currentWalkSpeed < selectedNavMeshAgent.speed)
                    {
                        selectedAnimator.SetFloat("WalkSpeed", currentWalkSpeed * animatorSpeedMultiplier);
                    }
                    selectedAnimator.SetBool("IsWalk", true);
                }
                else
                {
                    SelectedCreatureMoveStop(selectedNavMeshAgent, selectedAnimator);


                }
            }
        }
    }

    private void SelectedCreatureMoveStop(NavMeshAgent navMeshAgent, Animator animator)
    {
        animator.SetBool("IsWalk", false);
        if (navMeshAgent.enabled)
        {
            navMeshAgent.avoidancePriority = 50;
            navMeshAgent.stoppingDistance = 2f;
            navMeshAgent.ResetPath();
        }
    }

    private float _rotateSpeed = 10f;
    private void SelectedCreatureLookAt()
    {
        List<NavMeshAgent> navMeshAgents = CreatureSelection.GetSelectionCharacters<NavMeshAgent>();
        if (navMeshAgents == null) return;
        for (int i = 0; i < navMeshAgents.Count; ++i)
        {
            if (navMeshAgents[i].desiredVelocity.normalized != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(navMeshAgents[i].desiredVelocity.normalized);
                navMeshAgents[i].transform.rotation = Quaternion.Slerp(navMeshAgents[i].transform.rotation, newRotation, Time.deltaTime * _rotateSpeed);
            }
        }
    }
}
