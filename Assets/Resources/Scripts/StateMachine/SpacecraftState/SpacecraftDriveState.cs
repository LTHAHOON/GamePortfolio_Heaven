using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class SpacecraftDriveState : State<SpacecraftState, SpacecraftController>
{
    private BezierCurveStatData _bezierCurveStatData;

    public override SpacecraftState EState => SpacecraftState.Drive;

    public override void InitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine) 
    {
        stateMachine.TryGetStateData(out _bezierCurveStatData);
    }
    public override void EnterState(StateMachine<SpacecraftState, SpacecraftController> stateMachine) { }

    public override void UpdateState(StateMachine<SpacecraftState, SpacecraftController> stateMachine)
    {
        SpacecraftController owner = stateMachine.GetOwner();
        float maxTime = _bezierCurveStatData._maxTime;
        Vector3 startPoint = _bezierCurveStatData._startPoint;
        Vector3 endPoint = _bezierCurveStatData._endPoint;
        Vector3 middlePoint = _bezierCurveStatData._middlePoint;
        if (_bezierCurveStatData._middlePoint == Vector3.zero)
        {
            Vector3 p = ((1 - _bezierCurveStatData._curTime / maxTime) * startPoint)
                + (_bezierCurveStatData._curTime / maxTime * endPoint);
            _bezierCurveStatData._curTime += Time.deltaTime;
            owner.transform.position = p;
        }
        else
        {
            Vector3 e = ((1 - _bezierCurveStatData._curTime / maxTime) * startPoint) + (_bezierCurveStatData._curTime / maxTime * middlePoint);
            Vector3 f = ((1 - _bezierCurveStatData._curTime / maxTime) * startPoint) + (_bezierCurveStatData._curTime / maxTime * endPoint);
            Vector3 p = ((1 - _bezierCurveStatData._curTime / maxTime) * e) + (_bezierCurveStatData._curTime / maxTime * f);
            if (owner.Status != null)
            {
                _bezierCurveStatData._curTime += Time.deltaTime * (owner.Status.DEX / 3f);
            }
            else
            {
                _bezierCurveStatData._curTime += Time.deltaTime;
            }

            Vector3 dir = p - owner.transform.position;
            Quaternion rot = Quaternion.LookRotation(dir.normalized);
            Vector3 changedEulerAngle = rot.eulerAngles;
            Vector3 curEulerAngle = owner.transform.rotation.eulerAngles;
            changedEulerAngle.x = curEulerAngle.x;
            rot = Quaternion.Euler(changedEulerAngle);

            owner.transform.SetPositionAndRotation(p, rot);
        }

        if (_bezierCurveStatData._curTime >= maxTime)
        {
            _bezierCurveStatData._curTime = 0;
            //행성에 도착하면 착륙상태로 전환하게 된다.
            stateMachine.ChangeState(SpacecraftState.Landing);
        }
    
    }
    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine) { }

}
