using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public class SpacecraftBoardingState : State<SpacecraftState, SpacecraftController>
{
    private BoardingStatData _boardingStatData;
    public override SpacecraftState EState => SpacecraftState.Boarding;

    public override void InitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        stateMachine.TryGetStateData(out _boardingStatData);
    }

    public override void EnterState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        SpacecraftController owner = stateMachine.GetOwner();

        List<Creature> _boardingCreatureList = CreatureSelection.Instance.GetSelectionComponents<Creature>();
        int creatureMaxCount = Math.Clamp(_boardingCreatureList.Count, 0, _boardingStatData._maxCount);
        _boardingStatData._finalMaxCount = creatureMaxCount;
        NavMeshPath path = new();
        bool isPathValid = false;
        for (int i = 0; i < creatureMaxCount; i++)
        {
            if (NavMesh.SamplePosition(owner.transform.position, out NavMeshHit hit, 30f, NavMesh.AllAreas))
            {
                NavMesh.CalculatePath(_boardingCreatureList[i].transform.position, hit.position, NavMesh.AllAreas, path);
                if (path.status == NavMeshPathStatus.PathInvalid)
                {
                    Debug.Log($"creatureList[{i}] 길 막혔습니다");
                    --_boardingStatData._finalMaxCount;
                    continue;
                }
                if (!isPathValid)
                {
                    isPathValid = true;
                }
                _boardingCreatureList[i].TargetPosition = hit.position;
                _boardingCreatureList[i].StateMachine.ChangeState(CreatureState.Boarding);
                _boardingStatData._boardingCreatureList.Add(_boardingCreatureList[i]);
            }
        }
        CreatureSelection.Instance.ClearSelectedList();
        if (isPathValid)
        {
            DriveButtonController.Instance.AddDriveButton(owner, _boardingStatData._finalMaxCount);
            _boardingStatData.driveButton = DriveButtonController.Instance.GetDriveButton(stateMachine.GetOwner());
        }
        else
        {
            stateMachine.ChangeState(SpacecraftState.Idle);
            Debug.Log("탑승 가능한 유닛이 없습니다.");
        }
    }
    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        if (!_boardingStatData.driveButton)
        {
            stateMachine.ChangeState(SpacecraftState.Idle);
            return;
        }
        bool bGetChild = MyUnitPrefabDataControl.Instance.TryGetChild(out GameObject child, UnitType.Creature);
        if (!bGetChild)
            return;
        SpacecraftController owner = stateMachine.GetOwner();
        for (int i = 0; i < _boardingStatData._boardingCreatureList.Count; i++)
        {
            if (!_boardingStatData._boardingCreatureList[i].gameObject.activeSelf)
            {
                owner.AddPassenger(_boardingStatData._boardingCreatureList[i], 1, child.transform);
                _boardingStatData.driveButton.SetDriveCount(++_boardingStatData._curCount, _boardingStatData._finalMaxCount);
                _boardingStatData._boardingCreatureList.RemoveAt(i);
                return;
            }
        }
    }
    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine) 
    {

    }


}

