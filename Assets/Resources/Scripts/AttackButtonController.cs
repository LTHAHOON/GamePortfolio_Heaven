using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AttackButtonController : ModeButton
{
    [SerializeField]
    private LayerMask _outPlanetLayerMask;
    [Header("EnemyNexusTarget")]
    [SerializeField]
    private Transform _enemyNexusTarget;
    [Header("Next GuideText")]
    [Multiline]
    [SerializeField]
    private string _guideText_Next;
    [Header("InstancePos(Parent)")]
    [SerializeField]
    private GameObject _myUnitPrefab;
    [Space(10f)]
    [Header("MouseCursorScript")]
    [SerializeField]
    private MouseCursorController _mouseCursorController;
    [SerializeField]
    private GameObject _attackMark;
    [SerializeField]
    private Transform _mapMarkParent;
    private PoolComponent _pcAttackMark;
    private int _outPlanetLayer;

    public override void Awake()
    {
        base.Awake();
        PoolManager.Instance.AddPool(_attackMark, 3, 5, _mapMarkParent);
        _pcAttackMark = PoolManager.Instance.GetPool(_attackMark);
        _outPlanetLayer = (int)Mathf.Log(_outPlanetLayerMask.value, 2);
        bool hasOriginSpacecraftPrefab = _OriginSpacecraftPrefab.TryGetComponent(out MPComponent spacecraftMPC);
        if (hasOriginSpacecraftPrefab)
        {
            _originSpacecraftMpData = spacecraftMPC.GetMPData();
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PlanetInternalPopController.OnAttackModeOpen += OpenData;
        PlanetInternalPopController.OnAttackModeClose += CloseData;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PlanetInternalPopController.OnAttackModeOpen -= OpenData;
        PlanetInternalPopController.OnAttackModeClose -= CloseData;
    }

    private MouseCursorData _cursorData;
    private MPData _originSpacecraftMpData;
    private MPData _totalMPData = new();
    private void Update()
    {
        if (_bGetUnitPrefab && _unitMPData != null)
        {
            _totalMPData.MP_ConsValue = _originSpacecraftMpData.MP_ConsValue + _unitMPData.Value.MP_ConsValue;
            GameManager.Instance.UpdateButtonToMPData(_totalMPData, ref _thisButton, ref _buttonImage, ref _buttonText);
        }

        if (_bReadyPrefab && _unitPrefab)
        {
            _createCountController.RefreshCreateCount(_originSpacecraftMpData, _unitMPData);
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
                CreatePrefab();
            }
        }
    }

    private bool _bSetGoalProcess = false;
    private Goal _goalData;
    [SerializeField]
    private GameObject _OriginSpacecraftPrefab;
    public void ReadyPrefab()
    {
        _bReadyPrefab = true;
    }

    public void SetGoalProcess(Vector3 spacecraftGoalPosition, RespawnPositionType respawnPositionType)
    {
        UIManager.Instance.SetActiveAllChild(AttackSpawnTargetController.Instance.gameObject, false);
        _createCountController.SetActiveCount(true);
        _createCountController.ChangeGuideText(_guideText_Next);
        _goalData = new()
        {
            _spacecraftGoalPos = spacecraftGoalPosition,
            _respawnPositionType = respawnPositionType
        };
        _bSetGoalProcess = true;
    }
    private Vector3 _startPos = new Vector3(0f, 0f, -173.5f);
    public void CreatePrefab()
    {
        if (Input.GetMouseButtonDown(0) && _cursorData.GetFollwingSpriteRenderer().enabled)
        {
            _goalData._passengerGoalPos = _cursorData.GetFollwingSpriteRenderer().transform.position;
            GameObject attackMark = _pcAttackMark.PopPoolObject();
            attackMark.transform.position = _goalData._passengerGoalPos;
            Transform instantiateParent = GetSelectedUnitParentTransform(UnitType.Spacecraft);
            GameObject spacecraftPrefab = Instantiate(_OriginSpacecraftPrefab, instantiateParent);
            spacecraftPrefab.transform.position = _startPos;
            //TODO: _unitMPData의 소모량만큼 MP 소모하기
            MPController.Instance.UseUpMP(_unitMPData.Value.MP_ConsValue, _createCountController.GetCurCreateCount());
            MPController.Instance.UseUpMP(_originSpacecraftMpData.MP_ConsValue, 1);

            if (spacecraftPrefab.TryGetComponent(out SpacecraftController spacecraftController) &&
                _unitPrefab.TryGetComponent(out CreatureFSM fsm))
            {
                spacecraftController.GetLayerList().SetLayerList(spacecraftPrefab, true , _outPlanetLayer);
                MyUnitPrefabDataControl.Instance.AddUnitPrefabToList(UnitType.Spacecraft, spacecraftController);
                spacecraftController.GetCreateLoad().SetLoadReady(false);
                Transform creatureParent = GetSelectedUnitParentTransform(_selectedUnitType);
                spacecraftController.SetPassenger(fsm, _createCountController.GetCurCreateCount(), creatureParent);
                spacecraftController.SetReturnAttackMark(attackMark, ReturnAttackMark);
                Debug.Log(spacecraftController.GetPassengerCount(fsm.GetID()) + "탑승");
            }
            _bSetGoalProcess = false;
            //TODO: 목표 설정하기
            SetGoal(spacecraftPrefab, _goalData);
            PlanetInternalPopController.CloseMode(ModeType);
        }

    }

    public void ReturnAttackMark(GameObject attackMark)
    {
        _pcAttackMark.ReturnPoolObject(attackMark);
    }

    [SerializeField]
    private Transform _startPoint;
    [SerializeField]
    private Transform _leftMiddlePoint;
    [SerializeField]
    private Transform _rightMiddlePoint;
    [SerializeField]
    private Transform _endPoint;
    private void SetGoal(GameObject spacecraftPrefab, Goal goalData)
    {
        if (spacecraftPrefab.TryGetComponent(out SpacecraftController spacecraftController))
        {
            switch (goalData._respawnPositionType)
            {
                case RespawnPositionType.RespawnForward:
                    {
                        Vector3 euler = new Vector3(-90, -90, transform.rotation.z);
                        spacecraftController.SaveRotation(euler);
                        spacecraftController.SetGoal(_startPoint.position, _endPoint.position, Vector3.zero, goalData, _enemyNexusTarget.position);
                        break;
                    }
                case RespawnPositionType.RespawnBackward:
                    {
                        Vector3 euler = new Vector3(-90, 90, transform.rotation.z);
                        spacecraftController.SaveRotation(euler);
                        spacecraftController.SetGoal(_startPoint.position, _endPoint.position, Vector3.zero, goalData, _enemyNexusTarget.position);

                        break;
                    }
                case RespawnPositionType.RespawnLeft:
                    {
                        Vector3 euler = new Vector3(-90, 180, transform.rotation.z);
                        spacecraftController.SaveRotation(euler);
                        spacecraftController.SetGoal(_startPoint.position, _endPoint.position, _leftMiddlePoint.position, goalData, _enemyNexusTarget.position);

                        break;
                    }
                case RespawnPositionType.RespawnRight:
                    {
                        Vector3 euler = new Vector3(-90, 0, transform.rotation.z);
                        spacecraftController.SaveRotation(euler);
                        spacecraftController.SetGoal(_startPoint.position, _endPoint.position, _rightMiddlePoint.position, goalData, _enemyNexusTarget.position);

                        break;
                    }
            }
        }
    }
    protected override void OpenData()
    {
        _planetButtonController.SetToggleIsOn(1, true);
        UIManager.Instance.SetActiveAllChild(_createCountController.gameObject, true);
        InitCreateCount(_originSpacecraftMpData, _unitMPData.Value); //MPData로 생성 카운트 세팅(MPData 필요)
        _createCountController.SetActiveCount(false);
        UIManager.Instance.SetActiveAllChild(AttackSpawnTargetController.Instance.gameObject, true);
    }
    protected override void CloseData()
    {
        base.CloseData();
        _bSetGoalProcess = false;
        _planetButtonController.SetToggleIsOn(1, false);
        UIManager.Instance.SetActiveAllChild(AttackSpawnTargetController.Instance.gameObject, false);
        _cursorData = null;
    }
}
