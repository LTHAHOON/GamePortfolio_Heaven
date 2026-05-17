using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftAttackState : State<SpacecraftState, SpacecraftController>
{
    public override SpacecraftState EState => SpacecraftState.Attack;
    private BaseFSMStatData _baseFsmStatData;
    public override void EnterState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        stateMachine.TryGetStateData(out _baseFsmStatData);
    }

    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        if (_baseFsmStatData == null)
            return;
        if (!_baseFsmStatData.TargetCollider)
            return;
        
        SpacecraftController owner = stateMachine.GetOwner();
        Vector3 raycastStart = owner.transform.position;
        Vector3 raycastDirection = owner.transform.forward;
        Vector3 dir =   _baseFsmStatData.TargetPosition - owner.transform.position;
        Quaternion dirRotation = Quaternion.LookRotation(dir);
        bool bGetEnemy = InputManager.Instance.TryGetByRaycast(out RaycastHit enemy, raycastStart, raycastDirection, _baseFsmStatData._attackDistance ,GameLayerMask.EnemyOutPlanetLayerMask);
        if(!bGetEnemy)
        {
            stateMachine.ChangeState(SpacecraftState.Trace);
        }
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, dirRotation, Time.deltaTime * 5f);

        Debug.Log("Attack");
    }

    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        
    }
}
