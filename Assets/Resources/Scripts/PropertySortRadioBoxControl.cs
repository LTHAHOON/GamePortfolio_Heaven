using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertySortRadioBoxControl : MonoBehaviour, IRadioBoxes
{
    private List<Toggle> _propertySortRadioBoxes = new();

    public List<Toggle> GetRadioBoxList()
    {
        if (_propertySortRadioBoxes.Count <= 0)
        {
            SetRadioBoxList();
        }

        return _propertySortRadioBoxes;
    }

    public void SetRadioBoxList()
    {
        if (_propertySortRadioBoxes.Count <= 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out Toggle toggle))
                {
                    _propertySortRadioBoxes.Add(toggle);
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
}
