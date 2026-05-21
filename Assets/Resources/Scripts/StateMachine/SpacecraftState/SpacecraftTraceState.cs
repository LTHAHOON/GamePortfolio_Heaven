using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftTraceState : State<SpacecraftState, SpacecraftController>
{
    public override SpacecraftState EState => SpacecraftState.Trace;

    private BaseFSMStatData _baseFsmStatData;
    public override void InitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        stateMachine.TryGetStateData(out _baseFsmStatData);
    }

    public override void EnterState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        
    }
    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        if (_baseFsmStatData == null)
            return;

        SpacecraftController owner = stateMachine.GetOwner();
        Vector3 raycastStart = owner.transform.position;
        Vector3 raycastDirection = owner.transform.forward;
        bool bHit = InputManager.Instance.TryGetByRaycast(out RaycastHit hit, raycastStart, raycastDirection, _baseFsmStatData._attackDistance, GameLayerMask.AllOutPlanetMask);
        if (bHit)
        {
            if (hit.collider.gameObject.layer == GameLayer.OutPlanetEnemyLayer)
            {
                _baseFsmStatData.SetTarget(hit.collider);
                stateMachine.ChangeState(SpacecraftState.Attack);
            }
        }
        else
        {
            stateMachine.ChangeState(SpacecraftState.Drive);
        }
    }
    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
    }


}
