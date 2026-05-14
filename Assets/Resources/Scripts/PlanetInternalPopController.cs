using Cinemachine;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class PlanetInternalPopController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _mainVcam;
    [SerializeField]
    private CinemachineVirtualCamera _subVcam;
    private bool _isDoorOpened;
    [SerializeField]
    private GameObject _miniMap_Space;

    [SerializeField]
    private RectMask2D _spaceHUDMask;
    [SerializeField]
    private GameObject _planetPopUpCanvas;
    private Transform _planetCloseButton;
    private Transform _planetOpenButton;
    private static Action OnClickPlanetOpen;
    private static Action OnClickPlanetClose;
    public static Transform _planetInternal;
    void Awake()
    {
        OnClickPlanetOpen = OnClickPlanetOpenButton;
        OnClickPlanetClose = OnClickPlanetCloseButton;
    }
    void Start()
    {
        _planetCloseButton = _planetPopUpCanvas.transform.Find("PlanetCloseButton");
        _planetOpenButton = _planetPopUpCanvas.transform.Find("PlanetOpenButton");
        _planetInternal = transform.Find("PlanetInternal");
    }

    void Update()
    {
        //�ݱ� ����Ű ��ħ ����
        if (ModeButtonManager.Instance.IsUpdateMode)
            return;

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (_isDoorOpened)
            {
                OnClickPlanetCloseButton();
            }
            else
            {
                OnClickPlanetOpenButton();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickPlanetCloseButton();
        }

    }

    public void SetInteractaleOfPlanetOpenButton(bool interactable)
    {
        if (_planetOpenButton.gameObject.TryGetComponent(out Button button))
        {
            button.interactable = interactable;
        }
        if (_planetOpenButton.gameObject.TryGetComponent(out Image image))
        {
            image.raycastTarget = interactable;
        }
    }
    public static void OnClickOpenModeButton()
    {
        OnClickPlanetOpen?.Invoke();
    }
    public static void OnClickCloseModeButton()
    {
        OnClickPlanetClose?.Invoke();
    }

    public void OnClickPlanetOpenButton()
    {
        _spaceHUDMask.enabled = true;
        CullingMaskExtension.ChangeVirtualCamera(_mainVcam, _subVcam);
        _isDoorOpened = true;
        _miniMap_Space.SetActive(false);
        _planetCloseButton.gameObject.SetActive(true);
        _planetOpenButton.gameObject.SetActive(false);
        _planetInternal.gameObject.SetActive(true);
    }
    public void OnClickPlanetCloseButton()
    {
        if (UIManager.Instance.IsSubCameraActive)
        {
            _spaceHUDMask.enabled = false;
            CullingMaskExtension.ChangeVirtualCamera(_subVcam, _mainVcam);
            _isDoorOpened = false;
            _miniMap_Space.SetActive(true);
            _planetCloseButton.gameObject.SetActive(false);
            _planetOpenButton.gameObject.SetActive(true);
            _planetInternal.gameObject.SetActive(false);
        }
    }

}
