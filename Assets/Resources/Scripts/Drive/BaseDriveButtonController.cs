using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BaseDriveButtonController : ModeButtonController
{
    [Header("Next GuideText")]
    [Multiline]
    [SerializeField]
    private string _guideText_Next;
    #region 이동할 지점
    [SerializeField] 
    private Transform _startPoint;
    [SerializeField] 
    private Transform _leftMiddlePoint;
    [SerializeField] 
    private Transform _rightMiddlePoint;
    [SerializeField] 
    private Transform _endPoint;
    #endregion
    [Space(10f)] [Header("MouseCursorScript")] [SerializeField]
    private MouseCursorController _mouseCursorController;
    [SerializeField] 
    protected GameObject _attackMark;
    [SerializeField] 
    protected Transform _mapMarkParent;

    protected MouseCursorData _cursorData;

    protected bool _bSetGoalProcess = false;
    protected Goal _goalData;
    protected static PoolComponent _pcDestMark;
    protected PassengerController _vehicleUnit;
    protected MPData _totalMPData = new();

    protected void SetVehicleUnit(PassengerController vehicleUnit)
    {
        _vehicleUnit = vehicleUnit;
    }

    public override void OnEnter()
    {
        if (ModeButtonType == ModeType.AttackMode)
        {
            _startPos = _startPoint.position;
            _endPos = _endPoint.position;
            CursorManager.Instance.SetCursor(CursorType.Attack);
            CursorManager.SetSpriteRendererEnabled(CursorType.Attack, false);
            _cursorData = CursorManager.GetCursorData(CursorType.Attack);
            _planetButtonController.SetToggleIsOn(1, true);
        }
        else 
        {
            _startPos = _endPoint.position;
            _endPos = _startPoint.position;
            CursorManager.Instance.SetCursor(CursorType.Defend);
            CursorManager.SetSpriteRendererEnabled(CursorType.Defend, false);
            _cursorData = CursorManager.GetCursorData(CursorType.Defend);
            _planetButtonController.SetToggleIsOn(0, true);
            LandingPointButtonsManager.Instance.SetActiveLandingPointButtons(true, ModeType.DefenseMode);
        }
        LandingPointButtonsManager.Instance.SetActiveLandingPointButtons(true, _vehicleUnit.OppositeModeType);
        UIManager.Instance.SetActiveAllChild(_createCountController.gameObject, true);
        _createCountController.SetActiveCount(false);
    }

    public override void OnExecute()
    {
        GameObject destMark = _pcDestMark.PopPoolObject();
        if (!destMark) return;

        if(destMark.TryGetComponent(out ParticleColorSystem particleColorSystem))
        {
            Color color = UIManager.Instance.GetDestMarkColor(ModeButtonType);
            particleColorSystem.ChangeParticleColor(color);
        }
        _goalData._passengerGoalPos = _cursorData.GetFollwingSpriteRenderer().transform.position;
        destMark.transform.position = _goalData._passengerGoalPos;

        List<PassengerData> passengerDatas = _vehicleUnit.GetUnSpawnedPassengers();
        for (int i = 0; i < passengerDatas.Count; i++)
        {
            MPData unitMpData = passengerDatas[i].Passenger.UnitMPData;
            MPDataController.Instance.UseUpMP(unitMpData, passengerDatas[i].PassengerCount);
        }
        MPDataController.Instance.UseUpMP(_vehicleUnit.UnitMPData, 1);
        
        _vehicleUnit.transform.position = _startPos;
        if (_vehicleUnit.TryGetComponent(out SpacecraftController spacecraftController))
        {
            if(spacecraftController.GoalData._vehicleGoalPosData != null)
            {
                LandingPointDatasController landingPointDatasControl = spacecraftController.GoalData._vehicleGoalPosData.Owner;
                landingPointDatasControl.ReturnLandingPosition(spacecraftController.GoalData._vehicleGoalPosData);
            }
            spacecraftController.GetCreateLoad().SetLoadReady(false);
            spacecraftController.SetDestMark(destMark, ReturnDestMark);
        }

        _bSetGoalProcess = false;
        SetGoal(_vehicleUnit.gameObject, _goalData);
        OnExit(true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!_vehicleUnit || _cursorData == null) 
            return;


        if (_bSetGoalProcess)
        {
            if (ModeButtonType == ModeType.AttackMode)
            {
                CursorManager.SetSpriteRendererEnabled(CursorType.Attack, true);
            }
            else
            {
                CursorManager.SetSpriteRendererEnabled(CursorType.Defend, true);
            }
            CursorManager.Instance.SpriteFollowMouse(_cursorData.GetFollwingSpriteRenderer());
            if (Input.GetMouseButtonDown(0) && _cursorData.GetFollwingSpriteRenderer().enabled)
            {
                OnExecute();
            }
        }
    }

    public override void OnExit(bool bExitCompletely)
    {
        base.OnExit(bExitCompletely);
        if(!bExitCompletely)
        {
            LandingPointDatasController landingPointDatasContrl = _goalData._vehicleGoalPosData.Owner;
            landingPointDatasContrl.ReturnLandingPosition(_goalData._vehicleGoalPosData);
        }
        _bSetGoalProcess = false;
        if (ModeButtonType == ModeType.AttackMode)
        {
            _planetButtonController.SetToggleIsOn(1, false);
        }
        else
        {
            _planetButtonController.SetToggleIsOn(0, false);
        }
        LandingPointButtonsManager.Instance.SetActiveLandingPointButtons(false, ModeButtonType);
        _cursorData = null;
    }

    public override void RefreshModeButton(){ }

    public virtual void SetGoalProcess(LandingPointData respawnPosData)
    {
        _createCountController.ChangeGuideText(_guideText_Next);
        LandingPointButtonsManager.Instance.SetActiveLandingPointButtons(false, ModeButtonType);
        _goalData = new()
        {
            _vehicleGoalPosData = respawnPosData,
        };
        _bSetGoalProcess = true;
    }

    protected Vector3 _startPos;
    private Vector3 _endPos;
    protected void SetGoal(GameObject spacecraftPrefab, Goal goalData)
    {

        
        if (spacecraftPrefab.TryGetComponent(out SpacecraftController spacecraftController))
        {
            switch (goalData._vehicleGoalPosData.Owner.RespawnPositionType)
            {
                case LadingPointType.LandingForward:
                {
                    spacecraftController.SetGoal(_startPos,_endPos, Vector3.zero, goalData);
                    break;
                }
                case LadingPointType.LandingBackward:
                {
                    spacecraftController.SetGoal(_startPos, _endPos, Vector3.zero, goalData);
                    break;
                }
                case LadingPointType.LandingLeft:
                {
                    spacecraftController.SetGoal(_startPos, _endPos, _leftMiddlePoint.position,goalData);
                    break;
                }
                case LadingPointType.LandingRight:
                {
                    spacecraftController.SetGoal(_startPos,_endPos, _rightMiddlePoint.position, goalData);
                    break;
                }
            }
        }
    }

    protected void ReturnDestMark(GameObject destMark)
    {
        _pcDestMark.ReturnPoolObject(destMark);
    }
}