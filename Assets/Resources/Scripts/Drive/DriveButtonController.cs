using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DriveButtonController : BaseDriveButtonController
{
    public static DriveButtonController Instance;
    [SerializeField] 
    private Transform _driveButtonParent;
    private readonly Dictionary<PassengerController, DriveButton> _dicDriveButton = new();
    private PoolComponent _pcDriveButton;
    protected virtual void Awake()
    {
        Instance = this;
        ModeButtonManager.Instance.AddModeButtonControl(this);
        PoolManager.Instance.AddPool(ThisButton.gameObject, 10, 20, _driveButtonParent);
        _pcDriveButton = PoolManager.Instance.GetPool(ThisButton.gameObject);
    }

    public void AddDriveButton(PassengerController owner, int maxCount)
    {
        GameObject driveButtonObj = _pcDriveButton.PopPoolObject();
        if (!driveButtonObj.TryGetComponent(out DriveButton driveButton))
            return;
        driveButton.SetOwner(owner);
        driveButton.SetOnClickDriveEvent(OnClickDrive);
        driveButton.SetDriveCount(0, maxCount);
        _dicDriveButton.Add(owner, driveButton);
        ModeButtonManager.Instance.AddListenerModeButton(this, driveButton.ThisButton);
        ObjectVisbilitySystem.Instance.AddToList(driveButton);
    }
    public void RemoveDriveButton(PassengerController owner)
    {
        ModeButtonManager.Instance.RemoveListenerModeButton(this, _dicDriveButton[owner].ThisButton);
        _pcDriveButton.ReturnPoolObject(_dicDriveButton[owner].gameObject);
        if (_dicDriveButton.ContainsKey(owner))
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
        SetVehicleUnit(owner);
        RemoveDriveButton(owner);
    }

    public override void RefreshModeButton()
    {
        for (int i = 0; i < _dicDriveButton.Count; i++)
        {
            _dicDriveButton.ElementAt(i).Value.RefreshModeButton(_totalMPData);
        }
    }

    public bool IsContainDriveButton(PassengerController owner) => _dicDriveButton.ContainsKey(owner);
}
