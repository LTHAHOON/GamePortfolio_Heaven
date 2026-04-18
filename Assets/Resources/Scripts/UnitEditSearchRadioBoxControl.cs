using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitEditSearchRadioBoxControl : MonoBehaviour, IRadioBoxes
{
    private List<Toggle> _unitEditSearchRadioBoxes = new();

    public List<Toggle> GetRadioBoxList()
    {
        if (_unitEditSearchRadioBoxes.Count <= 0)
        {
            SetRadioBoxList();
        }

        return _unitEditSearchRadioBoxes;
    }

    public void SetRadioBoxList()
    {
        if (_unitEditSearchRadioBoxes.Count <= 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out Toggle toggle))
                {
                    _unitEditSearchRadioBoxes.Add(toggle);
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

    private void Update()
    {
        UpdateToggleInteractable();
    }

    private void UpdateToggleInteractable()
    {
        if(_unitEditSearchRadioBoxes.Count > 0)
        {
            foreach(var toggle in _unitEditSearchRadioBoxes)
            {
                if (toggle.isOn) { toggle.interactable = false; }
                else { toggle.interactable = true; }
            }
        }
        else
        {
            Debug.Log("Null: _unitEditSearchRadioBoxes");
        }
    }

    
}
