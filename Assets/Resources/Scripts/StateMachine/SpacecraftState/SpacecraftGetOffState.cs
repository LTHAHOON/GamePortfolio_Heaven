using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public class SpacecraftGetOffState : State<SpacecraftState, SpacecraftController>
{
    private SurroundPosStatData _surroundPosData;
    public override SpacecraftState EState => SpacecraftState.GetOff;

    public override void InitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        stateMachine.TryGetStateData(out _surroundPosData);
    }

    public override void EnterState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        SpacecraftController owner = stateMachine.GetOwner();
        int[] positionCountArray = SurroundPosManager.GetPositionCountArray(owner.AllPassengerCount,
            _surroundPosData._firstRingCount);
        float[] distancesArray = SurroundPosManager.DistanceArrayByCharacterCount(owner.AllPassengerCount,
            _surroundPosData._distanceFromUnit,
            _surroundPosData._radiusFromCenter, _surroundPosData._firstRingCount);
        Vector3[] arrGoalPos = SurroundPosManager.GetTargetPositionsAround(owner.GoalData._passengerGoalPos,
            distancesArray, positionCountArray);
        List<PassengerData> passengerList = owner.GetPassengerDatas();
        int startIndex = 0;
        for (int i = 0; i < passengerList.Count; i++)
        {
            for (int j = startIndex; j < passengerList[i].PassengerCount; j++)
            {
                if (NavMesh.SamplePosition(owner.GoalData._spacecraftGoalPos + Random.onUnitSphere, out NavMeshHit hit,
                        5f, NavMesh.AllAreas))
                {
                    Debug.Log($"SpacecraftGetOffState : GetOff {passengerList[i].Passenger} to {arrGoalPos[j]}");
                    Unit unit = UnitSpawnManager.Instance.Spawn(passengerList[i].Passenger);
                    unit.transform.position = hit.position;
                    if (unit is IPassenger passenger)
                    {
                        passenger.OnUnboard(arrGoalPos[j], owner.GoalData._enemytNexusPos);
                        if (passenger is CreatureController creature)
                        {
                            owner.SetAttackMarkToCreature(creature);
                        }
                    }
                }
            }
        }
    }

    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
    }

    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
    }
}