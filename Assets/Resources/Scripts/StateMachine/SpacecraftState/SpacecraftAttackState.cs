using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacecraftAttackState : State<SpacecraftState, SpacecraftController>
{
    public override SpacecraftState EState => SpacecraftState.Attack;
    private BaseFSMStatData _baseFsmStatData;
    private WeaponStatData _weaponStatData;
    public override void EnterState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        stateMachine.TryGetStateData(out _baseFsmStatData);
        stateMachine.TryGetStateData(out _weaponStatData);
    }

    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        if (_baseFsmStatData == null || _weaponStatData == null)
            return;
        if (!_baseFsmStatData.TargetCollider || !_weaponStatData.WeaponController)
        {
            stateMachine.ChangeState(SpacecraftState.Trace);
            return;
        }
        
        SpacecraftController owner = stateMachine.GetOwner();
        Vector3 raycastStart = owner.transform.position;
        Vector3 raycastDirection = owner.transform.forward;
        Vector3 dir =   _baseFsmStatData.TargetPosition - owner.transform.position;
        dir.y = 0f;
        Quaternion dirRotation = Quaternion.LookRotation(dir);
        RaycastHit hit = InputManager.Instance.GetBySphereCast(raycastStart, raycastDirection, 5f, _baseFsmStatData._attackDistance ,GameLayerMask.DamageableColliderMask);
        if(!hit.collider)
        {
            stateMachine.ChangeState(SpacecraftState.Trace);
        }
        else if(!hit.collider.CompareTag(GameTags.Enemy))
        {
            stateMachine.ChangeState(SpacecraftState.Trace);
        }
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, dirRotation, Time.deltaTime * 5f);

        var weaponController = _weaponStatData.WeaponController;
        weaponController.Attack();
    }

    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        
    }
}
