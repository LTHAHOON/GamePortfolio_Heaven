using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureAttackState : State<CreatureState, CreatureController>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;
    private AttackActivationStatData _attackActivationStatData;
    public override CreatureState EState => CreatureState.Attack;

    public override void InitState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
        stateMachine.TryGetStateData(out _attackActivationStatData);
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
        if (creature.IsAttackMode || creature.IsAttackTarget)
        {
            Vector3 lookDirection;
            float attackDistance;
            float distanceToTarget;
            if (creature.IsEnemyColliderExist)
            {
                attackDistance = creature.GetEnemyAttackDistance(navMeshAgentStatData);
                lookDirection = creature.GetLookDirection(creature.EnemyCollider.transform.position);
                distanceToTarget = creature.GetDistanceTo(creature.EnemyCollider.transform.position);
            }
            else
            {
                attackDistance = creature.GetNexusAttackDistance(navMeshAgentStatData);
                lookDirection = creature.GetLookDirection(creature.EnemyNexusPos);
                distanceToTarget = creature.GetDistanceTo(creature.EnemyNexusPos);
            }
            
            if (distanceToTarget > (attackDistance * attackDistance))
            {
                stateMachine.ChangeState(CreatureState.Trace);
                return;
            }

            Quaternion newRotation = Quaternion.LookRotation(lookDirection);
            navMeshAgent.transform.rotation =
                Quaternion.Slerp(navMeshAgent.transform.rotation, newRotation, Time.deltaTime * 10f);
            if (creature._isChoice)
            {
                creature.StartCoroutine(creature.IEAttackChoose(_animatorStatData, _attackActivationStatData));
            }
        }
        else
        {
            stateMachine.ChangeState(CreatureState.Idle);
        }
    }

    public override void ExitState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
    }
}