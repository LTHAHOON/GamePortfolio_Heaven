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
    protected static PoolComponent _pcAttackMark;
    protected PassengerController _vehicleUnit;
    protected MPData _totalMPData = new();

    protected void SetVehicleUnit(PassengerController vehicleUnit)
    {
        _vehicleUnit = vehicleUnit;
    }

    public override void OnEnter()
    {
        if (ModeType == ModeType.AttackDriveMode)
        {
            CursorManager.Instance.SetCursor(CursorType.Attack);
             _cursorData = CursorManager.GetCursorData(CursorType.Attack);
            _planetButtonController.SetToggleIsOn(1, true);
        }
        else
        {
            CursorManager.Instance.SetCursor(CursorType.Defend);
            _cursorData = CursorManager.GetCursorData(CursorType.Defend);
            _planetButtonController.SetToggleIsOn(0, true);
        }
        UIManager.Instance.SetActiveAllChild(_createCountController.gameObject, true);
        _createCountController.SetActiveCount(false);
        //CursorManager.SetSpriteRendererEnabled(CursorType.Attack, false);
        SpawnPointsManager.Instance.SetActiveSpawPointsControl(true, ModeType);
    }

    public override void OnExecute()
    {
        _goalData._passengerGoalPos = _cursorData.GetFollwingSpriteRenderer().transform.position;
        GameObject attackMark = _pcAttackMark.PopPoolObject();
        attackMark.transform.position = _goalData._passengerGoalPos;


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
            spacecraftController.GetCreateLoad().SetLoadReady(false);
            spacecraftController.SetAttackMark(attackMark, ReturnAttackMark);
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
            if (ModeType == ModeType.AttackDriveMode)
            {
                _startPos = _startPoint.position;
                _endPos = _endPoint.position;
                CursorManager.SetSpriteRendererEnabled(CursorType.Attack, true);
            }
            else
            {
                _startPos = _endPoint.position;
                _endPos = _startPoint.position;
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
        _bSetGoalProcess = false;
        if (ModeType == ModeType.AttackDriveMode)
        {
            _planetButtonController.SetToggleIsOn(1, false);
        }
        else
        {
            _planetButtonController.SetToggleIsOn(0, false);
        }
        SpawnPointsManager.Instance.SetActiveSpawPointsControl(false, ModeType);
        _cursorData = null;
    }

    public override void RefreshModeButton(){ }

    public virtual void SetGoalProcess(Vector3 spacecraftGoalPosition, RespawnPositionType respawnPositionType)
    {
        _createCountController.ChangeGuideText(_guideText_Next);
        SpawnPointsManager.Instance.SetActiveSpawPointsControl(false, ModeType);
        _goalData = new()
        {
            _spacecraftGoalPos = spacecraftGoalPosition,
            _respawnPositionType = respawnPositionType
        };
        _bSetGoalProcess = true;
    }

    protected Vector3 _startPos;
    private Vector3 _endPos;
    protected void SetGoal(GameObject spacecraftPrefab, Goal goalData)
    {

        
        if (spacecraftPrefab.TryGetComponent(out SpacecraftController spacecraftController))
        {
            switch (goalData._respawnPositionType)
            {
                case RespawnPositionType.RespawnForward:
                {
                    spacecraftController.SetGoal(_startPos,_endPos, Vector3.zero, goalData);
                    break;
                }
                case RespawnPositionType.RespawnBackward:
                {
                    spacecraftController.SetGoal(_startPos, _endPos, Vector3.zero, goalData);
                    break;
                }
                case RespawnPositionType.RespawnLeft:
                {
                    spacecraftController.SetGoal(_startPos, _endPos, _leftMiddlePoint.position,goalData);
                    break;
                }
                case RespawnPositionType.RespawnRight:
                {
                    spacecraftController.SetGoal(_startPos,_endPos, _rightMiddlePoint.position, goalData);
                    break;
                }
            }
        }
    }

    protected void ReturnAttackMark(GameObject attackMark)
    {
        _pcAttackMark.ReturnPoolObject(attackMark);
    }
}