using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class CreateButtonController : ModeButtonController
{
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
        InitCreateCount(_curSpawnedUnit.UnitMPInitData); //MPData로 생성 카운트 세팅(MPData 필요)
        TransparentMaterialControl.SetQpaqueOrTransparentControl(_curSpawnedUnit.gameObject, _curSpawnedUnit.UnitType, _transparentType, changedColor);
        //TODO: GridBuildingContainer 열기
        ButtonSystem.buttonInvoker.PressButton(ButtonIdentifier.Open, _gridBuildingContainer);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!_curSpawnedUnit)
            return;

        _createCountController.RefreshCreateCount(_curSpawnedUnit.UnitMPInitData);
        _isCreatableByPointerPos = _placementSystem.FollowUnitPrefabByMouse(_curSpawnedUnit);
        if (Input.GetMouseButtonDown(0) && GridIndicatorController.IsCreatableUnit && _isCreatableByPointerPos)
        {
            OnExecute();
        }
    }
    public override void OnExecute()
    {
        base.OnExecute();

        #region PlacementSystem에 Collider 정보 추가(꼭짓점 위치로 Bound에 포함이 되는지 비교하기 위함)
        if (_curSpawnedUnit.TryGetComponent(out BoxCollider boxCollider))
        {
            _placementSystem.AddPlaceUnitBoxCollider(boxCollider);
        }
        #endregion

        #region PlacementSystem 초기화
        GridIndicatorController.SetIsCreatableUnit(false);
        _placementSystem.SetUnitIndicator(null);
        #endregion

        if (_curSpawnedUnit is SpacecraftController spacecraftControl)
        {
            spacecraftControl._isGravity = true;
            spacecraftControl.GetCreateLoad().SetLoadReady(true);
        }
        else if (_curSpawnedUnit is HomeController homeControl)
        {
            homeControl._isGravity = true;
            homeControl.GetCreateLoad().SetLoadReady(true);
        }
        else
        {
            Debug.Log("ERROR: SpacecraftController 또는 HomeController 컴포넌트가 없습니다.");
        }

        //TODO: _unitMPData의 소모량만큼 MP 소모하기
        MPDataController.Instance.UseUpMP(_curSpawnedUnit.UnitMPInitData, 1);

        //TODO: 현재 생성할 갯수 하나 소모하기
        _createCountController.ConsumeCurCreateCount(1);
        _curSpawnedUnit = UnitSpawnManager.Instance.Spawn(_selectedUnitPrefab);
        TransparentMaterialControl.SetQpaqueOrTransparentControl(_curSpawnedUnit.gameObject, _curSpawnedUnit.UnitType, _transparentType, changedColor);
        if (_createCountController.GetCurCreateCount() <= 0)
        {
            OnExit();
        }

    }
    
    public override void OnExit()
    {
        base.OnExit();
        _planetButtonController.SetToggleIsOn(0, false);
        _placementSystem._isCellCalculate = false;
        _placementSystem.SetUnitIndicator(null);
        //TODO: GridBuildingContainer 닫기
        ButtonSystem.buttonInvoker.PressButton(ButtonIdentifier.Close, _gridBuildingContainer);

        if (_curSpawnedUnit)
        {
            ObjectVisbilitySystem.Instance.RemoveToList(_curSpawnedUnit.GetHealth().HealthBar);
            MyUnitPrefabDataManager.Instance.RemoveUnitPrefabToList(_curSpawnedUnit.UnitType, _curSpawnedUnit);
        }
    }

    public override void RefreshModeButton()
    {
        Unit selectedUnitPrefab = UnitButtonController.GetSelectedUnitPrefab();
        if(!selectedUnitPrefab)
            return;
        MPData mpData = MPDataManager.Instance.FindUnitMPData(selectedUnitPrefab.ID);
        if(mpData == null)
            return;
        MPDataController.Instance.UpdateButtonToMPData(mpData, ref _thisButton, ref _buttonImage, ref _buttonText);
    }
}
