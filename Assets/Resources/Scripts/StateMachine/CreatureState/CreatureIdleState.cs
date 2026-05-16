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
    private CreatureAnimatorStatData _animatorStatData;
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
        CreatureController creature = stateMachine.GetOwner();
        NavMeshAgentStatData navMeshAgentStatData = _navMeshStatData._navmeshAgentData;
        NavMeshAgent navMeshAgent = navMeshAgentStatData._navMeshAgent;
        _animatorStatData._animator.SetBool(_animatorStatData._dicAnimParameterHash[CreatureAnimParameter.IsWalk], false);
        Vector3 origin = creature.transform.position;
        origin.y += 30f;
        if (creature.IsCustomTarget)
        {

        }

        else if ((SurroundPosManager.IsContainTargetPos(creature.gameObject, _surroundPosData._surroundPosGroup) && creature.IsDestMarkExist))
        {
            creature.GetClickCollider().enabled = true;
            stateMachine.ChangeState(CreatureState.Trace);
        }
        //넥서스 타겟 결정하기 전 주변 Enemy 체크
        else if (creature.TryGetAroundEnemy(out RaycastHit enemy,
                     _navMeshStatData._navmeshAgentData._traceRaidus)) //�ֺ� Ž��(����� ��ġ ���� ��)
        {
            SurroundPosManager.ReleaseTargetPosition(creature.gameObject, _surroundPosData._surroundPosGroup);
            creature.SetDestination(enemy.transform.position);
            stateMachine.ChangeState(CreatureState.Trace);
        }
        //넥서스 타겟 위치 할당
        else if (!SurroundPosManager.IsContainTargetPos(creature.gameObject, _surroundPosData._surroundPosGroup) && creature.CurrentModeType == ModeType.AttackMode)
        {
            navMeshAgent.stoppingDistance = 0.5f;
            _surroundPosData._surroundPosGroup = NexusManager.Instance.GetNexusSurroundPosGroup(Faction.Enemy);
            SurroundPosManager.AssignTargetPosition(creature.gameObject, _surroundPosData._surroundPosGroup);
            if (SurroundPosManager.TryGetAssignedTargetPositionAround(creature.gameObject, _surroundPosData._surroundPosGroup, out Vector3 assigendPos))
            {
                creature.SetDestination(assigendPos);
            }
            stateMachine.ChangeState(CreatureState.Trace);
        }

    }

    public override void ExitState(StateMachine<CreatureState, CreatureController> stateMachine) { }
}