using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveButtonController : Singleton<DriveButtonController>
{
    [SerializeField]
    private DriveButton _driveButtonPrefab;
    [SerializeField]
    private Dictionary<PassengerController, DriveButton> _dicDriveButton = new();
    public void AddDriveButton(PassengerController owner, int maxCount)
    {
        DriveButton driveButton = Instantiate(_driveButtonPrefab, transform);
        driveButton.SetOwner(owner);
        driveButton.SetOnClickDriveEvent(OnClickDrive);
        driveButton.SetDriveCount(0, maxCount);
        _dicDriveButton.Add(owner, driveButton);
        ObjectVisbilitySystem.Instance.AddToList(driveButton);
    }
    public void RemoveDriveButton(PassengerController owner)
    {
        if(_dicDriveButton.ContainsKey(owner))
        {
            _dicDriveButton.Remove(owner);
        }
    }
    public DriveButton GetDriveButton(PassengerController owner)
    {
        return _dicDriveButton.GetValueOrDefault(owner);
    }
    private void OnClickDrive(PassengerController owner)
    {
        AttackButtonController.Instance.ReadyPrefab();
        AttackButtonController.Instance.OnClickOpenModeButton();
        RemoveDriveButton(owner);
    }
}
