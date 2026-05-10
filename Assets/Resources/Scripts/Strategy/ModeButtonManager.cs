using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum ModeType
{
    AttackMode,
    DefenseMode,
    None,
}

public class ModeButtonManager : Singleton<ModeButtonManager>
{
    [SerializeField]
    private List<ModeButtonController> _modeButtonControlList;
    private IStrategy _curStrategy;

    private void Awake()
    {
        ModeButtonController.OnExitCompletely += OnExitModeCompletely;
        for (int i = 0; i < _modeButtonControlList.Count; i++)
        {
            AddListenerModeButton(_modeButtonControlList[i], _modeButtonControlList[i].ThisButton);
        }
    }

    public void AddModeButtonControl(ModeButtonController modeButtonControl)
    {
        if(!_modeButtonControlList.Contains(modeButtonControl))
        {
            _modeButtonControlList.Add(modeButtonControl);
        }
    }

    public void RemoveModeButtonControl(ModeButtonController modeButtonControl)
    {
        _modeButtonControlList.Remove(modeButtonControl);
    }

    public void AddListenerModeButton(ModeButtonController modeButtonControl, Button modeButton)
    {
        modeButton.onClick.AddListener(() => OnClickModeButton(modeButtonControl));
    }
    public void RemoveListenerModeButton(ModeButtonController modeButtonControl, Button modeButton)
    {
        modeButton.onClick.RemoveListener(() => OnClickModeButton(modeButtonControl));
    }


    public void OnExitModeCompletely()
    {
        _isUpdateMode = false;
        _curStrategy = null;
    }

    private bool _isUpdateMode;
    private void OnClickModeButton(IStrategy strategy)
    {
        if (strategy == null)
            return;
        if (strategy is not ModeButtonController modeButtonController)
            return;
        _curStrategy = strategy;
        _curStrategy.OnEnter();
        _isUpdateMode = true;
    }

    private void Update()
    {
        if (!_isUpdateMode || _curStrategy == null)
            return;
        _curStrategy.OnUpdate();
        //������ �ʱ�ȭ
        if((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.BackQuote)))
        {
            _curStrategy.OnExit(false);
        }
        //���� ������ Exit
        if(Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.BackQuote))
        {
            OnExitModeCompletely();
        }
    }
    
    public void RefreshModeButtons()
    {
        for (int i = 0; i < _modeButtonControlList.Count; i++)
        {
            _modeButtonControlList[i].RefreshModeButton();
        }
    }
    public IStrategy CurStrategy => _curStrategy;
    public bool IsUpdateMode => _isUpdateMode;
}
