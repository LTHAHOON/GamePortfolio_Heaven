using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandingPointButtonBinder : MonoBehaviour
{
    [SerializeField]
    private Button _landingPointButton;
    [SerializeField]
    private LandingPointDatasController _ladingPointDatasController;

    public Button LandingPointButton => _landingPointButton;
    public LandingPointDatasController GetRespawnDatsControl()
    {
        return _ladingPointDatasController;
    }
}
