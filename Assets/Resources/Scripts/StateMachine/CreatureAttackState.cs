using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureAttackState : State<CreatureState, CreatureFSM>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;
    private AttackActivationStatData _attackActivationStatData;
    public override CreatureState EState => CreatureState.Attack;
    public override void InitState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
        stateMachine.TryGetStateData(out _attackActivationStatData);

    }

    public override void EnterState(StateMachine<CreatureState, CreatureFSM> stateMachine) { }
    public override void UpdateState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        try
        {
            CreatureFSM creatureFSM = stateMachine.GetOwner();
            NavMeshAgentStatData navMeshAgentStatData = _navMeshStatData._navmeshAgentData;
            NavMeshAgent navMeshAgent = navMeshAgentStatData._navMeshAgent;
            creatureFSM.SetEnableNavMeshObstacle(_navMeshStatData, _animatorStatData);
            float attackDistance = creatureFSM.GetAttackDistance(navMeshAgentStatData);
            if (creatureFSM.GetDistanceFromThisToTarget() > (attackDistance * attackDistance))
            {
                stateMachine.ChangeState(CreatureState.Trace);
            }
            Quaternion newRotation;
            if (SurroundPosManager.IsContainTargetPos(creatureFSM.gameObject))
            {
                newRotation = Quaternion.LookRotation(creatureFSM.GetEnemyNexusDirection());
            }
            else
            {
                newRotation = Quaternion.LookRotation(creatureFSM.GetMoveDirection());
            }
            navMeshAgent.transform.rotation = Quaternion.Slerp(navMeshAgent.transform.rotation, newRotation, Time.deltaTime * 10f);
            if (creatureFSM._isChoice)
            {
                creatureFSM.StartCoroutine(creatureFSM.IEAttackChoose(_animatorStatData, _attackActivationStatData));
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e);
            stateMachine.ChangeState(CreatureState.Trace);
        }
    }
    public override void ExitState(StateMachine<CreatureState, CreatureFSM> stateMachine) { }
}
