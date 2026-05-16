using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureDieState : State<CreatureState, CreatureController>
{
    private NavMeshStatData _navMeshStatData;
    private CreatureAnimatorStatData _animatorStatData;
    private DieStatData _dieStatData;
    public override CreatureState EState => CreatureState.Die;
    public override void InitState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
        stateMachine.TryGetStateData(out _dieStatData);
    }

    public override void EnterState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
        CreatureController creatureController = stateMachine.GetOwner();
        _navMeshStatData._navmeshAgentData._navMeshAgent.enabled = false;
        _navMeshStatData._navMeshObstacle.enabled = false;

        CreatureSelection.Instance.RemoveToSelectedCharacters(creatureController);
        UnitStorageManager.Instance.RemoveUnitToStorageList(Faction.Ally, UnitType.Creature, creatureController, _dieStatData._dieDelayTime, _animatorStatData);
        _bOnceEnter = true;
    }
    public override void UpdateState(StateMachine<CreatureState, CreatureController> stateMachine) { }
    public override void ExitState(StateMachine<CreatureState, CreatureController> stateMachine) { }
}
