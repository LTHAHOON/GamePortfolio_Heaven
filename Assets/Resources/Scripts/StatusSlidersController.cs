using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum StatusSldiersType
{
    //넥서스같은 Slider가 필요없는 유닛
    None,
    //모든 Status가 들어간 타입
    Base,
    //힐 위주의 Status가 들어간 타입
    HealBase,
}

public class StatusSlidersController : MonoBehaviour
{
    [SerializeField]
    private StatusSldiersType _sliderType;
    [SerializeField]
    private Slider[] _arrStatusSlider;
    [SerializeField]
    private float _sliderSpeed;

    private bool _bSetStatus;
    private RuntimeUnitStatus _status;

    private void Update()
    {
         if (_bSetStatus && _status != null)
        {

            float targetValue = 0f;
            BaseUnitStatusData statusData = _status.GetBaseStatusData();
            AllStatusNames[] allStatusNames = statusData.GetAllStatusNames();
            for (int i = 0; i < _arrStatusSlider.Length; i++)
            {
                targetValue = FindStatusUsingSlider(allStatusNames[i]);
                _arrStatusSlider[i].value = Mathf.Lerp(_arrStatusSlider[i].value, targetValue, Time.deltaTime * _sliderSpeed);
            }

            for (int i = 0; i < _arrStatusSlider.Length; i++)
            {
                if (Mathf.Approximately(_arrStatusSlider[i].value, targetValue))
                {
                    _bSetStatus = false;
                }
                else
                {
                    _bSetStatus = true;
                    break;
                }
            }
        }
        
    }

    public void OnDisable()
    {
        for (int i = 0; i < _arrStatusSlider.Length; i++)
        {
            _arrStatusSlider[i].value = 0f;
        }
    }

    public void SetStatusSliders(long unitID)
    {
       _status = StatusManager.Instance.FindStatusData(unitID);
        ReloadStatus();
    }
    public void ReloadStatus()
    {
        if(_status != null)
        {
            _bSetStatus = true;
        }
    }
    public float FindStatusUsingSlider(AllStatusNames statusName)
    {
        return statusName switch
        {
            AllStatusNames.ATK => _status.ATK,
            AllStatusNames.DEF => _status.DEF,
            AllStatusNames.HIT => _status.HIT,
            AllStatusNames.Critical => _status.Critical,
            AllStatusNames.DEX => _status.DEX,
            AllStatusNames.CON => _status.CON,
            AllStatusNames.HealingPower => _status.HealingPower,
            _ => 0f,
        };
    }
    
    public StatusSldiersType SliderType => _sliderType;
}
