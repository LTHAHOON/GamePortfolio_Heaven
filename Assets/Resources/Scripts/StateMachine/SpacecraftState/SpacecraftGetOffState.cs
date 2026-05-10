using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public class SpacecraftGetOffState : State<SpacecraftState, SpacecraftController>
{
    private NavMeshAgentStatData _navMeshAgentStatData;
    private SurroundPosStatData _surroundPosData;
    public override SpacecraftState EState => SpacecraftState.GetOff;

    public override void InitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        stateMachine.TryGetStateData(out _surroundPosData);
    }

    public override void EnterState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        SpacecraftController owner = stateMachine.GetOwner();
        List<PassengerData> unSpawnedList = owner.GetUnSpawnedPassengers();
        List<Unit> spawnedList = owner.GetSpawnedPassengers();
        #region Spawn���� ���� Passenger�� ���� ���
        if (unSpawnedList.Count > 0)
        {
            SurroundPosGroup initalGroup = null;
            for (int i = 0; i < unSpawnedList.Count; i++)
            {
                if (unSpawnedList[i].Passenger is not CreatureController creaturePrefab)
                    continue;
                for (int j = 0; j < unSpawnedList[i].PassengerCount; j++)
                {
                    Vector3 searchPos = owner.transform.position;
                    searchPos.y += 1f;
                    if (NavMesh.SamplePosition(searchPos, out NavMeshHit hit, 25f, NavMesh.AllAreas))
                    {
                        CreatureController creature = UnitSpawnManager.Instance.Spawn(creaturePrefab);
                        if (i == 0 && j == 0)
                        {
                            creature.SetSurroundPosGroup(SurroundPosManager.AssignCenterTargetPosition(creature.gameObject, owner.GoalData._passengerGoalPos,
                                _surroundPosData._radiusFromCenter, _surroundPosData._distanceFromUnit, _surroundPosData._firstRingCount, true));
                            initalGroup = creature.GetSurroundPosGroup();
                        }
                        else
                        {
                            creature.SetSurroundPosGroup(initalGroup);
                            SurroundPosManager.AssignTargetPosition(creature.gameObject, initalGroup);
                        }
                        SurroundPosManager.TryGetAssignedTargetPositionAround(creature.gameObject, initalGroup, out Vector3 assigendPos);
                        creature.transform.position = hit.position;
                        creature.SetModeType(owner.CurrentModeType);
                        creature.OnUnboard(assigendPos);
                        owner.SetDestMarkToCreature(creature);
                    }
                }
            }
        }
        #endregion
        #region Spawn�� Passenger�� ���� ���
        if (spawnedList.Count > 0)
        {
            SurroundPosGroup initalGroup = null;
            for (int i = 0; i < spawnedList.Count; i++)
            {
                if (spawnedList[i] is not CreatureController creature)
                    continue;
                Vector3 searchPos = owner.transform.position;
                searchPos.y += 1f;
                if (NavMesh.SamplePosition(searchPos, out NavMeshHit hit, 25f, NavMesh.AllAreas))
                {
                    if (i == 0)
                    {
                        creature.SetSurroundPosGroup(SurroundPosManager.AssignCenterTargetPosition(creature.gameObject, owner.GoalData._passengerGoalPos,
                            _surroundPosData._radiusFromCenter, _surroundPosData._distanceFromUnit, _surroundPosData._firstRingCount, true));
                        initalGroup = creature.GetSurroundPosGroup();
                    }
                    else
                    {
                        creature.SetSurroundPosGroup(initalGroup);
                        SurroundPosManager.AssignTargetPosition(creature.gameObject,initalGroup);
                    }
                    
                    SurroundPosManager.TryGetAssignedTargetPositionAround(creature.gameObject,initalGroup, out Vector3 assigendPos);
                    creature.transform.position = hit.position;
                    creature.gameObject.SetActive(true);
                    creature.SetModeType(owner.CurrentModeType);
                    creature.OnUnboard(assigendPos);
                    owner.SetDestMarkToCreature(creature);
                }
            }
        }
        #endregion
        owner.ClearPassengerDatas();
    }

    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
    }

    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
    }
}