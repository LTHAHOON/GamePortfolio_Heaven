using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureBoardingState : State<CreatureState, Creature>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;

    public override CreatureState EState => CreatureState.Boarding;
    public override void InitState(StateMachine<CreatureState, Creature> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
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
        if (!creature.TargetPosition.HasValue)
        {
            stateMachine.ChangeState(CreatureState.DeSelection);
            return;
        }
        creature.MoveToDestination(out float currentWalkSpeed, navMeshAgent, _animatorStatData._animator, creature.TargetPosition);

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance * 3)
        {
            Health health = creature.GetHealth();
            health.HealthBar.SetForceHideUI(true);
            creature.gameObject.SetActive(false);
            //stateMachine.ChangeState(CreatureState.DeSelection);
        }
    }
    public override void ExitState(StateMachine<CreatureState, Creature> stateMachine) { }
}
