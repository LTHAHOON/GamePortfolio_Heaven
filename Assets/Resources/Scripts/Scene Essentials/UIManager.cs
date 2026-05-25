using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public enum FactionColoringMode
    {
        BasicPlayerColor,
        TeamColor,
        FriendTeamColor,
    }
    public enum UIHexadecimal
    {
        BasicFactionPlayerHex = 0x0A7300,
        BasicFactionEnemyHex = 0xFF0500,

        FactionPlayerTeamHex = 0x5B9BD5,
        FactionEnemyTeamHex = 0xFF0500,

        FactionFriendTeamHex = 0x7030A0,

        DrawScreenHex = 0x00FF00,
        DrawScreenBorderHex = 0x00FF00,

        ButtonDisableHex = 0xC8C8C8,
        ButtonBaseHex = 0xFFFFFF,
    }

    [SerializeField]
    private FactionColoringMode _factionColorMode;
    [Range(0f, 3f)]
    [SerializeField]
    private float _factionColorIntensity = 1.5f;
    [SerializeField]
    [ColorUsage(true,true)]
    private Color _factionAllyColor = Color.green;
    [SerializeField]
    [ColorUsage(true, true)]
    private Color _factionEnemyColor = Color.red;
    [SerializeField]
    [ColorUsage(true,true)]
    private Color _factionAllyPlanetColor = Color.blue;
    [SerializeField]
    [ColorUsage(true, true)]
    private Color _factionPlanetEnemyColor = Color.red;
    [Space]

    [SerializeField]
    private Material _playerOutLineMaterial;
    [SerializeField]
    private Material _enemyOutLineMaterial;
    [SerializeField]
    private Material _playerNexusMaterial;
    [SerializeField]
    private Material _enemyNexusMaterial;

    [Space]

    [SerializeField]
    private Color _drawScreenColor = Color.green;
    [SerializeField]
    private Color _drawScreenBorderColor = Color.green;
    [Space]

    [Header("목적지 표시 컬러(Attack Or Defense Color)")]
    [SerializeField]
    private Color _destMarkColor_Defense = Color.blue;
    [SerializeField]
    private Color _destMarkColor_Attack = Color.red;
    [Header("Move 표시 컬러")]
    [SerializeField]
    private Color _moveMarkColor = Color.green;
    [Space]

    [SerializeField]
    public Color _buttonDisableColor = Color.white;
    [SerializeField]
    private Color _buttonBaseColor = Color.white;
    [SerializeField]
    private CanvasScaler _hudCanvasScaler;
    [SerializeField]
    private RectMask2D _spaceHUDMask;

    [SerializeField] 
    private GameObject _createCountUI;
    
    void Awake()
    {
        LoadButtonColor();
    }
    private void OnValidate()
    {
        LoadFactionColorMode();
    }
    
    private void LoadButtonColor()
    {
        string hex = ((int)UIHexadecimal.ButtonDisableHex).ToString("X6");
        ColorUtility.TryParseHtmlString("#" + hex, out _buttonDisableColor);
        hex = ((int)UIHexadecimal.ButtonBaseHex).ToString("X6");
        ColorUtility.TryParseHtmlString("#" + hex, out _buttonBaseColor);
    }

    private void LoadFactionColorMode()
    {
        switch (_factionColorMode)
        {
            case FactionColoringMode.BasicPlayerColor:
                {
                    string hex = ((int)UIHexadecimal.BasicFactionPlayerHex).ToString("X6");
                    ColorUtility.TryParseHtmlString("#" + hex, out _factionAllyColor);
                    hex = ((int)UIHexadecimal.BasicFactionEnemyHex).ToString("X6");
                    ColorUtility.TryParseHtmlString("#" + hex, out _factionEnemyColor);
                    break;
                }
            case FactionColoringMode.TeamColor:
                {
                    string hex = ((int)UIHexadecimal.FactionPlayerTeamHex).ToString("X6");
                    ColorUtility.TryParseHtmlString("#" + hex, out _factionAllyColor);
                    hex = ((int)UIHexadecimal.FactionEnemyTeamHex).ToString("X6");
                    ColorUtility.TryParseHtmlString("#" + hex, out _factionEnemyColor);
                    break;
                }
            case FactionColoringMode.FriendTeamColor:
                {
                    string hex = ((int)UIHexadecimal.FactionFriendTeamHex).ToString("X6");
                    ColorUtility.TryParseHtmlString("#" + hex, out _factionAllyColor);
                    hex = ((int)UIHexadecimal.FactionEnemyTeamHex).ToString("X6");
                    ColorUtility.TryParseHtmlString("#" + hex, out _factionEnemyColor);
                    break;
                }
        }
        _factionAllyColor *= _factionColorIntensity;
        _factionEnemyColor *= _factionColorIntensity;
        SetColorOfFaction();
    }

    public Color GetColorOfDrawScreen()
    {
        return _drawScreenColor;
    }
    public Color GetColorOfDrawScreenBorder()
    {
        return _drawScreenBorderColor;
    }

    public Color GetDestMarkColor(ModeType modeType)
    {
        return modeType switch
        {
            ModeType.AttackMode => _destMarkColor_Attack,
            ModeType.DefenseMode => _destMarkColor_Defense,
            _ => _destMarkColor_Defense,
        };
    }
    public Color GetMoveMarkColor()
    {
        return _moveMarkColor;
    }

    //Shader.PropertyToID는 쉐이더 전용 ID 해쉬값으로 구해주기 때문에 최적화에 유용하다.
    private static readonly int _allyOutlineColorID = Shader.PropertyToID("_Global_Outline_Color_Ally");
    private static readonly int _enemyOutlineColorID = Shader.PropertyToID("_Global_Outline_Color_Enemy");
    private readonly int _nexusColorID = Shader.PropertyToID("_Color");
    private void SetColorOfFaction()
    {
        if(Shader.GetGlobalColor(_allyOutlineColorID) != _factionAllyColor)
        {
            Shader.SetGlobalColor(_allyOutlineColorID, _factionAllyColor);
            _playerNexusMaterial.SetColor(_nexusColorID, _factionAllyColor);
        }
        if (Shader.GetGlobalColor(_enemyOutlineColorID) != _factionEnemyColor)
        {
            Shader.SetGlobalColor(_enemyOutlineColorID, _factionEnemyColor);
            _enemyNexusMaterial.SetColor(_nexusColorID, _factionEnemyColor);
        }
    }
    
    public Color FactionPlayerColor => _factionAllyColor;
    public Color FactionEnemyColor => _factionEnemyColor;

    public Color ChangeToImageDisableColor()
    {
        return _buttonDisableColor;
    }
    public Color ChangeToInitialColor()
    {
        return _buttonBaseColor;

    }

    /*
    public Vector3 GetMousePositionInSubCamera()
    {
        //Image Rect는 (Screen.width - r.width) / 2만큼 공백이 존재하기 때문에 Image Rect좌표가 실제 위치보다 작으므로 실제 위치에 그 델타값을 빼줘서 값을 맞춰줍니다.
        //예를 들어 Image Rect의 width가 1700이라면 실질적으로는 위치가 1810이기 때문에 이에 대한 델타값을 사용하여 1700 == 1700이 되도록 보정해주는 것입니다.
        //SubCamera는 RenderTexture라서 subCamera를 이용해 월드나 다른 좌표를 변환하면 해당 RenderTexture Image Rect 크기에 맞춰서 변환이 되기 때문에
        //Input.mousePosition을 넣어 변환하면 1700이어야 하는 width가 1810이 되어 버립니다. 따라서 보정을 해야합니다.
        
        float widthDelta = (Screen.width - _subCameraRTImageRect.rect.width) / 2;
        float heightDelta = (Screen.height - _subCameraRTImageRect.rect.height) / 2;
        float x = (Input.mousePosition.x - widthDelta);
        float y = (Input.mousePosition.y - heightDelta);

        return new Vector3(x, y, 0f);
        
    }
    */

    public void SetActiveAllChild(GameObject obj, bool active)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            obj.transform.GetChild(i).gameObject.SetActive(active);
        }
    }

    [Header("Button Scale Up Settings")]
    [SerializeField]
    private Vector3 _buttonScaleUPDelta = new Vector3(0.5f, 0.5f , 0f);
    [SerializeField]
    private float _buttonScaleUPSpeed = 10f;
    public void OnPointerEnterScaleUp(Transform buttonTransform, Vector3 baseScale, Vector3 buttonScaleUPDelta = default)
    {
        if(buttonScaleUPDelta == Vector3.zero)
        {
            buttonScaleUPDelta = _buttonScaleUPDelta;
        }
        if (buttonTransform != null)
        {
            buttonTransform.localScale = Vector3.Lerp(buttonTransform.localScale, baseScale + buttonScaleUPDelta, Time.deltaTime * _buttonScaleUPSpeed);
        }
    }
    public void OnPointerExitScaleDown(Transform buttonTransform, Vector3 baseScale)
    {
        if (buttonTransform != null)
        {
            buttonTransform.localScale = Vector3.Lerp(buttonTransform.localScale, baseScale, Time.deltaTime * _buttonScaleUPSpeed);
        }
    }
    
    public bool IsCreateCountUIActive => _createCountUI.gameObject.activeSelf;
    public bool IsSubCameraActive => _spaceHUDMask.enabled;
    public CanvasScaler HUDCanvasScaler => _hudCanvasScaler;
}
