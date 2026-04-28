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
        owner.transform.position = owner.GoalData._spacecraftGoalPos;
        _layerTargetStatData._layerTargetList.SetLayerList(owner.gameObject, true, owner.UnitTypeLayer);
        //Gravity로 인해 착륙하게 되고 바닥에 닿은 순간 Idle상태로 전환하게 된다.
        owner._isGravity = true;
    }
    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine){ }

    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine){ }

}
