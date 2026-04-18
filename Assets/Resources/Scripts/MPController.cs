using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class MPController : Singleton<MPController>
{
    [SerializeField]
    private StatusAddButtonController _statusAddButtonController;
    [SerializeField]
    private TextMeshProUGUI MP_AmountText;
    [SerializeField]
    private Button MP_AddButton;

    public Slider MP_StatusSlider;
    private float _mpMaxValue = 5000f;
    private void FixedUpdate()
    {
        if(MP_StatusSlider.value != _mpMaxValue)
        {
            AutoFillInMP();
        }
        if (MP_StatusSlider.value >= _mpMaxValue/ 1.5f)
        {
            MP_AddButton.interactable = true;
        }
        else
        {
            MP_AddButton.interactable = false;
        }
    }


    private float _delayOfTime = 5f;
    private float _curTime = 0;

    private float _autoFillInMPValue = 200f;
    private void AutoFillInMP()
    {
        _curTime += Time.fixedDeltaTime;
        if (_curTime >= _delayOfTime)
        {
            MP_StatusSlider.value += _autoFillInMPValue;
            MP_AmountText.text = $"MP {MP_StatusSlider.value}";
            _curTime = 0;
            StatusDataMng.Instance.RefreshStatusAddButtons();
        }
    }

    public bool CheckToUseUpMP(float consumptionMPValue)
    {
        if (MP_StatusSlider.value >= consumptionMPValue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public bool UseUpMP(float consumptionMPValue, int count)
    {
        bool canUseUpMP = CheckToUseUpMP(consumptionMPValue);
        for(int i = 0; i < count; i++) 
        {
            if(canUseUpMP)
            {
                MP_StatusSlider.value -= consumptionMPValue;
                MP_AmountText.text = $"MP {MP_StatusSlider.value}";
            }
        }
        StatusDataMng.Instance.RefreshStatusAddButtons();
        return canUseUpMP;
        
    }

    public void OnClickMPAddButton()
    {
        Task task = Task.Factory.StartNew(()=> MPTask());
        task.Wait();
        UseUpMP(_consumptionMPValue, 1);
        MP_StatusSlider.maxValue = _mpMaxValue;
    }
    private int _mpAddButtonCount = 0;
    private float _delayOfInitialTime = 5f;
    private float _consumptionMPValue;
    private void MPTask()
    {
        ++_mpAddButtonCount; 
        _delayOfTime = _delayOfInitialTime - _mpAddButtonCount;
        _delayOfTime = Mathf.Clamp(_delayOfTime, 0.5f, 5f);
        _mpMaxValue = 5000f + (_mpAddButtonCount * 1200f);
        _mpMaxValue = Mathf.Clamp(_mpMaxValue, 5000f, 15000f);
        _consumptionMPValue = (_mpAddButtonCount * 1200f);
    }

    
}
