using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
#if UNITY_EDITOR
    [SerializeField]
    private bool editorFrameRateLock = false;
    [SerializeField]
    private int _fixedFrameRate = 30;
#endif

    void Awake()
    {
#if UNITY_EDITOR
        if (editorFrameRateLock)
        {
            Application.targetFrameRate = _fixedFrameRate;
        }
#endif

    }

    void Update()
    {
        
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
}
