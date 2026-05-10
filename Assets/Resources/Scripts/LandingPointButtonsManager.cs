using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class LandingPointButtonsManager : Singleton<LandingPointButtonsManager>
{
    [SerializeField] 
    private List<LandingPointButtonsController> _landingPointButtonsContrl;
    private Dictionary<int, LandingPointButtonsController> _dicLandingPointButtons = new Dictionary<int, LandingPointButtonsController>();
    [SerializeField] 
    private GameObject _landingPointControlParent;

    private void Awake()
    {
        for (int i = 0; i < _landingPointButtonsContrl.Count; i++)
        {
            _dicLandingPointButtons.Add((int)_landingPointButtonsContrl[i].ModeType, _landingPointButtonsContrl[i]);
            _landingPointButtonsContrl[i].SetUpButton();
        }
    }

    /// <summary>
    /// ModeType은 AttackDriveMode 또는 DefenseDriveMode 중 하나이어야 합니다.
    /// </summary>
    public void SetActiveLandingPointButtons(bool isActive, ModeType modeType)
    {
        _landingPointControlParent.gameObject.SetActive(isActive);
        if (_dicLandingPointButtons.TryGetValue((int)modeType, out LandingPointButtonsController landingPointButtonsControl))
        {
            landingPointButtonsControl.gameObject.SetActive(isActive);
        }
    }

}