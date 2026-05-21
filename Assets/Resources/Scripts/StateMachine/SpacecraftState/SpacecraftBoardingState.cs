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
    private NavMeshPath _path;
    private NavMeshQueryFilter _filter;
   
    public override void InitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        stateMachine.TryGetStateData(out _boardingStatData);
        _path = new();
        _filter = new NavMeshQueryFilter();
        _filter.agentTypeID = NavMesh.GetSettingsByIndex(1).agentTypeID;
        _filter.areaMask = NavMesh.AllAreas;
    }

    public override void EnterState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        SpacecraftController owner = stateMachine.GetOwner();
        if (DriveButtonController.Instance.IsContainDriveButton(owner))
            return;
        List<CreatureController> boardingCreatureList = CreatureSelection.Instance.GetSelectionComponents<CreatureController>();
        int creatureMaxCount = Math.Clamp(boardingCreatureList.Count, 0, _boardingStatData._maxCount);
        _boardingStatData._finalMaxCount = creatureMaxCount;
        bool isPathValid = false;
        for (int i = 0; i < creatureMaxCount; i++)
        {
            if (boardingCreatureList[i] is not IPassenger passenger)
                return;
            if (NavMesh.SamplePosition(owner.transform.position, out NavMeshHit hit, 30f, NavMesh.AllAreas))
            {
                NavMesh.CalculatePath(boardingCreatureList[i].transform.position, hit.position, _filter, _path);
                if (_path.status == NavMeshPathStatus.PathInvalid)
                {
                    --_boardingStatData._finalMaxCount;
                    continue;
                }
                if (!isPathValid)
                {
                    isPathValid = true;
                }
                boardingCreatureList[i].SetDestination(hit.position);
                boardingCreatureList[i].StateMachine.ChangeState(CreatureState.Boarding);
                _boardingStatData._boardingPassengerList.Add(boardingCreatureList[i]);
            }
        }
        CreatureSelection.Instance.ClearSelectedList();
        if (isPathValid)
        {

            DriveButtonController.Instance.AddDriveButton(owner, owner.CurrentModeType, _boardingStatData._finalMaxCount);
            _boardingStatData.driveButton = DriveButtonController.Instance.GetDriveButton(owner);
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
        List<IPassenger> boardingPassengerList = _boardingStatData._boardingPassengerList;
        for (int i = 0; i < boardingPassengerList.Count; i++)
        {
            if (boardingPassengerList[i] is not CreatureController creature)
                return;

            if (boardingPassengerList[i].SuccessBoard)
            {
                creature.gameObject.SetActive(false);
                owner.AddSpawnedPassenger(creature);
                _boardingStatData.driveButton.SetDriveCount(++_boardingStatData._curCount, _boardingStatData._finalMaxCount);
                boardingPassengerList.RemoveAt(i);
                return;
            }
            else if(creature.StateMachine.CurrentState.EState != CreatureState.Boarding)
            {
                _boardingStatData.driveButton.SetDriveCount(_boardingStatData._curCount, --_boardingStatData._finalMaxCount);
                boardingPassengerList.RemoveAt(i);
                return;
            }
        }
    }
    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine) 
    {
        _boardingStatData._curCount = 0;
        _boardingStatData._finalMaxCount = 0;
    }


}

