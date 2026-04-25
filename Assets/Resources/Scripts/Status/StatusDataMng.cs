using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum AllStatusNames
{
    ATK, DEF, HIT, Critical, DEX, CON, HealingPower
};
[System.Serializable]
public struct ConsumeMPValue
{
    public AllStatusNames statusName;
    public int consumeMPValue;
}

public class StatusDataMng : Singleton<StatusDataMng>
{
    [SerializeField]
    private StatusAddButtonController _statusAddButtonController;
    private static Dictionary<GameObject, RuntimeUnitStatus> _statusDatas = new Dictionary<GameObject, RuntimeUnitStatus>();
    [SerializeField]
    private List<ConsumeMPValue> _consumeMPValues;
    public static bool AddStatusData(GameObject selectedUnitButton, StatusComponent statusComponent)
    {

        if (_statusDatas.ContainsKey(selectedUnitButton) == false)
        {
            _statusDatas.Add(selectedUnitButton, statusComponent.GetStatus());
            return true;
        }
        return false;
    }

    public void RefreshStatusAddButtons()
    {
        _statusAddButtonController.RefreshStatusAddButtons(_consumeMPValues);
    }
    
    private static readonly int _maxStatus = 20;
    private static readonly int _minStatus = 0;
    public void AddStatus(RuntimeUnitStatus status,AllStatusNames statusName)
    {
        bool hasConsumeMpValue = TryGetConsumeMPValue(statusName, out ConsumeMPValue consumeMPValue);
        if(hasConsumeMpValue)
        {
            bool isUseUPMP = MPController.Instance.UseUpMP(consumptionMPValue: consumeMPValue.consumeMPValue, count: 1);
            if (isUseUPMP)
            {
                int value = status.GetStatusValue(statusName);
                value = Mathf.Clamp(value + 1, _minStatus, _maxStatus);
                status.SetStatusValue(statusName, value);

                StatusSliderController.ReloadStatus();
            }
        }



    }

    public bool TryGetSelectedUnitMPData(out MPData? unitMPData)
    {
        if (SelectedUnitPopController.Instance._selectedUnitButton.TryGetComponent(out MPComponent mPComponent))
        {
            unitMPData = mPComponent.GetMPData();
            return true;
        }
        unitMPData = default;
        return false;
    }

    public void UpdateButtonToMPData(MPData unitMPData, ref Button button, ref Image buttonImage, ref TextMeshProUGUI buttonText)
    {
        float mpValue = MPController.Instance.MP_StatusSlider.value;

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

    public static RuntimeUnitStatus FindStatusData(GameObject selectedUnitButton)
    {
        if(_statusDatas.ContainsKey(selectedUnitButton))
        {
            return _statusDatas[selectedUnitButton];
        }
        else
        {
            return null;
        }
    }
}
