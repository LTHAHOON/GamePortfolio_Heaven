using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HomeDecriptionMng : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _mpResumeAmountText;
    [SerializeField]
    private TextMeshProUGUI _extentText;
    [SerializeField]
    private TextMeshProUGUI _functionDescription;

    public void SetHomeDecription(GameObject selectedUnitButton)
    {
        if(selectedUnitButton.TryGetComponent(out UnitChipState unitChipState))
        {
            float horizontalLength = unitChipState.GetHorizontalLength();
            float verticaLength = unitChipState.GetVerticalLength();
            _extentText.text = $"면적: {verticaLength} X {horizontalLength}";

            string decription = unitChipState.GetFunctionDecription();
            _functionDescription.text = $"기능: {decription}";
        }
        if(selectedUnitButton.TryGetComponent(out MPComponent mpComponent))
        {
            _mpResumeAmountText.text = $"MP 소모량: {mpComponent.GetMPData().MP_ConsValue}";
        }
    }
}
