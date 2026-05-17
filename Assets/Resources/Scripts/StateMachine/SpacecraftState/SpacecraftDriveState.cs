using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class SpacecraftDriveState : State<SpacecraftState, SpacecraftController>
{
    private BaseFSMStatData _baseFsmStatData;
    private BezierCurveStatData _bezierCurveStatData;
    private float _curSpeed = 1f;
    private float _velocity = 0f;
    private readonly float _baseSpeed = 1f;
    private readonly float _smoothTime = 0.6f;
    public override SpacecraftState EState => SpacecraftState.Drive;

    public override void InitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine) 
    {
        stateMachine.TryGetStateData(out _baseFsmStatData);
        stateMachine.TryGetStateData(out _bezierCurveStatData);
    }
    public override void EnterState(StateMachine<SpacecraftState, SpacecraftController> stateMachine) 
    {
        SpacecraftController owner = stateMachine.GetOwner();
        if(owner.CurrentModeType == ModeType.DefenseMode)
        {
            owner.SetModeType(owner.OppositeModeType);
        }
    }

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
            //Vector3 e = ((1 - _bezierCurveStatData._curTime / maxTime) * startPoint) + (_bezierCurveStatData._curTime / maxTime * middlePoint);
            Vector3 e = Vector3.Lerp(startPoint, middlePoint, _bezierCurveStatData._curTime / maxTime);

            //Vector3 f = ((1 - _bezierCurveStatData._curTime / maxTime) * middlePoint) + (_bezierCurveStatData._curTime / maxTime * endPoint);
            Vector3 f = Vector3.Lerp(middlePoint, endPoint, _bezierCurveStatData._curTime / maxTime);

            //Vector3 p = ((1 - _bezierCurveStatData._curTime / maxTime) * e) + (_bezierCurveStatData._curTime / maxTime * f);
            
            Vector3 p = Vector3.Lerp(e, f, _bezierCurveStatData._curTime / maxTime);
            if (owner.Status != null)
            {
                _bezierCurveStatData._curTime += Time.deltaTime * Mathf.Abs(_curSpeed) * (owner.Status.DEX / 3f);
            }
            else
            {
                _bezierCurveStatData._curTime += Time.deltaTime;
            }

            Vector3 dir = p - owner.transform.position;
            Quaternion rot = Quaternion.LookRotation(dir.normalized);
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rot, Time.deltaTime * 3f);
            owner.transform.position = p;
        }
        if(InputManager.Instance.TryGetByRaycast(out RaycastHit enemy, owner.transform.position, owner.transform.forward, 
                    _baseFsmStatData._traceDistance ,GameLayerMask.EnemyOutPlanetLayerMask))
        {
            _curSpeed = Mathf.SmoothDamp(_curSpeed, -1f, ref _velocity, _smoothTime);
            if (_curSpeed <= 0f)
            {
               stateMachine.ChangeState(SpacecraftState.Trace);
               _curSpeed = _baseSpeed;
            }
                
        }
        else if (_bezierCurveStatData._curTime >= maxTime)
        {
            _bezierCurveStatData._curTime = 0;
            //행성에 도착하면 착륙상태로 전환하게 된다.
            stateMachine.ChangeState(SpacecraftState.Landing);
        }
    }
    public override void ExitState(StateMachine<SpacecraftState, SpacecraftController> stateMachine) { }

}
