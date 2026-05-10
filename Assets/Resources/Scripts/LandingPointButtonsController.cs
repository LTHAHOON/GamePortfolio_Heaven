using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandingPointButtonsController : MonoBehaviour
{
    [SerializeField]
    private ModeType _modeType;
    [SerializeField]
    private List<LandingPointButtonBinder> _landingPointBinders;
    
    public void SetUpButton()
    {
        if(_landingPointBinders.Count <= 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                bool bGet = transform.GetChild(i).TryGetComponent(out LandingPointButtonBinder binder);
                if (!bGet) continue;
                _landingPointBinders.Add(binder);
            }
        }
        foreach(LandingPointButtonBinder landingPointButton in _landingPointBinders)
        {
            landingPointButton.LandingPointButton.onClick.AddListener(() => OnClickSpawnPointButton(landingPointButton.LandingPointButton,
                                                                                            landingPointButton.GetRespawnDatsControl()));
        }
    }

    private void OnClickSpawnPointButton(Button button, LandingPointDatasController respawnDatasControl)
    {
        LandingPointData respawnPosData = respawnDatasControl.GetRandomRespawnData();
        int unUsedDataCount = respawnDatasControl.GetUnUsedRespawnDatasCount();
        if(unUsedDataCount <= 0)
        {
            button.interactable = false;
        }
        if (respawnPosData != null)
        {
            if(ModeButtonManager.Instance.CurStrategy is BaseDriveButtonController driveButtonController)
                driveButtonController.SetGoalProcess(respawnPosData);
        }
    }

    public ModeType ModeType => _modeType;
}
