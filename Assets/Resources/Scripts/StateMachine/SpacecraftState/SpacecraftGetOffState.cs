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
        List<PassengerData> passengerList = owner.GetPassengerDatas();
        for (int i = 0; i < passengerList.Count; i++)
        {
            int[] positionCountArray = SurroundPosManager.GetPositionCountArray(passengerList[i]._passengerCount, _surroundPosData._firstRingCount);
            float[] distancesArray = SurroundPosManager.DistanceArrayByCharacterCount(passengerList[i]._passengerCount, _surroundPosData._distanceFromUnit,
                _surroundPosData._radiusFromCenter, _surroundPosData._firstRingCount);
            Vector3[] arrGoalPos = SurroundPosManager.GetTargetPositionsAround(owner.GoalData._passengerGoalPos, distancesArray, positionCountArray);
            for (int j = 0; j < passengerList[i]._passengerCount; j++)
            {
                if (NavMesh.SamplePosition(owner.GoalData._spacecraftGoalPos + Random.onUnitSphere, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                {
                    passengerList[i]._passenger.transform.position = hit.position;
                }
                Unit passenger = MonoBehaviour.Instantiate(passengerList[i]._passenger, owner.PassengerParent);
                if (passenger.TryGetComponent(out Creature creature))
                {
                    MyUnitPrefabDataControl.Instance.AddUnitPrefabToList(UnitType.Creature, passenger);
                    //  creature.SetStatus(StatusSliderController._status);
                    creature.SetEnemyNexusTargetPos(owner.GoalData._enemytNexusPos);
                    creature.TargetPosition = arrGoalPos[j];
                    creature.SetIsAttackTarget(true);
                    creature.SetIsAttackMode(true);    
                }
                
                if (j == 0)
                {
                    owner.SetAttackMarkToCreature(creature);
                }
            }
        }
    }

    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
    }
    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine) { }

}
