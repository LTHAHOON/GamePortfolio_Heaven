using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;


public class CreatureIdleState : State<CreatureState, Creature>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;
    private SurroundPosStatData _surroundPosData;

    public override CreatureState EState => CreatureState.Idle;
    public override void InitState(StateMachine<CreatureState, Creature> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
        stateMachine.TryGetStateData(out _surroundPosData);
    }

    public override void EnterState(StateMachine<CreatureState, Creature> stateMachine) 
    {
        Creature creature = stateMachine.GetOwner();
        creature.SetEnableNavMeshObstacle(_navMeshStatData, _animatorStatData);
    }
    public override void UpdateState(StateMachine<CreatureState, Creature> stateMachine) 
    {
        try
        {
            Creature creature = stateMachine.GetOwner();
            NavMeshAgentStatData navMeshAgentStatData = _navMeshStatData._navmeshAgentData;
            NavMeshAgent navMeshAgent = navMeshAgentStatData._navMeshAgent;
            _animatorStatData._animator.SetBool(_animatorStatData._dicAnimParameterHash[AnimParameter.IsWalk], false);
            Vector3 origin = creature.transform.position;
            origin.y += 30f;
            if (creature.TargetPosition != null && (creature.IsAttackMode || creature.IsAttackTarget))//사용자 위치로 이동해야할 경우
            {
                stateMachine.ChangeState(CreatureState.Trace);
            }
            else if (creature.TryGetAroundEnemy(out creature._enemy, _navMeshStatData._navmeshAgentData._traceRaidus)) //주변 탐색(사용자 위치 도달 후)
            {
                stateMachine.ChangeState(CreatureState.Trace);
            }
            else if(creature.IsAttackMode || creature.IsAttackTarget) //상대 넥서스 위치 결정(사용자 위치 도달 후) 
            {
                if (!SurroundPosManager.IsContainTargetPos(creature.gameObject))
                {
                    navMeshAgent.stoppingDistance = 0.5f;
                    SurroundPosManager.AssignTargetPosition(creature.gameObject, creature._enemyNexusPos,
                        _surroundPosData._radiusFromCenter, _surroundPosData._distanceFromUnit, _surroundPosData._firstRingCount);
                }
                if (SurroundPosManager.TryGetAssignedTargetPositionAround(creature.gameObject, out Vector3 assigendPos))
                {
                    creature.TargetPosition = assigendPos;
                }
                stateMachine.ChangeState(CreatureState.Trace);
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }
    }
    public override void ExitState(StateMachine<CreatureState, Creature> stateMachine) { }
}

