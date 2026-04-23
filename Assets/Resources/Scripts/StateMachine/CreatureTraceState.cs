using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class CreatureTraceState : State<CreatureState, CreatureFSM>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;
    private SurroundPosStatData _surroundPosData;
    private LayerStatData _layerStatData;

    public override CreatureState EState => CreatureState.Trace;
    public override void EnterState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
        stateMachine.TryGetStateData(out _surroundPosData);
        stateMachine.TryGetStateData(out _layerStatData);
        _bOnceEnter = true;
    }
    public override void UpdateState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        CreatureFSM creatureFSM = stateMachine.GetOwner();
        NavMeshAgentStatData navMeshAgentStatData = _navMeshStatData._navmeshAgentData;
        NavMeshAgent navMeshAgent = navMeshAgentStatData._navMeshAgent;
        creatureFSM.SetEnableNavMeshAgent(_navMeshStatData);
        if (!navMeshAgent.enabled)
        {
            return;
        }
        float distanceToTarget = creatureFSM.GetDistanceFromThisToTarget();
        float attackDistance = creatureFSM.GetAttackDistance(navMeshAgentStatData);
        if (distanceToTarget <= (attackDistance * attackDistance) && !navMeshAgent.pathPending
            && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            navMeshAgent.ResetPath();
            creatureFSM._isAttackTarget = false;
            creatureFSM.HandleAttacktarget(navMeshAgentStatData, _animatorStatData, _surroundPosData, _layerStatData);
            return;
        }
        else if (distanceToTarget <= (navMeshAgentStatData._traceDistance * navMeshAgentStatData._traceDistance) || creatureFSM._isAttackTarget)
        {
            creatureFSM.MoveToDestination(out float currentWalkSpeed, navMeshAgent, _animatorStatData._animator, creatureFSM.TargetPosition);
        }
        if (SurroundPosManager.IsContainTargetPos(creatureFSM.gameObject))
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                creatureFSM.TargetPosition = null;
                stateMachine.ChangeState(CreatureState.Idle);
            }
            else
            {
                creatureFSM.MoveToDestination(out float currentWalkSpeed, navMeshAgent, _animatorStatData._animator, creatureFSM.TargetPosition);
            }
        }

    }
    public override void ExitState(StateMachine<CreatureState, CreatureFSM> stateMachine) { }
}
