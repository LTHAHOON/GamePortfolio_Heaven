using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StatusSliderController : MonoBehaviour
{
    [SerializeField]
    private Slider[] _arrStatusSlider;

    [SerializeField]
    private float _sliderSpeed;


    void Update()
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

    private static bool _bSetStatus;
    public static RuntimeUnitStatus _status;
    public static void SetStatusSliders(long unitID)
    {
       _status = StatusDataMng.Instance.FindStatusData(unitID);
        ReloadStatus();
    }
    public static void ReloadStatus()
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
}
