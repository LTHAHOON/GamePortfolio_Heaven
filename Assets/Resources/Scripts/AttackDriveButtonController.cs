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
        _pcDestMark = PoolManager.Instance.GetPool(_attackMark);
    }

    public override void OnEnter()
    {
        PlanetInternalPopController.OnClickOpenModeButton();
        _selectedUnitPrefab = UnitButtonController.GetSelectedUnitPrefab();
        
        base.OnEnter();
    }

    public override void OnExecute()
    {
        GameObject destMark = _pcDestMark.PopPoolObject();
        if (!destMark) return;
        if (destMark.TryGetComponent(out ParticleColorSystem particleColorSystem))
        {
            Color color = UIManager.Instance.GetDestMarkColor(ModeType.AttackMode);
            particleColorSystem.ChangeParticleColor(color);
        }
        _goalData._passengerGoalPos = _cursorData.GetFollwingSpriteRenderer().transform.position;
        _goalData._passengerGoalPos.y += 1.5f;
        destMark.transform.position = _goalData._passengerGoalPos;

        Unit vehicleUnit = UnitSpawnManager.Instance.Spawn(_vehicleUnit);
        //MPData로 생성 카운트 세팅(MPData 필요)
        InitCreateCount(vehicleUnit.UnitMPInitData, _selectedUnitPrefab.UnitMPInitData);
        vehicleUnit.transform.position = _startPos;
        if (vehicleUnit is SpacecraftController spacecraftController) 
        {
            spacecraftController.GetCreateLoad().SetLoadReady(false);
            spacecraftController.SetDestMark(destMark, ReturnDestMark);
        }
        if(vehicleUnit is PassengerController passengerController && _selectedUnitPrefab.TryGetComponent(out IPassenger passenger))
        {
            passengerController.AddUnSpawnedPassenger(passenger, _createCountController.GetCurCreateCount());
            Debug.Log(passengerController.GetPassengerCountInData(_selectedUnitPrefab.ID) + "탑승");
        }
        //_unitMPData의 소모량만큼 MP 소모하기
        MPDataController.Instance.UseUpMP(_selectedUnitPrefab.UnitMPInitData, _createCountController.GetCurCreateCount());
        MPDataController.Instance.UseUpMP(vehicleUnit.UnitMPInitData, 1);


        StatusManager.Instance.TryAddStatusData(vehicleUnit.UnitInfo);
        _bSetGoalProcess = false;
        //목표 설정하기
        SetGoal(vehicleUnit.gameObject, _goalData);
        OnExit();
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!_vehicleUnit || _cursorData == null) 
            return;
        _createCountController.RefreshCreateCount(_vehicleUnit.UnitMPInitData, _selectedUnitPrefab.UnitMPInitData);
    }



    public override void RefreshModeButton()
    {
        Unit selectedUnitPrefab = UnitButtonController.GetSelectedUnitPrefab();
        if(!selectedUnitPrefab)
            return;
        MPData mpData = MPDataManager.Instance.FindUnitMPData(selectedUnitPrefab.ID);
        if(mpData == null)
            return;
        _totalMPData.SetMPConsValue(_vehicleUnit.UnitMPInitData.MP_ConsValue + mpData.MP_ConsValue);
        MPDataController.Instance.UpdateButtonToMPData(_totalMPData, ref _thisButton, ref _buttonImage,
            ref _buttonText);
    }

    public override void SetGoalProcess(LandingPointData respawnPosData)
    {
        base.SetGoalProcess(respawnPosData);
        _createCountController.SetActiveCount(true);
    }

}
