using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class CreatureSelectionState : State<CreatureState, CreatureFSM>
{
    private NavMeshStatData _navMeshStatData;

    public override CreatureState EState => CreatureState.Selection;
    public override void InitState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
    }
    public override void EnterState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        CreatureFSM creatureFSM = stateMachine.GetOwner();
        creatureFSM.SetEnableNavMeshAgent(_navMeshStatData);
        _navMeshStatData._navmeshAgentData._navMeshAgent.stoppingDistance = 0.5f;
    }
    public override void UpdateState(StateMachine<CreatureState, CreatureFSM> stateMachine) { }
    public override void ExitState(StateMachine<CreatureState, CreatureFSM> stateMachine) { }
}
