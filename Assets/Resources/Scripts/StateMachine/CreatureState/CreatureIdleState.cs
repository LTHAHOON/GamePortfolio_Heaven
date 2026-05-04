using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;


public class CreatureIdleState : State<CreatureState, CreatureController>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;
    private SurroundPosStatData _surroundPosData;

    public override CreatureState EState => CreatureState.Idle;

    public override void InitState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
        stateMachine.TryGetStateData(out _surroundPosData);
    }

    public override void EnterState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
        CreatureController creatureController = stateMachine.GetOwner();
        creatureController.SetEnableNavMeshObstacle(_navMeshStatData, _animatorStatData);
    }

    public override void UpdateState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
        CreatureController creatureController = stateMachine.GetOwner();
        NavMeshAgentStatData navMeshAgentStatData = _navMeshStatData._navmeshAgentData;
        NavMeshAgent navMeshAgent = navMeshAgentStatData._navMeshAgent;
        _animatorStatData._animator.SetBool(_animatorStatData._dicAnimParameterHash[AnimParameter.IsWalk], false);
        Vector3 origin = creatureController.transform.position;
        origin.y += 30f;
        if (creatureController.IsAttackMode || creatureController.IsAttackTarget) //����� ��ġ�� �̵��ؾ��� ���
        {
            //AttackMode 처음 들어갈때(AttackMark가 있을 경우)
            if (SurroundPosManager.IsContainTargetPos(creatureController.gameObject, _surroundPosData._surroundPosGroup) && creatureController.IsAttackMarkExist)
            {
                stateMachine.ChangeState(CreatureState.Trace);
            }
            //넥서스 타겟 결정하기 전 주변 Enemy 체크
            else if (creatureController.TryGetAroundEnemy(out RaycastHit enemy,
                         _navMeshStatData._navmeshAgentData._traceRaidus)) //�ֺ� Ž��(����� ��ġ ���� ��)
            {
                creatureController.SetDestination(enemy.transform.position);
                stateMachine.ChangeState(CreatureState.Trace);
            }
            //넥서스 타겟 위치 할당
            else if(!SurroundPosManager.IsContainTargetPos(creatureController.gameObject, _surroundPosData._surroundPosGroup))
            {
                navMeshAgent.stoppingDistance = 0.5f;
                _surroundPosData._surroundPosGroup = NexusManager.Instance.GetNexusSurroundPosGroup(Fraction.Enemy);
                SurroundPosManager.AssignTargetPosition(creatureController.gameObject,_surroundPosData._surroundPosGroup);
                if (SurroundPosManager.TryGetAssignedTargetPositionAround(creatureController.gameObject, _surroundPosData._surroundPosGroup ,out Vector3 assigendPos))
                {
                    creatureController.SetDestination(assigendPos);
                }
                stateMachine.ChangeState(CreatureState.Trace);
            }
        }

    }

    public override void ExitState(StateMachine<CreatureState, CreatureController> stateMachine) { }
}