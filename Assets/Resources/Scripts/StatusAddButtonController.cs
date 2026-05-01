using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StatusAddButtonController : MonoBehaviour
{
    [System.Serializable] 
    public struct AllStatusAddButtonInfo
    {
        public Button button;
        public AllStatusNames statusName;
    }
    [SerializeField]
    private List<AllStatusAddButtonInfo> _statusAddButtonList;
    public void Awake()
    {
        for (int i = 0; i < _statusAddButtonList.Count; i++)
        {
            //클로저 문제 해결 위해 statusName을 for문 안에서 새로 선언
            AllStatusNames statusName = _statusAddButtonList[i].statusName;
            _statusAddButtonList[i].button.onClick.AddListener(() => OnClickStatusAddButton(statusName));
        }

    }

    public void RefreshStatusAddButtons(List<ConsumeMPValue> consumeMPValues)
    {
        for (int i = 0; i < consumeMPValues.Count; i++)
        {
            bool canUseUpMP = MPDataController.Instance.CheckToUseUpMP(consumeMPValues[i].consumeMPValue);
            SetInteractableStatusAddButton(canUseUpMP, consumeMPValues[i].statusName);
        }
    }
    private void SetInteractableStatusAddButton(bool interactable, AllStatusNames statusName)
    {
        Button button = FindStatusAddButton(statusName);
        if(button != null)
        {
            button.interactable = interactable;
        }
    }

    private Button FindStatusAddButton(AllStatusNames statusName)
    {
        for (int i = 0; i < _statusAddButtonList.Count; i++)
        {
            if(_statusAddButtonList[i].statusName == statusName)
            {
                return _statusAddButtonList[i].button;
            }
        }
        return null;
    }
    public void OnClickStatusAddButton(AllStatusNames statusName)
    {
        StatusDataMng.Instance.AddStatus(StatusSliderController._status, statusName);
    }
}
