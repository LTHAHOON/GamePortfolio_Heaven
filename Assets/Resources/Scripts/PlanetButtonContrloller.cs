using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetButtonContrloller : MonoBehaviour
{
    [SerializeField]
    private SubCameraController _subCameraController;
    [SerializeField]
    private TextMeshProUGUI _planetName;

    [Header("ToggleInfo")]
    [SerializeField]
    private string[] _names = new string[2];
    [SerializeField]
    private Toggle[] _toggleButtons;

    private void Awake()
    {
        if(_toggleButtons.Length <= 0)
        {
            _toggleButtons = transform.GetComponentsInChildren<Toggle>();
        }
        _planetName.text = _names[0];
        currentToggle = _toggleButtons[0];
        currentToggle.interactable = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !_mode)
        {
            foreach (Toggle toggleButton in _toggleButtons)
            {
                if (toggleButton.isOn == false)
                {
                    toggleButton.isOn = true;
                    break;
                }
            }
            
        }

    }

    private bool _mode = false;
    public void SetToggleIsOn(int toggleIndex, bool mode)
    {
        _mode = mode;
        _toggleButtons[toggleIndex].isOn = true;
    }


    private Toggle currentToggle;
    public void OnClickPlanetButton(Toggle toggleButton)
    {
        if(currentToggle != toggleButton)
        {
            currentToggle.interactable = true;
            currentToggle.isOn = false;
        }
        if (toggleButton.isOn)
        {
            currentToggle = toggleButton;
            toggleButton.interactable = false;
            ChangePlanetText(toggleButton);
            _subCameraController.CameraMoveToOtherPlanet();
        }
    }

    private void ChangePlanetText(Toggle toggle)
    {
        if (toggle.gameObject.tag == "MyPlanet")
        {
            _planetName.text = _names[0];
        }
        else
        {
            _planetName.text = _names[1];
        }
    }



}
