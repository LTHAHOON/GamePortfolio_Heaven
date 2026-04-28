using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class CreatureTraceState : State<CreatureState, Creature>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;
    private SurroundPosStatData _surroundPosData;

    public override CreatureState EState => CreatureState.Trace;
    public override void InitState(StateMachine<CreatureState, Creature> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
        stateMachine.TryGetStateData(out _surroundPosData);
    }
    public override void EnterState(StateMachine<CreatureState, Creature> stateMachine)
    {
        Creature creature = stateMachine.GetOwner();
        creature.SetEnableNavMeshAgent(_navMeshStatData);
    }
    public override void UpdateState(StateMachine<CreatureState, Creature> stateMachine)
    {
        Creature creature = stateMachine.GetOwner();
        NavMeshAgentStatData navMeshAgentStatData = _navMeshStatData._navmeshAgentData;
        NavMeshAgent navMeshAgent = navMeshAgentStatData._navMeshAgent;
        if (!navMeshAgent.enabled)
        {
            return;
        }
        float distanceToTarget = creature.GetDistanceFromThisToTarget();
        //둘러쌓은 위치를 가지고 있을 경우 해당 위치로 이동합니다.
        if (SurroundPosManager.IsContainTargetPos(creature.gameObject))
        {
            creature.MoveToDestination(out float currentWalkSpeed, navMeshAgent, _animatorStatData._animator, creature.TargetPosition);
        }
        //추적거리안에 있을 시 타겟위치로 이동합니다.
        else if (distanceToTarget <= (navMeshAgentStatData._traceDistance * navMeshAgentStatData._traceDistance) || creature.IsAttackTarget)
        {
            creature.MoveToDestination(out float currentWalkSpeed, navMeshAgent, _animatorStatData._animator, creature.TargetPosition);
        }
        //추적거리 범위보다 멀어졌을 경우 Idle전환
        else
        {
            creature.TargetPosition = null;
            stateMachine.ChangeState(CreatureState.Idle);
        }

        float attackDistance = creature.GetAttackDistance(navMeshAgentStatData);
        if (navMeshAgent.pathPending) return;
        //공격거리안에 있을 시 HandleAttackTarget함수로 확실히 Attack할 지 한번 더 추적합니다
        if (distanceToTarget <= (attackDistance * attackDistance)
            && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            navMeshAgent.ResetPath();
            creature.SetIsAttackTarget(false);
            bool bAttack = creature.HandleAttacktarget(navMeshAgentStatData, _animatorStatData, _surroundPosData);
            if (bAttack)
            {
                stateMachine.ChangeState(CreatureState.Attack);
            }
        }
    }
    public override void ExitState(StateMachine<CreatureState, Creature> stateMachine) { }
}
