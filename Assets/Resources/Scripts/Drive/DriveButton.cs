using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DriveButton : MonoBehaviour, ICullingUI
{
    [Serializable]
    private struct DriveButtonData
    {
        public TextMeshProUGUI _driveText;
        public TextMeshProUGUI _countText;
        [HideInInspector]
        public int _curCount;
        [HideInInspector]
        public int _maxCount;
    }
    [SerializeField]
    private UIFollowObject _uiFollowObject;
    [SerializeField]
    private Button _driveButton;
    [SerializeField]
    private DriveButtonData _driveButtonData;
    private PassengerController _owner;

    private void LateUpdate()
    {
        if (!_owner)
            return;
        _uiFollowObject.FollowObject(Camera.main, _owner.gameObject, gameObject);
    }
    public void SetDriveCount(int curCount, int maxCount)
    {
        _driveButtonData._curCount = curCount;
        _driveButtonData._maxCount = maxCount;
        _driveButtonData._countText.text = $"{curCount}/{maxCount}";
        SetActiveText();
    }

    private void SetActiveText()
    {
        if (_driveButtonData._curCount == _driveButtonData._maxCount)
        {
            _driveButton.interactable = true;
            _driveButtonData._countText.gameObject.SetActive(false);
            _driveButtonData._driveText.gameObject.SetActive(true);

        }
        else
        {
            _driveButton.interactable = false;
            _driveButtonData._driveText.gameObject.SetActive(false);
            _driveButtonData._countText.gameObject.SetActive(true);
        }
    }
    public void RefreshModeButton(MPData totalMPData)
    {
        List<PassengerData> passengerDatas = _owner.GetUnSpawnedPassengers();
        float passengerTotalMP = 0f;
        for (int i = 0; i < passengerDatas.Count; i++)
        {
            MPData unitMpData = passengerDatas[i].Passenger.UnitMPData;
            passengerTotalMP += unitMpData.MP_ConsValue * passengerDatas[i].PassengerCount;
        }
        totalMPData.SetMPConsValue(_owner.UnitMPData.MP_ConsValue + passengerTotalMP);
        
        MPDataController.Instance.UpdateButtonToMPData(totalMPData, ref _driveButton);
    }
    public GameObject ThisGameObject => gameObject;
    public Collider ColliderForCulling => _owner.GetClickCollider();
    public bool IsForceHideUI => false;
    public Button ThisButton => _driveButton;
    public void SetOnClickDriveEvent(UnityAction<PassengerController> action) => _driveButton.onClick.AddListener(() => action?.Invoke(_owner));
    public void SetForceHideUI(bool isForceHide){}
    public void SetOwner(PassengerController owner) => _owner = owner;
}
