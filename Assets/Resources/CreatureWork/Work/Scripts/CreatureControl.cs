using System;
using System.Collections.Generic;
using TMPro;
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

    private void OnDestroy()
    {
        _dicTargetPosition.Clear();
    }

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


    public static void RemoveTargetPos(CreatureFSM creatureFSM)
    {
        _dicTargetPosition.Remove(creatureFSM);
    }

    private void SetTarget()
    {
        if (Input.GetMouseButtonDown(1) && CreatureSelection.Instance.GetSelectionCharactersCount() > 0)
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
            List<CreatureFSM> selectedCreatures = CreatureSelection.Instance.GetSelectionComponents<CreatureFSM>();
            float[] distancesArray = SurroundPosManager.DistanceArrayByCharacterCount(selectedCreatures.Count, _distanceFromUnit, _radiusFromCenter, _firstRingCount);
            int[] positionCountArray = SurroundPosManager.GetPositionCountArray(selectedCreatures.Count, _firstRingCount);
            Vector3[] targetPositions = SurroundPosManager.GetTargetPositionsAround(_targetPosition.Value, distancesArray, positionCountArray);

            for (int i = 0; i < selectedCreatures.Count; i++)
            {
                if (SurroundPosManager.IsPositionWalkable(targetPositions[i], _obstacleCheckRadius, _obstacleLayerMask))
                {
                    NavMeshAgent navMeshAgent = selectedCreatures[i].GetNavMeshAgent();
                    Animator animator = selectedCreatures[i].GetAnimator();
                    SelectedCreatureMoveStop(selectedCreatures[i], navMeshAgent, animator);
                    _dicTargetPosition[selectedCreatures[i]] = targetPositions[i];
                }
            }
        }
        if (_dicTargetPosition.Count <= 0)
        {
            _isMoving = false;
        }
    }




    private static readonly Dictionary<CreatureFSM, Vector3> _dicTargetPosition = new();
    private void SelectedCreatureMoveTo()
    {
        List<CreatureFSM> selectedCreatures = CreatureSelection.Instance.GetSelectionComponents<CreatureFSM>();
        if (selectedCreatures == null) return;
        for (int i = 0; i < selectedCreatures.Count; i++)
        {
            CreatureFSM selectedCreature = selectedCreatures[i];

            if (_dicTargetPosition.TryGetValue(selectedCreature, out Vector3 targetPosition))
            {
                NavMeshAgent selectedNavMeshAgent = selectedCreature.GetNavMeshAgent();
                Animator selectedAnimator = selectedCreature.GetAnimator();
                if (!selectedNavMeshAgent.enabled || !selectedAnimator || !selectedNavMeshAgent) continue;
                selectedCreature.MoveToDestination(out float currentWalkSpeed, selectedNavMeshAgent, selectedAnimator, targetPosition);
                if (selectedNavMeshAgent.pathPending) continue;
                if (currentWalkSpeed < 0.3f && selectedNavMeshAgent.remainingDistance <= selectedNavMeshAgent.stoppingDistance)
                {
                    RemoveTargetPos(selectedCreature);
                    SelectedCreatureMoveStop(selectedCreature, selectedNavMeshAgent, selectedAnimator);
                    return;
                }

            }
        }
    }

    private void SelectedCreatureMoveStop(CreatureFSM selectedCreature, NavMeshAgent navMeshAgent, Animator animator)
    {
        selectedCreature.StopToMove(navMeshAgent, animator);
    }

    private float _rotateSpeed = 10f;
    private void SelectedCreatureLookAt()
    {
        List<NavMeshAgent> navMeshAgents = CreatureSelection.Instance.GetSelectionComponents<NavMeshAgent>();
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
