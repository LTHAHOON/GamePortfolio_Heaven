using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class CreatureDeSelectionState : State<CreatureState, CreatureFSM>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;

    public override CreatureState EState => CreatureState.DeSelection;
    public override void InitState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
    }
    public override void EnterState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        CreatureFSM creatureFSM = stateMachine.GetOwner();
        creatureFSM.SetEnableNavMeshAgent(_navMeshStatData);
    }
    public override void UpdateState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        CreatureFSM creatureFSM = stateMachine.GetOwner();
        NavMeshAgentStatData navMeshAgentStatData = _navMeshStatData._navmeshAgentData;
        NavMeshAgent navMeshAgent = navMeshAgentStatData._navMeshAgent;
        if(!creatureFSM.TargetPosition.HasValue)
        {
            stateMachine.ChangeState(CreatureState.Idle);
            return;
        }

        creatureFSM.MoveToDestination(out float currenntWalkSpeed, navMeshAgent, _animatorStatData._animator, creatureFSM.TargetPosition);
        if (currenntWalkSpeed < 0.3f || navMeshAgentStatData.IsNavPathInValid)
        {
            creatureFSM.StopToMove(navMeshAgent, _animatorStatData._animator);
            CreatureControl.RemoveTargetPos(creatureFSM);
            SurroundPosManager.ReleaseTargetPosition(creatureFSM.gameObject);
            if(!creatureFSM.IsAttackMode)
            {
                creatureFSM.ResetTargetAndState();
            }
            creatureFSM.SetEnableNavMeshObstacle(_navMeshStatData, _animatorStatData);
            stateMachine.ChangeState(CreatureState.Idle);
        }
    }
    public override void ExitState(StateMachine<CreatureState, CreatureFSM> stateMachine) { }
    
}