using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class CreatureSelectionState : State<CreatureState, Creature>
{
    private NavMeshStatData _navMeshStatData;

    public override CreatureState EState => CreatureState.Selection;
    public override void InitState(StateMachine<CreatureState, Creature> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
    }
    public override void EnterState(StateMachine<CreatureState, Creature> stateMachine)
    {
        Creature creature = stateMachine.GetOwner();
        creature.SetEnableNavMeshAgent(_navMeshStatData);
        _navMeshStatData._navmeshAgentData._navMeshAgent.stoppingDistance = 0.5f;
    }
    public override void UpdateState(StateMachine<CreatureState, Creature> stateMachine) { }
    public override void ExitState(StateMachine<CreatureState, Creature> stateMachine) { }
}
