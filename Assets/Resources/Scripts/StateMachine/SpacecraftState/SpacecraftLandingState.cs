using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class SpacecraftLandingState : State<SpacecraftState, SpacecraftController>
{
    private LayerTargetStatData _layerTargetStatData;
    public override SpacecraftState EState => SpacecraftState.Landing;

    public override void InitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine) 
    {
        stateMachine.TryGetStateData(out _layerTargetStatData);
    }
    public override void EnterState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        SpacecraftController owner = stateMachine.GetOwner();
        owner.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        owner.transform.position = owner.GoalData._vehicleGoalPosData.Data.position;
        _layerTargetStatData._layerTargetList.SetLayerList(owner.gameObject, true, GameLayer.SpacecraftLayer);
        //Gravity�� ���� �����ϰ� �ǰ� �ٴڿ� ���� ���� Idle���·� ��ȯ�ϰ� �ȴ�.
        owner._isGravity = true;
    }
    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        SpacecraftController owner = stateMachine.GetOwner();
        if(!owner._isGravity)
        {
            stateMachine.ChangeState(SpacecraftState.GetOff);
        }
    }

    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine){ }

}
