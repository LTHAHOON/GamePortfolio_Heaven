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

public class MPDataController : Singleton<MPDataController>
{
    [SerializeField]
    private List<ConsumeMPValue> _consumeMPValues;
    //MP StatusAddButton
    [SerializeField]
    private StatusAddButtonController _statusAddButtonController;
    [SerializeField]
    private TextMeshProUGUI MP_AmountText;
    [SerializeField]
    private Button MP_AddButton;

    public Slider MP_StatusSlider;
    public float CurrentMPValue { get; private set; }
    private float _mpMaxValue = 5000f;
    private void Awake()
    {
        MP_StatusSlider.maxValue = _mpMaxValue;
        MP_StatusSlider.value = _mpMaxValue;
        CurrentMPValue = _mpMaxValue;
        MP_AmountText.text = $"MP {CurrentMPValue}";

    }
    private void Update()
    {
        MP_StatusSlider.value = CurrentMPValue;
        if (MP_StatusSlider.value != _mpMaxValue)
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
        _curTime += Time.deltaTime;
        if (_curTime >= _delayOfTime)
        {
            CurrentMPValue += _autoFillInMPValue;
            CurrentMPValue = Mathf.Clamp(CurrentMPValue, 0, _mpMaxValue);
            MP_AmountText.text = $"MP {CurrentMPValue}";
            _curTime = 0;
            _statusAddButtonController.RefreshStatusAddButtons(_consumeMPValues);
        }
    }

    public bool CheckToUseUpMP(float consumptionMPValue)
    {
        return MP_StatusSlider.value >= consumptionMPValue;
    }

    public void UpdateButtonToMPData(MPData unitMPData, ref Button button, ref Image buttonImage, ref TextMeshProUGUI buttonText)
    {
        float mpValue = MP_StatusSlider.value;

        if (mpValue >= unitMPData.MP_ConsValue && button.interactable == false)
        {
            button.interactable = true;
            Color newButtonColor = UIManager.Instance.ChangeToInitialColor();
            Color newButtontextColor = UIManager.Instance.ChangeToInitialColor();

            buttonImage.color = newButtonColor;
            buttonText.color = newButtontextColor;
        }
        if (mpValue < unitMPData.MP_ConsValue && button.interactable == true)
        {
            button.interactable = false;
            Color newButtonColor = UIManager.Instance.ChangeToImageDisableColor();
            Color newButtontextColor = UIManager.Instance.ChangeToImageDisableColor();

            buttonImage.color = newButtonColor;
            buttonText.color = newButtontextColor;
        }
    }

    public bool TryGetConsumeMPValue(AllStatusNames statusName, out ConsumeMPValue consumeMPValue)
    {
        for (int i = 0; i < _consumeMPValues.Count; i++)
        {
            if (_consumeMPValues[i].statusName == statusName)
            {
                consumeMPValue = _consumeMPValues[i];
                return true;
            }
        }
        consumeMPValue = default;
        return false;
    }

    public bool UseUpMP(float consumptionMPValue, int count)
    {
        bool canUseUpMP = CheckToUseUpMP(consumptionMPValue);
        for(int i = 0; i < count; i++) 
        {
            if(canUseUpMP)
            {
                CurrentMPValue -= consumptionMPValue;
                MP_StatusSlider.value = CurrentMPValue;
                MP_AmountText.text = $"MP {CurrentMPValue}";
            }
        }
        _statusAddButtonController.RefreshStatusAddButtons(_consumeMPValues);
        return canUseUpMP;
        
    }

    public void OnClickMPAddButton()
    {
        UpdateMPStats();
        UseUpMP(_consumptionMPValue, 1);
        MP_StatusSlider.maxValue = _mpMaxValue;
    }

    private int _mpAddButtonCount = 0;
    private float _delayOfInitialTime = 5f;
    private float _consumptionMPValue;
    private void UpdateMPStats()
    {
        ++_mpAddButtonCount; 
        _delayOfTime = _delayOfInitialTime - _mpAddButtonCount;
        _delayOfTime = Mathf.Clamp(_delayOfTime, 0.5f, 5f);
        _mpMaxValue = 5000f + (_mpAddButtonCount * 1200f);
        _mpMaxValue = Mathf.Clamp(_mpMaxValue, 5000f, 15000f);
        _consumptionMPValue = (_mpAddButtonCount * 1200f);
    }

    
}
