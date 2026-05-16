using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftTraceState : State<SpacecraftState, SpacecraftController>
{
    public override SpacecraftState EState => SpacecraftState.Trace;

    public override void EnterState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        SpacecraftController owner = stateMachine.GetOwner();
        Vector3 raycastStart = owner.transform.position;
        Vector3 raycastDirection = owner.transform.forward;
        float rayMaxDistance = 1000f;
        bool bGetEnemy = InputManager.Instance.TryGetByRaycast(out RaycastHit enemy, raycastStart, raycastDirection, rayMaxDistance, GameLayerMask.EnemyOutPlanetLayerMask);
        if(bGetEnemy)
        {
            
        }
    }

    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
    }

    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
    }
}
