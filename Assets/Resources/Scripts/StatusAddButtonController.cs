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

    [SerializeField] private AllStatusAddButtonInfo[] _statusAddButtonList;
    private Dictionary<int, Button> _statusAddButtonDict = new();

    public void Awake()
    {
        for (int i = 0; i < _statusAddButtonList.Length; i++)
        {
            //클로저 문제 해결 위해 statusName을 for문 안에서 새로 선언
            AllStatusNames statusName = _statusAddButtonList[i].statusName;
            _statusAddButtonList[i].button.onClick.AddListener(() => OnClickStatusAddButton(statusName));
            _statusAddButtonDict.Add((int)statusName, _statusAddButtonList[i].button);
        }
    }

    public void RefreshStatusAddButtons()
    {
        List<StatusUnitMpData> statusMpDatas = MPDataManager.Instance.GetStatusMPDatas();
        for (int i = 0; i < statusMpDatas.Count; i++)
        {
            bool canUseUpMP = MPDataController.Instance.CheckToUseUpMP(statusMpDatas[i].MP_ConsValue);
            SetInteractableStatusAddButton(canUseUpMP, statusMpDatas[i].statusName);
        }
    }

    private void SetInteractableStatusAddButton(bool interactable, AllStatusNames statusName)
    {
        bool bGetButton = _statusAddButtonDict.TryGetValue((int)statusName, out Button button);
        if (bGetButton)
        {
            button.interactable = interactable;
        }
    }

    public void OnClickStatusAddButton(AllStatusNames statusName)
    {
        StatusManager.Instance.AddStatus(statusName);
    }
}