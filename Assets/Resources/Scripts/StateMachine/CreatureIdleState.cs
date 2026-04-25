using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;


public class CreatureIdleState : State<CreatureState, CreatureFSM>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;
    private SurroundPosStatData _surroundPosData;
    private LayerStatData _layerStatData;

    public override CreatureState EState => CreatureState.Idle;
    public override void InitState(StateMachine<CreatureState, CreatureFSM> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
        stateMachine.TryGetStateData(out _surroundPosData);
        stateMachine.TryGetStateData(out _layerStatData);
    }

    public override void EnterState(StateMachine<CreatureState, CreatureFSM> stateMachine) 
    {
        CreatureFSM creatureFSM = stateMachine.GetOwner();
        creatureFSM.SetEnableNavMeshObstacle(_navMeshStatData, _animatorStatData);
    }
    public override void UpdateState(StateMachine<CreatureState, CreatureFSM> stateMachine) 
    {
        try
        {
            CreatureFSM creatureFSM = stateMachine.GetOwner();
            NavMeshAgentStatData navMeshAgentStatData = _navMeshStatData._navmeshAgentData;
            NavMeshAgent navMeshAgent = navMeshAgentStatData._navMeshAgent;
            _animatorStatData._animator.SetBool(_animatorStatData._dicAnimParameterHash[AnimParameter.IsWalk], false);
            Vector3 origin = creatureFSM.transform.position;
            origin.y += 30f;
            if (creatureFSM.TargetPosition != null && (creatureFSM.IsAttackMode || creatureFSM.IsAttackTarget))//사용자 위치로 이동해야할 경우
            {
                stateMachine.ChangeState(CreatureState.Trace);
            }
            else if (creatureFSM.TryGetAroundEnemy(out creatureFSM._enemy, _navMeshStatData._navmeshAgentData._traceRaidus, _layerStatData)) //주변 탐색(사용자 위치 도달 후)
            {
                stateMachine.ChangeState(CreatureState.Trace);
            }
            else if(creatureFSM.IsAttackMode || creatureFSM.IsAttackTarget) //상대 넥서스 위치 결정(사용자 위치 도달 후) 
            {
                if (!SurroundPosManager.IsContainTargetPos(creatureFSM.gameObject))
                {
                    navMeshAgent.stoppingDistance = 0.5f;
                    SurroundPosManager.AssignTargetPosition(creatureFSM.gameObject, creatureFSM._enemyNexusPos,
                        _surroundPosData._radiusFromCenter, _surroundPosData._distanceFromUnit, _surroundPosData._firstRingCount);
                }
                if (SurroundPosManager.TryGetAssignedTargetPositionAround(creatureFSM.gameObject, out Vector3 assigendPos))
                {
                    creatureFSM.TargetPosition = assigendPos;
                }
                stateMachine.ChangeState(CreatureState.Trace);
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }
    }
    public override void ExitState(StateMachine<CreatureState, CreatureFSM> stateMachine) { }
}

