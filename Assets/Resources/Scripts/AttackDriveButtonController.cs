using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AttackDriveButtonController : BaseDriveButtonController
{
    [Header("Next GuideText")]
    [Multiline]
    [SerializeField]
    private string _guideText_Next;
    [SerializeField]
    private PassengerController _spacecraftPrefab;
    
    protected void Awake()
    {
        SetVehicleUnit(_spacecraftPrefab);
        PoolManager.Instance.AddPool(_attackMark, 3, 5, _mapMarkParent);
        _pcAttackMark = PoolManager.Instance.GetPool(_attackMark);
    }

    public override void OnEnter()
    {
        PlanetInternalPopController.OnClickOpenModeButton();
        _selectedUnitPrefab = UnitButtonController.GetSelectedUnitPrefab();

        UIManager.Instance.SetActiveAllChild(_createCountController.gameObject, true);
        InitCreateCount(_vehicleUnit.MPData, _selectedUnitPrefab.MPData); //MPData로 생성 카운트 세팅(MPData 필요)
        _createCountController.SetActiveCount(false);
        base.OnEnter();
    }
    

    public override void OnExecute()
    {
        Unit vehicleUnit = UnitSpawnManager.Instance.Spawn(_vehicleUnit);
        _goalData._passengerGoalPos = _cursorData.GetFollwingSpriteRenderer().transform.position;
        GameObject attackMark = _pcAttackMark.PopPoolObject();
        attackMark.transform.position = _goalData._passengerGoalPos;

        vehicleUnit.transform.position = _startPos;
        if (vehicleUnit.TryGetComponent(out SpacecraftController spacecraftController) &&
            _selectedUnitPrefab.TryGetComponent(out Creature creature))
        {
            spacecraftController.GetCreateLoad().SetLoadReady(false);
            Transform creatureParent = GetSelectedUnitParentTransform(creature.UnitType);
            spacecraftController.AddPassenger(creature, _createCountController.GetCurCreateCount(), creatureParent);
            spacecraftController.SetAttackMark(attackMark, ReturnAttackMark);
            Debug.Log(spacecraftController.GetPassengerCount(creature.ID) + "탑승");
        }
        //_unitMPData의 소모량만큼 MP 소모하기
        MPDataController.Instance.UseUpMP(_selectedUnitPrefab.MPData.MP_ConsValue, _createCountController.GetCurCreateCount());
        MPDataController.Instance.UseUpMP(_vehicleUnit.MPData.MP_ConsValue, 1);
        
        vehicleUnit.StatusComponent.InitializeStatus(vehicleUnit.UnitInfo);
        StatusDataMng.Instance.AddStatusData(vehicleUnit.ID, vehicleUnit.StatusComponent);
        _bSetGoalProcess = false;
        //목표 설정하기
        SetGoal(vehicleUnit.gameObject, _goalData);
        OnExit(true);
    }

    public override void SetGoalProcess(Vector3 spacecraftGoalPosition, RespawnPositionType respawnPositionType)
    {
        base.SetGoalProcess(spacecraftGoalPosition, respawnPositionType);
        _createCountController.SetActiveCount(true);
        _createCountController.ChangeGuideText(_guideText_Next);
    }

}
