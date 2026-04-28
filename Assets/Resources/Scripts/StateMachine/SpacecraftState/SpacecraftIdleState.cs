using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftIdleState : State<SpacecraftState, SpacecraftController>
{
    public override SpacecraftState EState => SpacecraftState.Idle;
    public override void EnterState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        SpacecraftController owner = stateMachine.GetOwner();
        TransparentMaterialControl.SetQpaqueOrTransparentControl(owner.gameObject, owner.UnitType, TransparentMaterialControl.SurfaceType.Opaque, new Color32(255, 255, 255, 255));
        
    }

    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
    }

    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
    }
}
