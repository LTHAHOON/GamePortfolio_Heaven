using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class CreatureTraceState : State<CreatureState, CreatureController>
{
    private NavMeshStatData _navMeshStatData;
    private AnimatorStatData _animatorStatData;
    private SurroundPosStatData _surroundPosData;

    public override CreatureState EState => CreatureState.Trace;

    public override void InitState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _animatorStatData);
        stateMachine.TryGetStateData(out _surroundPosData);
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
        if (!navMeshAgent.enabled) return;

        if (creature.IsAttackMode || creature.IsAttackTarget)
        {
            
            creature.MoveToDestination(out float currentWalkSpeed, navMeshAgent, _animatorStatData._animator);
            if (navMeshAgent.pathPending) return;
          
            //AttackMark가 존재하지 않을 경우(즉, 넥서스 타겟 또는 Enemy 타겟 이라는 뜻)
            if (!creature.IsAttackMarkExist)
            {
                float attackDistance = 0f;
                float distanceToTarget = 0f;
                //Enemy 타겟일 경우
                if (creature.IsEnemyColliderExist)
                {
                    attackDistance = creature.GetEnemyAttackDistance(navMeshAgentStatData);
                    distanceToTarget = creature.GetDistanceTo(creature.EnemyCollider.transform.position);
                    if (distanceToTarget > (navMeshAgentStatData._traceDistance * navMeshAgentStatData._traceDistance))
                    {
                        stateMachine.ChangeState(CreatureState.Idle);
                    }
                }
                //이동중 주변 Enemy 탐색
                else if (creature.TryGetAroundEnemy(out RaycastHit enemy,
                             _navMeshStatData._navmeshAgentData._traceRaidus))
                {
                    creature.SetDestination(enemy.transform.position);
                }
                //Nexus 타겟일 경우
                else
                {
                    attackDistance = creature.GetNexusAttackDistance(navMeshAgentStatData);
                    Vector3 enemyNexusPos = NexusManager.Instance.GetNexusPosByFraction(Fraction.Enemy);
                    distanceToTarget = creature.GetDistanceTo(enemyNexusPos);
                }                

                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance||
                    navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    if (navMeshAgent.hasPath && currentWalkSpeed > 0)
                        return;
                    //목적지 도달 후 AttackDistance 체크
                    if (distanceToTarget <= (attackDistance * attackDistance))
                    {
                        navMeshAgent.ResetPath();
                        creature.SetIsAttackTarget(false);
                        stateMachine.ChangeState(CreatureState.Attack);
                    }
                    //AttackDistance보다 멀면 Idle
                    else
                    {
                        stateMachine.ChangeState(CreatureState.Idle);
                    }
                }
            }
            //AttackMark가 존재할 경우(AttackDistance를 사용하지 않음)
            else
            {
                if (currentWalkSpeed <= 0 && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance||
                    navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    creature.ReleaseAttackMark();
                    SurroundPosManager.ReleaseTargetPosition(creature.gameObject,_surroundPosData._surroundPosGroup);
                    stateMachine.ChangeState(CreatureState.Idle);
                }
            }
        }
        else
        {
            stateMachine.ChangeState(CreatureState.Idle);
        }
    }

    public override void ExitState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
    }
}