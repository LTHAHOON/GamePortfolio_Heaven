using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BaseDriveButtonController : ModeButtonController
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _leftMiddlePoint;
    [SerializeField] private Transform _rightMiddlePoint;
    [SerializeField] private Transform _endPoint;

    [Header("EnemyNexusTarget")] [SerializeField]
    private Transform _enemyNexusTarget;

    [Header("InstancePos(Parent)")] [SerializeField]
    private GameObject _myUnitPrefab;

    [Space(10f)] [Header("MouseCursorScript")] [SerializeField]
    private MouseCursorController _mouseCursorController;

    [SerializeField] protected GameObject _attackMark;
    [SerializeField] protected Transform _mapMarkParent;
    [SerializeField] protected Transform _driveButtonParent;
    protected Vector3 _startPos = new(0f, 0f, -173.5f);
    protected MouseCursorData _cursorData;
    private MPData _totalMPData = new();
    protected bool _bSetGoalProcess = false;
    protected Goal _goalData;
    protected static PoolComponent _pcAttackMark;
    protected PassengerController _vehicleUnit;

    protected void SetVehicleUnit(PassengerController vehicleUnit)
    {
        _vehicleUnit = vehicleUnit;
    }

    public override void OnEnter()
    {
        _bReadyUnit = true;
        _bGetUnit = true;
        _planetButtonController.SetToggleIsOn(1, true);
        UIManager.Instance.SetActiveAllChild(AttackSpawnTargetController.Instance.gameObject, true);
    }

    public override void OnExecute()
    {
        _goalData._passengerGoalPos = _cursorData.GetFollwingSpriteRenderer().transform.position;
        GameObject attackMark = _pcAttackMark.PopPoolObject();
        attackMark.transform.position = _goalData._passengerGoalPos;

        _vehicleUnit.transform.position = _startPos;

        List<PassengerData> passengerDatas = _vehicleUnit.GetPassengerDatas();
        for (int i = 0; i < passengerDatas.Count; i++)
        {
            MPData passengerMPData = passengerDatas[i]._passenger.MPData;
            MPDataController.Instance.UseUpMP(passengerMPData.MP_ConsValue,
                passengerDatas[i]._passengerCount);
        }
        MPDataController.Instance.UseUpMP(_vehicleUnit.MPData.MP_ConsValue, 1);
        
        if (_vehicleUnit.TryGetComponent(out SpacecraftController spacecraftController))
        {
            spacecraftController.GetCreateLoad().SetLoadReady(false);
            spacecraftController.SetAttackMark(attackMark, ReturnAttackMark);
        }

        _bSetGoalProcess = false;
        //��ǥ �����ϱ�
        SetGoal(_vehicleUnit.gameObject, _goalData);
        OnExit(true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_bGetUnit)
        {
            _totalMPData.MP_ConsValue = _vehicleUnit.MPData.MP_ConsValue + _selectedUnitPrefab.MPData.MP_ConsValue;
            MPDataController.Instance.UpdateButtonToMPData(_totalMPData, ref _thisButton, ref _buttonImage,
                ref _buttonText);
        }

        if (_bReadyUnit)
        {
            _createCountController.RefreshCreateCount(_vehicleUnit.MPData, _selectedUnitPrefab.MPData);
            if (_cursorData == null)
            {
                CursorManager.Instance.SetCursor(CursorType.Attack);
                CursorManager.SetSpriteRendererEnabled(CursorType.Attack, false);
                _cursorData = CursorManager.GetCursorData(CursorType.Attack);
            }

            if (_bSetGoalProcess)
            {
                CursorManager.SetSpriteRendererEnabled(CursorType.Attack, true);
                CursorManager.Instance.SpriteFollowMouse(_cursorData.GetFollwingSpriteRenderer());
                if (Input.GetMouseButtonDown(0) && _cursorData.GetFollwingSpriteRenderer().enabled)
                {
                    OnExecute();
                }
            }
        }
    }

    public override void OnExit(bool bExitCompletely)
    {
        base.OnExit(bExitCompletely);
        _bSetGoalProcess = false;
        _planetButtonController.SetToggleIsOn(1, false);
        UIManager.Instance.SetActiveAllChild(AttackSpawnTargetController.Instance.gameObject, false);
        _cursorData = null;
    }

    public virtual void SetGoalProcess(Vector3 spacecraftGoalPosition, RespawnPositionType respawnPositionType)
    {
        UIManager.Instance.SetActiveAllChild(AttackSpawnTargetController.Instance.gameObject, false);
        _goalData = new()
        {
            _spacecraftGoalPos = spacecraftGoalPosition,
            _respawnPositionType = respawnPositionType
        };
        _bSetGoalProcess = true;
    }

    protected void SetGoal(GameObject spacecraftPrefab, Goal goalData)
    {
        if (spacecraftPrefab.TryGetComponent(out SpacecraftController spacecraftController))
        {
            switch (goalData._respawnPositionType)
            {
                case RespawnPositionType.RespawnForward:
                {
                    spacecraftController.SetGoal(_startPoint.position, _endPoint.position, Vector3.zero, goalData,
                        _enemyNexusTarget.position);
                    break;
                }
                case RespawnPositionType.RespawnBackward:
                {
                    spacecraftController.SetGoal(_startPoint.position, _endPoint.position, Vector3.zero, goalData,
                        _enemyNexusTarget.position);
                    break;
                }
                case RespawnPositionType.RespawnLeft:
                {
                    spacecraftController.SetGoal(_startPoint.position, _endPoint.position, _leftMiddlePoint.position,
                        goalData, _enemyNexusTarget.position);
                    break;
                }
                case RespawnPositionType.RespawnRight:
                {
                    spacecraftController.SetGoal(_startPoint.position, _endPoint.position, _rightMiddlePoint.position,
                        goalData, _enemyNexusTarget.position);
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