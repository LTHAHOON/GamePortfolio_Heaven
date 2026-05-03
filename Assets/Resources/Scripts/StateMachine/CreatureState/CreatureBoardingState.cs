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
        CreatureController creatureController = stateMachine.GetOwner();
        creatureController.SetEnableNavMeshAgent(_navMeshStatData);
    }
    public override void UpdateState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
        CreatureController creatureController = stateMachine.GetOwner();
        NavMeshAgentStatData navMeshAgentStatData = _navMeshStatData._navmeshAgentData;
        NavMeshAgent navMeshAgent = navMeshAgentStatData._navMeshAgent;
        creatureController.MoveToDestination(out float currentWalkSpeed, navMeshAgent, _animatorStatData._animator);

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance * 3)
        {
            Health health = creatureController.GetHealth();
            health.HealthBar.SetForceHideUI(true);
            creatureController.gameObject.SetActive(false);
            //stateMachine.ChangeState(CreatureState.DeSelection);
        }
    }
    public override void ExitState(StateMachine<CreatureState, CreatureController> stateMachine) { }
}
