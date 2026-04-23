using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum ModeType
{
    AttackMode,
    DefenseMode,
    CreateMode,
    NONE
}
public abstract class ModeButton : MonoBehaviour
{
    [Space(10f)]
    [Header("GuideText")]
    [Multiline]
    [SerializeField]
    protected string _guideText;
    [Space(10f)]
    [Header("SelectedUnit")]
    [SerializeField]
    protected TextMeshProUGUI _selectedUnitName;
    [Space(10f)]
    [Header("ThisButton")]
    [SerializeField]
    protected Button _thisButton;
    [Space(10f)]
    [Header("planetButtonScript")]
    [SerializeField]
    protected PlanetButtonContrloller _planetButtonController;
    [SerializeField]
    protected CreateCountController _createCountController;
    [Space(10f)]
    [SerializeField]
    private ModeType _modeType;
    
    protected ModeType ModeType
    {
        set { _modeType = value; }
        get { return _modeType; }
    }
    protected TextMeshProUGUI _buttonText;
    protected Image _buttonImage;
    protected GameObject _unitPrefab;
    protected MPData? _unitMPData;
    protected static bool _bGetUnitPrefab = false;
    protected static UnitType _selectedUnitType;

    public virtual void Awake()
    {
        _buttonText = _thisButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _buttonImage = _thisButton.GetComponent<Image>();
    }
    public static void SetUnitPrefab(bool bGetUnitPrefab, UnitType unitType)
    {
        _selectedUnitType = unitType;
        _bGetUnitPrefab = bGetUnitPrefab;
    }
    public virtual void OnEnable()
    {
        if (_unitPrefab == null)
        {
            InitUnitWithMPData(); //유닛과 MPData 세팅
        }
        _bGetUnitPrefab = true;
    }
    public virtual void OnDisable()
    {
        _bGetUnitPrefab = false;
        _unitPrefab = null;
        _unitMPData = null;
    }
    public void OnClickOpenModeButton()
    {
        PlanetInternalPopController.OnClickOpenModeButton(_modeType);
    }

    protected void InitCreateCount(MPData unitMPData)
    {
        _createCountController.InitCreateCount(unitMPData, _guideText);
    }
    protected void InitCreateCount(MPData unitMPData,MPData subUnitMPData)
    {
        _createCountController.InitCreateCount(unitMPData, _guideText, subUnitMPData);
    }
    protected void InitUnitWithMPData()
    {
        StatusComponent status = StatusDataController.GetSelectedStatusComponent();
        UnitData unitData = status.GetUnitData();
        SpawnComponent spawnComponent = status.gameObject.GetComponent<SpawnComponent>();
        _unitPrefab = spawnComponent.GetSpawnPrefab(unitData.ID);
        if (_unitPrefab != null)
        {
            if (!GameManager.Instance.TryGetSelectedUnitMPData(out _unitMPData))
            {
                Debug.Log("[Error] TryGetSelectedUnitMPData()]");
            }
        }
    }
    protected Transform GetSelectedUnitParentTransform(UnitType unitType)
    {
        MyUnitPrefabDataControl.Instance.TryGetChild(out GameObject instantiateParentObj, unitType);
        return instantiateParentObj.transform;
    }
    protected virtual void OpenData()
    {
        UIManager.Instance.SetActiveAllChild(_createCountController.gameObject, true);
        InitCreateCount(_unitMPData.Value); //MPData로 생성 카운트 세팅(MPData 필요)
    }
    protected bool _bReadyPrefab = false;
    protected virtual void CloseData()
    {
        _bReadyPrefab = false;
        CursorManager.Instance.SetCursor(CursorType.Origin);
        UIManager.Instance.SetActiveAllChild(_createCountController.gameObject, false);
    }
}
