using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class CreatureDeSelectionState : State<CreatureState, CreatureController>
{
    private NavMeshStatData _navMeshStatData;
    private CreatureAnimatorStatData _animatorStatData;

    public override CreatureState EState => CreatureState.DeSelection;
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
        CreatureController creature = stateMachine.GetOwner();
        NavMeshAgentStatData navMeshAgentStatData = _navMeshStatData._navmeshAgentData;
        NavMeshAgent navMeshAgent = navMeshAgentStatData._navMeshAgent;
        if (!navMeshAgent.enabled)
        {
            stateMachine.ChangeState(CreatureState.Idle);
            return;
        }
        creature.MoveToDestination(out float currenntWalkSpeed, navMeshAgent, _animatorStatData._animator);
        if (currenntWalkSpeed < 0.3f || navMeshAgentStatData.IsNavPathInValid)
        {
            creature.StopToMove(navMeshAgent, _animatorStatData._animator);
            CreatureCommandControl.RemoveTargetPos(creature);

            creature.ResetTargetAndState();
            creature.SetEnableNavMeshObstacle(_navMeshStatData, _animatorStatData);
            stateMachine.ChangeState(CreatureState.Idle);
        }
    }
    public override void ExitState(StateMachine<CreatureState, CreatureController> stateMachine) { }

}