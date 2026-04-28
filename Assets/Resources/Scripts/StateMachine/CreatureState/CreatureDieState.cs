using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureDieState : State<CreatureState, Creature>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;
    private DieStatData _dieStatData;
    public override CreatureState EState => CreatureState.Die;
    public override void InitState(StateMachine<CreatureState, Creature> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
        stateMachine.TryGetStateData(out _dieStatData);
    }

    public override void EnterState(StateMachine<CreatureState, Creature> stateMachine)
    {
        Creature creature = stateMachine.GetOwner();
        _navMeshStatData._navmeshAgentData._navMeshAgent.enabled = false;
        _navMeshStatData._navMeshObstacle.enabled = false;

        CreatureSelection.Instance.RemoveToSelectedCharacters(creature);
        MyUnitPrefabDataControl.Instance.RemoveUnitPrefabToList(UnitType.Creature, creature, _dieStatData._dieDelayTime, _animatorStatData);
        _bOnceEnter = true;
    }
    public override void UpdateState(StateMachine<CreatureState, Creature> stateMachine) { }
    public override void ExitState(StateMachine<CreatureState, Creature> stateMachine) { }
}
