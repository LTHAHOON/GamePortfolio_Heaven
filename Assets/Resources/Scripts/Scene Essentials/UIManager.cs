using System.Runtime.CompilerServices;
using UnityEngine;

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

    [SerializeField]
    [ColorUsage(true,true)]
    private Color _factionPlayerColor = Color.green;
    [SerializeField]
    [ColorUsage(true, true)]
    private Color _factionEnemyColor = Color.red;
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

    [SerializeField]
    public Color _buttonDisableColor = Color.white;
    [SerializeField]
    private Color _buttonBaseColor = Color.white;
    [SerializeField]
    private RectTransform _subCameraRTImageRect;
    [SerializeField]
    private Camera _subCamera;
    public Camera CurrentUICamera => _subCamera.transform.parent.gameObject.activeSelf ? _subCamera : Camera.main;
    public bool IsSubCameraActive => _subCamera.transform.parent.gameObject.activeSelf;
    void Awake()
    {
        LoadButtonColor();
        LoadFactionColorMode();
    }


    void Update()
    {
        

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
                    ColorUtility.TryParseHtmlString("#" + hex, out _factionPlayerColor);
                    hex = ((int)UIHexadecimal.BasicFactionEnemyHex).ToString("X6");
                    ColorUtility.TryParseHtmlString("#" + hex, out _factionEnemyColor);
                    break;
                }
            case FactionColoringMode.TeamColor:
                {
                    string hex = ((int)UIHexadecimal.FactionPlayerTeamHex).ToString("X6");
                    ColorUtility.TryParseHtmlString("#" + hex, out _factionPlayerColor);
                    hex = ((int)UIHexadecimal.FactionEnemyTeamHex).ToString("X6");
                    ColorUtility.TryParseHtmlString("#" + hex, out _factionEnemyColor);
                    break;
                }
            case FactionColoringMode.FriendTeamColor:
                {
                    string hex = ((int)UIHexadecimal.FactionFriendTeamHex).ToString("X6");
                    ColorUtility.TryParseHtmlString("#" + hex, out _factionPlayerColor);
                    hex = ((int)UIHexadecimal.FactionEnemyTeamHex).ToString("X6");
                    ColorUtility.TryParseHtmlString("#" + hex, out _factionEnemyColor);
                    break;
                }
        }
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

    //HASH기법인 ID로 바꿔서 저장하면 정수형이기 때문에 최적화에 유용하다.
    private static readonly int _outlineColorID = Shader.PropertyToID("_Outline_Color");
    private static readonly int _nexusColorID = Shader.PropertyToID("_Color");
    private void SetColorOfFaction()
    {
        if(_playerOutLineMaterial.GetColor(_outlineColorID) != _factionPlayerColor)
        {
            _playerOutLineMaterial.SetColor(_outlineColorID, _factionPlayerColor);
            _playerNexusMaterial.SetColor(_nexusColorID, _factionPlayerColor);
        }
        if (_playerOutLineMaterial.GetColor(_outlineColorID) != _factionEnemyColor)
        {
            _enemyOutLineMaterial.SetColor(_outlineColorID, _factionEnemyColor);
            _enemyNexusMaterial.SetColor(_nexusColorID, _factionEnemyColor);
        }
    }
    
    public Color GetFactionPlayerColor() => _factionPlayerColor;
    public Color GetFactionEnemyColor() => _factionEnemyColor;

    public Color ChangeToImageDisableColor()
    {
        return _buttonDisableColor;
    }
    public Color ChangeToInitialColor()
    {
        return _buttonBaseColor;

    }

    private void OnValidate()
    {
        SetColorOfFaction();
    }

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
    public void OnPointerEnterScaleUp(Transform buttonTransform, Vector3 baseScale)
    {
        if (buttonTransform != null)
        {
            buttonTransform.localScale = Vector3.Lerp(buttonTransform.localScale, baseScale + _buttonScaleUPDelta, Time.deltaTime * _buttonScaleUPSpeed);
        }
    }
    public void OnPointerExitScaleDown(Transform buttonTransform, Vector3 baseScale)
    {
        if (buttonTransform != null)
        {
            buttonTransform.localScale = Vector3.Lerp(buttonTransform.localScale, baseScale, Time.deltaTime * _buttonScaleUPSpeed);
        }
    }
}
