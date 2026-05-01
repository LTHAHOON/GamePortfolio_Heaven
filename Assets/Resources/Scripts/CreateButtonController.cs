using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class CreateButtonController : ModeButtonController
{
    [SerializeField]
    private float _spawnHeight = 5f;

    [Space(10f)]
    [Header("MouseCursorScript")]
    [SerializeField]
    private MouseCursorController _mouseCursorController;

    [Space(10f)]
    [Header("GridBuildingContainer")]
    [SerializeField]
    private GameObject _gridBuildingContainer;

    [Space(10f)]
    [Header("PlacementSystem")]
    [SerializeField]
    private PlacementSystem _placementSystem;

    private bool _isCreatableByPointerPos = true; // 마우스 포인터 위치에 의한 생성 가능 여부
    private Unit _curSpawnedUnit;
    private TransparentMaterialControl.SurfaceType _transparentType = TransparentMaterialControl.SurfaceType.Transparent;
    private Color changedColor = new Color32(255, 255, 255, 60);
    public override void OnEnter()
    {
        base.OnEnter();
        _planetButtonController.SetToggleIsOn(0, true);
        _curSpawnedUnit = UnitSpawnManager.Instance.Spawn(_selectedUnitPrefab);
        _curSpawnedUnit.transform.position = new Vector3(0f, 5f ,0f);
        TransparentMaterialControl.SetQpaqueOrTransparentControl(_curSpawnedUnit.gameObject, _curSpawnedUnit.UnitType, _transparentType, changedColor);
        //TODO: GridBuildingContainer 열기
        ButtonSystem.buttonInvoker.PressButton(ButtonIdentifier.Open, _gridBuildingContainer);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_bGetUnit && _curSpawnedUnit)
        {
            MPDataController.Instance.UpdateButtonToMPData(_curSpawnedUnit.MPData, ref _thisButton, ref _buttonImage, ref _buttonText);
        }

        if (_bReadyUnit && _curSpawnedUnit)
        {
            _createCountController.RefreshCreateCount(_curSpawnedUnit.MPData);
            _isCreatableByPointerPos = _placementSystem.FollowUnitPrefabByMouse(_curSpawnedUnit);
            if (Input.GetMouseButtonDown(0) && GridIndicatorController.IsCreatableUnit && _isCreatableByPointerPos)
            {
                OnExecute();
            }
        }
    }
    public override void OnExecute()
    {
        base.OnExecute();

        if (_curSpawnedUnit.TryGetComponent(out BoxCollider boxCollider))
        {
            _placementSystem.AddPlaceUnitBoxCollider(boxCollider);
        }
        GridIndicatorController.SetIsCreatableUnit(false);


        _placementSystem.SetUnitIndicator(null);

        if (_curSpawnedUnit.TryGetComponent(out SpacecraftController spacecraftControl))
        {
            spacecraftControl._isGravity = true;
            spacecraftControl.GetCreateLoad().SetLoadReady(true);
        }
        else if (_curSpawnedUnit.TryGetComponent(out HomeController homeControl))
        {
            homeControl._isGravity = true;
            homeControl.GetCreateLoad().SetLoadReady(true);
        }
        else
        {
            Debug.Log("ERROR: SpacecraftController 또는 HomeController 컴포넌트가 없습니다.");
        }

        //TODO: _unitMPData의 소모량만큼 MP 소모하기
        MPDataController.Instance.UseUpMP(_curSpawnedUnit.MPData.MP_ConsValue, 1);

        //TODO: 현재 생성할 갯수 하나 소모하기
        _createCountController.ConsumeCurCreateCount(1);
        _curSpawnedUnit = UnitSpawnManager.Instance.Spawn(_selectedUnitPrefab);
        TransparentMaterialControl.SetQpaqueOrTransparentControl(_curSpawnedUnit.gameObject, _curSpawnedUnit.UnitType, _transparentType, changedColor);
        if (_createCountController.GetCurCreateCount() <= 0)
        {
            OnExit(true);
        }

    }
    
    public override void OnExit(bool bExitCompletely)
    {
        base.OnExit(bExitCompletely);
        _planetButtonController.SetToggleIsOn(0, false);

        _placementSystem._isCellCalculate = false;
        _placementSystem.SetUnitIndicator(null);
        //TODO: GridBuildingContainer 닫기
        ButtonSystem.buttonInvoker.PressButton(ButtonIdentifier.Close, _gridBuildingContainer);

        if (_curSpawnedUnit)
        {
            ObjectVisbilitySystem.Instance.RemoveToList(_curSpawnedUnit.GetHealth().HealthBar);
            Destroy(_curSpawnedUnit.gameObject);
        }
    }


}
