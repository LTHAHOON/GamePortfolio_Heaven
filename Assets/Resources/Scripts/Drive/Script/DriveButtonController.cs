using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class DriveButtonController : BaseDriveButtonController
{
    public static DriveButtonController Instance;
    [SerializeField] 
    private Transform _driveButtonParent;
    private readonly Dictionary<PassengerController, DriveButton> _dicDriveButton = new();
    private PoolComponent<DriveButton> _pcDriveButton;
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        if (!ThisButton.gameObject.TryGetComponent(out DriveButton driveButton))
            return;
        ModeButtonManager.Instance.AddModeButtonControl(this);
        PoolManager.Instance.AddPool(driveButton, 10, 20, _driveButtonParent);
        PoolManager.Instance.TryGetPool(driveButton, out _pcDriveButton);
    }

    public void AddDriveButton(PassengerController owner, int maxCount)
    {
        DriveButton driveButton = _pcDriveButton.PopPoolObject();
        if (!driveButton)
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
        if (_dicDriveButton.ContainsKey(owner))
        {
            _pcDriveButton.ReturnPoolObject(_dicDriveButton[owner]);
            ModeButtonManager.Instance.RemoveListenerModeButton(this, _dicDriveButton[owner].ThisButton);
            ObjectVisbilitySystem.Instance.RemoveToList(_dicDriveButton[owner], false);
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
        SetModeButtonType(_vehicleUnit.OppositeModeType);
    }
    public override void OnExecute()
    {
        base.OnExecute();
        RemoveDriveButton(_vehicleUnit);
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
