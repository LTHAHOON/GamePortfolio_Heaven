using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitChipSearchRadioBoxControl : MonoBehaviour, IRadioBoxes
{
    private List<Toggle> _unitChipSearchRadioBoxes = new();

    public List<Toggle> GetRadioBoxList()
    {
        if (_unitChipSearchRadioBoxes.Count <= 0)
        {
            SetRadioBoxList();
        }

        return _unitChipSearchRadioBoxes;
    }

    public void SetRadioBoxList()
    {
        if (_unitChipSearchRadioBoxes.Count <= 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out Toggle toggle))
                {
                    _unitChipSearchRadioBoxes.Add(toggle);
                }
                else
                {
                    Debug.Log("ERROR: Not existed Toggle");
                }
            }
        }

            
    }


    private void Awake()
    {
        SetRadioBoxList();
    }


    void Update()
    {
        UpdateRadioTextColor();
    }

    private void UpdateRadioTextColor()
    {
        if(_unitChipSearchRadioBoxes.Count > 0)
        {
            for (int i = 0; i < _unitChipSearchRadioBoxes.Count; i++)
            {
                TextMeshProUGUI radioText = _unitChipSearchRadioBoxes[i].transform.Find("RadioText").GetComponent<TextMeshProUGUI>();

                if (_unitChipSearchRadioBoxes[i].isOn == true)
                {
                    ChangeRadioTextColor(radioText, Color.white);
                }
                if (_unitChipSearchRadioBoxes[i].isOn == false && radioText.color == Color.white)
                {
                    ChangeRadioTextColor(radioText, Color.black);
                }
            }
        }
        else
        {
            Debug.Log("Null: _unitChipSearchRadioBoxes");
        }
    }


    private void ChangeRadioTextColor(TextMeshProUGUI radioText, Color colorToChange)
    {
        radioText.color = colorToChange;
    }

}
