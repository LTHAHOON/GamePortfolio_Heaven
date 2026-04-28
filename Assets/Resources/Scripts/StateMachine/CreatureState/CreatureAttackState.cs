using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureAttackState : State<CreatureState, Creature>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;
    private AttackActivationStatData _attackActivationStatData;
    public override CreatureState EState => CreatureState.Attack;
    public override void InitState(StateMachine<CreatureState, Creature> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
        stateMachine.TryGetStateData(out _attackActivationStatData);
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
            float attackDistance = creature.GetAttackDistance(navMeshAgentStatData);
            //적과 해당거리만큼 멀어졌을 때 Trace전환
            if (creature.GetDistanceFromThisToTarget() > (attackDistance * attackDistance))
            {
                stateMachine.ChangeState(CreatureState.Trace);
                return;
            }
            Quaternion newRotation;
            if (SurroundPosManager.IsContainTargetPos(creature.gameObject))
            {
                newRotation = Quaternion.LookRotation(creature.GetEnemyNexusDirection());
            }
            else
            {
                newRotation = Quaternion.LookRotation(creature.GetMoveDirection());
            }
            navMeshAgent.transform.rotation = Quaternion.Slerp(navMeshAgent.transform.rotation, newRotation, Time.deltaTime * 10f);
            if (creature._isChoice)
            {
                creature.StartCoroutine(creature.IEAttackChoose(_animatorStatData, _attackActivationStatData));
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e);
            stateMachine.ChangeState(CreatureState.Trace);
        }
    }
    public override void ExitState(StateMachine<CreatureState, Creature> stateMachine) { }
}
