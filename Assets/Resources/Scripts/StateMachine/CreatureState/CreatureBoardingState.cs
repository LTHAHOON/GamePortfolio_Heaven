using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureBoardingState : State<CreatureState, CreatureController>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;

    public override CreatureState EState => CreatureState.Boarding;
    public override void InitState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
    }

    public override void EnterState(StateMachine<CreatureState, CreatureController> stateMachine) 
    {
        CreatureController creature = stateMachine.GetOwner();
        creature.SetEnableNavMeshAgent(_navMeshStatData);
    }
    public override void UpdateState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
        CreatureController creature = stateMachine.GetOwner();
        NavMeshAgentStatData navMeshAgentStatData = _navMeshStatData._navmeshAgentData;
        NavMeshAgent navMeshAgent = navMeshAgentStatData._navMeshAgent;
        creature.MoveToDestination(out float currentWalkSpeed, navMeshAgent, _animatorStatData._animator);

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance * 3)
        {
            Health health = creature.GetHealth();
            health.HealthBar.SetForceHideUI(true);
            creature.GetClickCollider().enabled = false;
            creature.OnBoard();
            creature.StateMachine.ChangeState(CreatureState.Idle);
        }
    }
    public override void ExitState(StateMachine<CreatureState, CreatureController> stateMachine) { }
}
