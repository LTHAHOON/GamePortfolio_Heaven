using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class CreatureDeSelectionState : State<CreatureState, Creature>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;

    public override CreatureState EState => CreatureState.DeSelection;
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
        if(!creature.TargetPosition.HasValue)
        {
            stateMachine.ChangeState(CreatureState.Idle);
            return;
        }

        creature.MoveToDestination(out float currenntWalkSpeed, navMeshAgent, _animatorStatData._animator, creature.TargetPosition);
        if (currenntWalkSpeed < 0.3f || navMeshAgentStatData.IsNavPathInValid)
        {
            creature.StopToMove(navMeshAgent, _animatorStatData._animator);
            CreatureControl.RemoveTargetPos(creature);
            SurroundPosManager.ReleaseTargetPosition(creature.gameObject);
            if(!creature.IsAttackMode)
            {
                creature.ResetTargetAndState();
            }
            creature.SetEnableNavMeshObstacle(_navMeshStatData, _animatorStatData);
            stateMachine.ChangeState(CreatureState.Idle);
        }
    }
    public override void ExitState(StateMachine<CreatureState, Creature> stateMachine) { }
    
}