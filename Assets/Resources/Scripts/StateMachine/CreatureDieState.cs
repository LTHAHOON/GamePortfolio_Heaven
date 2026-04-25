using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureDieState : State<CreatureState, CreatureFSM>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;
    private DieStatData _dieStatData;
    public override CreatureState EState => CreatureState.Die;
    public override void InitState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
        stateMachine.TryGetStateData(out _dieStatData);
    }

    public override void EnterState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        CreatureFSM creatureFSM = stateMachine.GetOwner();
        MyUnitPrefabDataControl.Instance.RemoveUnitPrefabToList(UnitType.Creature, creatureFSM);
        CreatureSelection.Instance.RemoveToSelectedCharacters(creatureFSM);
        _animatorStatData._animator.SetTrigger(_animatorStatData._dicAnimParameterHash[AnimParameter.Die]);
        Health health = creatureFSM.GetHealth();
        health.SetActiveHealthBar(false);
        creatureFSM.GetClickCollider().enabled = false;
        _navMeshStatData._navmeshAgentData._navMeshAgent.enabled = false;
        _navMeshStatData._navMeshObstacle.enabled = false;
        creatureFSM.StartCoroutine(creatureFSM.IEDie(_dieStatData._dieDelayTime));
        _bOnceEnter = true;
    }
    public override void UpdateState(StateMachine<CreatureState, CreatureFSM> stateMachine) { }
    public override void ExitState(StateMachine<CreatureState, CreatureFSM> stateMachine) { }
}
