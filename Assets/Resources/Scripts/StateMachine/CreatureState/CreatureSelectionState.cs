using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class CreatureSelectionState : State<CreatureState, CreatureController>
{
    private NavMeshStatData _navMeshStatData;
    private SurroundPosStatData _surroundPosData;
    public override CreatureState EState => CreatureState.Selection;
    public override void InitState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
        stateMachine.TryGetStateData(out _navMeshStatData);
        stateMachine.TryGetStateData(out _surroundPosData);
    }
    public override void EnterState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
        CreatureController creature = stateMachine.GetOwner();
        creature.SetEnableNavMeshAgent(_navMeshStatData);
        _navMeshStatData._navmeshAgentData._navMeshAgent.stoppingDistance = 0.5f;
        if (creature.IsDestMarkExist)
        {
            creature.ReleaseDestMark();
            SurroundPosManager.ReleaseTargetPosition(creature.gameObject, _surroundPosData._surroundPosGroup);
            SurroundPosManager.ReleaseSurroundPosGroup(_surroundPosData._surroundPosGroup);
        }
    }

    public override void UpdateState(StateMachine<CreatureState, CreatureController> stateMachine)
    {
        CreatureController creature = stateMachine.GetOwner();
        float attackDistance;
        float distanceToTarget;
        if (creature.IsEnemyColliderExist)
        {
            attackDistance = creature.GetEnemyAttackDistance(_navMeshStatData._navmeshAgentData);
            distanceToTarget = creature.GetDistanceTo(creature.EnemyCollider.transform.position);
        }
        //Nexus 타겟일 경우
        else
        {
            attackDistance = creature.GetNexusAttackDistance(_navMeshStatData._navmeshAgentData);
            Vector3 enemyNexusPos = NexusManager.Instance.GetNexusPosByFraction(Faction.Enemy);
            distanceToTarget = creature.GetDistanceTo(enemyNexusPos);
        }

        if (distanceToTarget > (attackDistance * attackDistance))
        {
            SurroundPosManager.ReleaseTargetPosition(creature.gameObject, _surroundPosData._surroundPosGroup);
        }
    }
    public override void ExitState(StateMachine<CreatureState, CreatureController> stateMachine) { }
}
