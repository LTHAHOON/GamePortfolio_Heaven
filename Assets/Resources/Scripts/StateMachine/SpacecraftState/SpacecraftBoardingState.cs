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
        if (DriveButtonController.Instance.IsContainDriveButton(owner))
            return;
        List<CreatureController> boardingCreatureList = CreatureSelection.Instance.GetSelectionComponents<CreatureController>();
        int creatureMaxCount = Math.Clamp(boardingCreatureList.Count, 0, _boardingStatData._maxCount);
        _boardingStatData._finalMaxCount = creatureMaxCount;
        NavMeshPath path = new();
        bool isPathValid = false;
        for (int i = 0; i < creatureMaxCount; i++)
        {
            if (NavMesh.SamplePosition(owner.transform.position, out NavMeshHit hit, 30f, NavMesh.AllAreas))
            {
                NavMeshQueryFilter filter = new NavMeshQueryFilter();
                filter.agentTypeID = NavMesh.GetSettingsByIndex(1).agentTypeID;
                filter.areaMask = NavMesh.AllAreas;
                NavMesh.CalculatePath(boardingCreatureList[i].transform.position, hit.position, filter, path);
                if (path.status == NavMeshPathStatus.PathInvalid)
                {
                    Debug.Log($"creatureList[{i}] �� �������ϴ�");
                    --_boardingStatData._finalMaxCount;
                    continue;
                }
                if (!isPathValid)
                {
                    isPathValid = true;
                }
                boardingCreatureList[i].SetDestination(hit.position);
                boardingCreatureList[i].StateMachine.ChangeState(CreatureState.Boarding);
                _boardingStatData._boardingCreatureList.Add(boardingCreatureList[i]);
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
        }
    }
    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        if (!_boardingStatData.driveButton)
        {
            stateMachine.ChangeState(SpacecraftState.Idle);
            return;
        }
        
        SpacecraftController owner = stateMachine.GetOwner();
        for (int i = 0; i < _boardingStatData._boardingCreatureList.Count; i++)
        {
            //ž�¿Ϸ��� ���
            if (!_boardingStatData._boardingCreatureList[i].gameObject.activeSelf)
            {
                owner.AddPassenger(_boardingStatData._boardingCreatureList[i], 1);
                _boardingStatData.driveButton.SetDriveCount(++_boardingStatData._curCount, _boardingStatData._finalMaxCount);
                _boardingStatData._boardingCreatureList.RemoveAt(i);
                return;
            }
            //ž���� ���� ���
            else if(_boardingStatData._boardingCreatureList[i].StateMachine.CurrentState.EState != CreatureState.Boarding)
            {
                _boardingStatData.driveButton.SetDriveCount(_boardingStatData._curCount, --_boardingStatData._finalMaxCount);
                _boardingStatData._boardingCreatureList.RemoveAt(i);
                return;
            }
        }
    }
    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine) 
    {

    }


}

