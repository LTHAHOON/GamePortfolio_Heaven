using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class PlanetInternalPopController : MonoBehaviour
{

    private bool _isDoorOpened;
    [SerializeField]
    private Image _planetHpSituationImage;

    [SerializeField]
    private GameObject _miniMap_Space;

    [SerializeField]
    private GameObject _planetPopUpCanvas;
    private Transform _planetCloseButton;
    private Transform _planetOpenButton;
    public static Transform _planetInternal;
    void Awake()
    {
        OnClickPlaentOpen += OnClickPlanetOpenButton;
    }
    void Start()
    {
        _planetCloseButton = _planetPopUpCanvas.transform.Find("PlanetCloseButton");
        _planetOpenButton = _planetPopUpCanvas.transform.Find("PlanetOpenButton");
        _planetInternal = transform.Find("PlanetInternal");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote) && _mode == ModeType.NONE)
        {
            if (_isDoorOpened)
            {
                OnClickPlanetCloseButton(_mode);
            }
            else
            {
                OnClickPlanetOpenButton(_mode);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.BackQuote) && _mode != ModeType.NONE) //수비표시 또는 공격표시하는 상황일 경우
        {
            OnClickPlanetCloseButton(_mode);
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
    public static void OnClickOpenModeButton(ModeType mode)
    {
        OnClickPlaentOpen?.Invoke(mode);
    }

    public void OnClickPlanetOpenButton()
    {
        OnClickPlanetOpenButton(_mode);
    }
    public void OnClickPlanetCloseButton()
    {
        OnClickPlanetCloseButton(_mode);
    }
    private static ModeType _mode = ModeType.NONE;
    public void OnClickPlanetOpenButton(ModeType mode) // mode는 수비표시 또는 공격표시 모드이다.
    {
        _mode = mode;
        _isDoorOpened = true;
        _miniMap_Space.SetActive(false);
        _planetCloseButton.gameObject.SetActive(true);
        _planetOpenButton.gameObject.SetActive(false);
        _planetInternal.gameObject.SetActive(true);
        if(mode != ModeType.NONE)
        {
            OpenMode(mode);
        }
    }
    public void OnClickPlanetCloseButton(ModeType mode)
    {
        if (mode != ModeType.NONE)
        {
            CloseMode(mode);
            return;
        }
        _isDoorOpened = false;
        _miniMap_Space.SetActive(true);
        _planetCloseButton.gameObject.SetActive(false);
        _planetOpenButton.gameObject.SetActive(true);
        _planetInternal.gameObject.SetActive(false);
    }
    public static event Action OnCreateModeOpen;
    public static event Action OnAttackModeOpen;
    public static event Action OnDefenseModeOpen;
    public static void OpenMode(ModeType mode)
    {
        switch (mode)
        {
            case ModeType.AttackMode:
                OnAttackModeOpen?.Invoke();
                break;
            case ModeType.DefenseMode:
                OnDefenseModeOpen?.Invoke();
                break;
            case ModeType.CreateMode:
                OnCreateModeOpen?.Invoke();
                break;
        }
    }
    public static event Action<ModeType> OnClickPlaentOpen;
    public static event Action OnCreateModeClose;
    public static event Action OnAttackModeClose;
    public static event Action OnDefenseModeClose;

    public static void CloseMode(ModeType mode)
    {
        switch (mode)
        {
            case ModeType.AttackMode:
                OnAttackModeClose?.Invoke();
                break;
            case ModeType.DefenseMode:
                OnDefenseModeClose?.Invoke();
                break;
            case ModeType.CreateMode:
                OnCreateModeClose?.Invoke();
                break;
        }
        _mode = ModeType.NONE;
    }
}
