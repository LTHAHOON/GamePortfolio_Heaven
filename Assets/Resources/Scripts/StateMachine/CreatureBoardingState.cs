using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureBoardingState : State<CreatureState, CreatureFSM>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;

    public override CreatureState EState => CreatureState.Boarding;
    public override void InitState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
    }

    public override void EnterState(StateMachine<CreatureState, CreatureFSM> stateMachine) { }
    public override void UpdateState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        CreatureFSM creatureFSM = stateMachine.GetOwner();
        NavMeshAgentStatData navMeshAgentStatData = _navMeshStatData._navmeshAgentData;
        NavMeshAgent navMeshAgent = navMeshAgentStatData._navMeshAgent;
        if (!creatureFSM.TargetPosition.HasValue)
        {
            stateMachine.ChangeState(CreatureState.Idle);
            return;
        }
        creatureFSM.MoveToDestination(out float currentWalkSpeed, navMeshAgent, _animatorStatData._animator, creatureFSM.TargetPosition);

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance * 3)
        {
            Health health = creatureFSM.GetHealth();
            ObjectVisbilitySystem.RemoveToList(health.GetHealthBar());
            health.SetActiveHealthBar(false);
            creatureFSM.gameObject.SetActive(false);
            stateMachine.ChangeState(CreatureState.Idle);
        }
    }
    public override void ExitState(StateMachine<CreatureState, CreatureFSM> stateMachine) { }
}
