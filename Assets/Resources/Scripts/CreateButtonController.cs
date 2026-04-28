using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateButtonController : ModeButton
{
    [Header("InstancePos(Parent)")]
    [SerializeField]
    private GameObject _myUnitPrefab;

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

    float minX = 100f;
    float maxX = 1800f;
    float minY = 180f;
    float maxY = 1000f;


    public override void OnEnable()
    {
        base.OnEnable();
        PlanetInternalPopController.OnCreateModeOpen += OpenData;
        PlanetInternalPopController.OnCreateModeClose += CloseData;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PlanetInternalPopController.OnCreateModeOpen -= OpenData;
        PlanetInternalPopController.OnCreateModeClose -= CloseData;
        _unitPrefab = null;
        _unitMPData = null;
    }

    void Update()
    {
        if (_bGetUnitPrefab && _unitMPData != null)
        {
            StatusDataMng.Instance.UpdateButtonToMPData(_unitMPData.Value, ref _thisButton, ref _buttonImage, ref _buttonText);
        }

        if (_bReadyPrefab && _unitPrefab)
        {
            _createCountController.RefreshCreateCount(_unitMPData.Value);
            UnitPrefabFollowMouse();
            CreatePrefab();
        }
    }
    private Unit _curUnitPrefab;
    public override void ReadyPrefab()
    {
        TransparentMaterialControl.SurfaceType surfaceType = TransparentMaterialControl.SurfaceType.Transparent;
        Color changedColor = new Color32(255, 255, 255, 60);
        Transform instantiateParent = GetSelectedUnitParentTransform(_selectedUnitType);

        _curUnitPrefab = Instantiate(_unitPrefab, instantiateParent);
        _curUnitPrefab.transform.position = new Vector3(instantiateParent.position.x, _spawnComponent.SpawnHeight, instantiateParent.position.z);
        TransparentMaterialControl.SetQpaqueOrTransparentControl(_curUnitPrefab.gameObject, _selectedUnitType, surfaceType, changedColor);

        //TODO: GridBuildingContainer 열기
        ButtonSystem.buttonInvoker.PressButton(ButtonIdentifier.Open, _gridBuildingContainer);
        _bReadyPrefab = true;
    }


    private bool _isCreatableByPointerPos = true; // 마우스 포인터 위치에 의한 생성 가능 여부

    private void CreatePrefab()
    {
        
        if (Input.GetMouseButtonDown(0) && GridIndicatorController.GetIsCreatableUnit() && _isCreatableByPointerPos)
        {
            if (_curUnitPrefab.TryGetComponent(out BoxCollider boxCollider))
            {
                _placementSystem.AddPlaceUnitBoxCollider(boxCollider);
            }
            GridIndicatorController.SetIsCreatableUnit(false);
            

            _placementSystem.SetUnitIndicator(null);

            if (_curUnitPrefab.TryGetComponent(out SpacecraftController spacecraftControl))
            {
                spacecraftControl._isGravity = true;
                spacecraftControl.GetCreateLoad().SetLoadReady(true);
            }
            else if(_curUnitPrefab.TryGetComponent(out HomeController homeControl))
            {
                homeControl._isGravity = true;
                homeControl.GetCreateLoad().SetLoadReady(true);
            }
            else
            {
                Debug.Log("ERROR: SpacecraftController 또는 HomeController 컴포넌트가 없습니다.");
            }

            //TODO: _unitMPData의 소모량만큼 MP 소모하기
            MPController.Instance.UseUpMP(_unitMPData.Value.MP_ConsValue, 1);

            _curUnitPrefab = null;
            _bReadyPrefab = false;

            //TODO: 현재 생성할 갯수 하나 소모하기
            _createCountController.ConsumeCurCreateCount(1);
            if (_createCountController.GetCurCreateCount() >= 1)
            {
                ReadyPrefab();
            }
            else
            {
                PlanetInternalPopController.CloseMode(ModeType);
            }

        }
    }

    private void UnitPrefabFollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;

        if (mousePosition.x >= maxX || mousePosition.x <= minX || mousePosition.y >= maxY || mousePosition.y <= minY)
        {
            _mouseCursorController.ShowCursor(true);
            _isCreatableByPointerPos = false;
        }
        else
        {
            _mouseCursorController.ShowCursor(false);
            _isCreatableByPointerPos = true;
        }

        if(_placementSystem.GetUnitIndicator() == null)
        {
            _placementSystem.SetUnitIndicator(_curUnitPrefab.gameObject);
        }

        _placementSystem.CalculateIndicator(_curUnitPrefab.gameObject);
        _placementSystem.FollowIndicatorByMouse(_curUnitPrefab.gameObject);
    }
    protected override void OpenData()
    {
        base.OpenData();
        _planetButtonController.SetToggleIsOn(0, true);
    }
    protected override void CloseData()
    {
        base.CloseData();
        _planetButtonController.SetToggleIsOn(0, false);
        
        _placementSystem._isCellCalculate = false;
        _placementSystem.SetUnitIndicator(null);
        
        //TODO: GridBuildingContainer 닫기
        ButtonSystem.buttonInvoker.PressButton(ButtonIdentifier.Close, _gridBuildingContainer);

        if (_curUnitPrefab != null)
        {
            Destroy(_curUnitPrefab);

        }
    }
}
