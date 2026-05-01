using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public enum ModeType
{
    AttackMode,
    DefenseMode,
    CreateMode,
    NONE
}
public abstract class ModeButtonController : MonoBehaviour, IStrategy
{
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
    protected bool _bGetUnit = false;
    protected bool _bReadyUnit = false;
    public static event Action OnExitCompletely;

    protected void InitCreateCount(MPData unitMPData)
    {
        _createCountController.InitCreateCount(unitMPData, _guideText);
    }
    protected void InitCreateCount(MPData unitMPData,MPData subUnitMPData)
    {
        _createCountController.InitCreateCount(unitMPData, _guideText, subUnitMPData);
    }
    
    
    protected Transform GetSelectedUnitParentTransform(UnitType unitType)
    {
        MyUnitPrefabDataControl.Instance.TryGetChild(out GameObject instantiateParentObj, unitType);
        return instantiateParentObj.transform;
    }
    
    //OpenData, ReadyPrefab
    public virtual void OnEnter()
    {
        _bReadyUnit = true;
        PlanetInternalPopController.OnClickOpenModeButton();
        //현재 유닛 버튼으로 생성할 유닛 프리팹 가져오기
        _selectedUnitPrefab = UnitButtonController.GetSelectedUnitPrefab();
        InitCreateCount(_selectedUnitPrefab.MPData); //MPData로 생성 카운트 세팅(MPData 필요)
        UIManager.Instance.SetActiveAllChild(_createCountController.gameObject, true);
        _bGetUnit = true;
    }

    public virtual void OnUpdate() { }

    //CreatePrefab
    public virtual void OnExecute() { }

    //CloseData
    public virtual void OnExit(bool bExitCompletely)
    {
        _bReadyUnit = false;
        CursorManager.Instance.SetCursor(CursorType.Origin);
        UIManager.Instance.SetActiveAllChild(_createCountController.gameObject, false);
        _bGetUnit = false;
        _selectedUnitPrefab = null;
        if(bExitCompletely)
        {
            OnExitCompletely?.Invoke();
        }
    }

    public Button ThisButton => _thisButton;
}
