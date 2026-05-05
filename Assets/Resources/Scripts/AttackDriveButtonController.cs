using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class AttackDriveButtonController : BaseDriveButtonController
{
   
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
        
        base.OnEnter();
    }

    public override void OnExecute()
    {
        Unit vehicleUnit = UnitSpawnManager.Instance.Spawn(_vehicleUnit);
        InitCreateCount(vehicleUnit.UnitMPData, _selectedUnitPrefab.UnitMPData); //MPData로 생성 카운트 세팅(MPData 필요)
        _goalData._passengerGoalPos = _cursorData.GetFollwingSpriteRenderer().transform.position;
        GameObject attackMark = _pcAttackMark.PopPoolObject();
        attackMark.transform.position = _goalData._passengerGoalPos;

        vehicleUnit.transform.position = _startPos;
        if (vehicleUnit is SpacecraftController spacecraftController) 
        {
            spacecraftController.GetCreateLoad().SetLoadReady(false);
            spacecraftController.SetModeTypeForBoardingStaData(ModeType.AttackDriveMode);
            spacecraftController.SetAttackMark(attackMark, ReturnAttackMark);
        }
        if(vehicleUnit is PassengerController passengerController && _selectedUnitPrefab.TryGetComponent(out IPassenger passenger))
        {
            passengerController.AddUnSpawnedPassenger(passenger, _createCountController.GetCurCreateCount());
            Debug.Log(passengerController.GetPassengerCountInData(_selectedUnitPrefab.ID) + "탑승");
        }
        //_unitMPData의 소모량만큼 MP 소모하기
        MPDataController.Instance.UseUpMP(_selectedUnitPrefab.UnitMPData, _createCountController.GetCurCreateCount());
        MPDataController.Instance.UseUpMP(vehicleUnit.UnitMPData, 1);


        StatusManager.Instance.TryAddStatusData(vehicleUnit.UnitInfo);
        _bSetGoalProcess = false;
        //목표 설정하기
        SetGoal(vehicleUnit.gameObject, _goalData);
        OnExit(true);
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!_vehicleUnit || _cursorData == null) 
            return;
        _createCountController.RefreshCreateCount(_vehicleUnit.UnitMPData, _selectedUnitPrefab.UnitMPData);
    }



    public override void RefreshModeButton()
    {
        Unit selectedUnitPrefab = UnitButtonController.GetSelectedUnitPrefab();
        if(!selectedUnitPrefab)
            return;
        MPData mpData = MPDataManager.Instance.FindUnitMPData(selectedUnitPrefab.ID);
        if(mpData == null)
            return;
        _totalMPData.SetMPConsValue(_vehicleUnit.UnitMPData.MP_ConsValue + mpData.MP_ConsValue);
        MPDataController.Instance.UpdateButtonToMPData(_totalMPData, ref _thisButton, ref _buttonImage,
            ref _buttonText);
    }

    public override void SetGoalProcess(Vector3 spacecraftGoalPosition, RespawnPositionType respawnPositionType)
    {
        base.SetGoalProcess(spacecraftGoalPosition, respawnPositionType);
        _createCountController.SetActiveCount(true);
    }

}
