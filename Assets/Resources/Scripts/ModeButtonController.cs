using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;


public abstract class ModeButtonController : MonoBehaviour, IStrategy
{
    [SerializeField]
    private ModeType _modeButtonType;
    [SerializeField]
    protected Button _thisButton;
    [Space(10f)]
    [Header("GuideText")]
    [Multiline]
    [SerializeField]
    protected string _guideText;
    [Space(10f)]
    [Header("planetButtonScript")]
    [SerializeField]
    protected PlanetButtonContrloller _planetButtonController;
    [SerializeField]
    protected CreateCountController _createCountController;
    [Space(10f)]
    [SerializeField]
    protected TextMeshProUGUI _buttonText;
    [SerializeField]
    protected Image _buttonImage;
    protected Unit _selectedUnitPrefab;
    public static event Action OnExitCompletely;

    protected void InitCreateCount(MPData mpData)
    {
        _createCountController.InitCreateCount(mpData, _guideText);
    }
    protected void InitCreateCount(MPData mpData,MPData subMpData)
    {
        _createCountController.InitCreateCount(mpData, _guideText, subMpData);
    }
    
    
    protected Transform GetSelectedUnitParentTransform(UnitType unitType)
    {
        MyUnitPrefabDataManager.Instance.TryGetChild(out GameObject instantiateParentObj, unitType);
        return instantiateParentObj.transform;
    }
    
    //OpenData, ReadyPrefab
    public virtual void OnEnter()
    {
        PlanetInternalPopController.OnClickOpenModeButton();
        //현재 유닛 버튼으로 생성할 유닛 프리팹 가져오기
        _selectedUnitPrefab = UnitButtonController.GetSelectedUnitPrefab();

        UIManager.Instance.SetActiveAllChild(_createCountController.gameObject, true);
    }

    public virtual void OnUpdate() { }

    //CreatePrefab
    public virtual void OnExecute() { }

    //CloseData
    public virtual void OnExit(bool bExitCompletely)
    {
        CursorManager.Instance.SetCursor(CursorType.Origin);
        if (_createCountController)
        {
            UIManager.Instance.SetActiveAllChild(_createCountController.gameObject, false);
        }
        _selectedUnitPrefab = null;
        if(bExitCompletely)
        {
            OnExitCompletely?.Invoke();
        }
    }

    public abstract void RefreshModeButton();

    public Button ThisButton => _thisButton;
    public void SetModeButtonType(ModeType modeType) => _modeButtonType = modeType;
    public ModeType ModeButtonType => _modeButtonType;
}
