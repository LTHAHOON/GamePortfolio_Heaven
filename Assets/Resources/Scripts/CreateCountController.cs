using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateCountController : Singleton<CreateCountController>
{
    private static TextMeshProUGUI _guideText;
    private static TextMeshProUGUI _createCountText;

    private static Image _createCountImage_L;
    private static Image _createCountImage_R;
    private static int _curCreateCount = 1;
    private static int _maxCreateCount = 1;
    void Awake()
    {
        _guideText = transform.Find("GuideText").GetComponent<TextMeshProUGUI>();
        _createCountText = transform.Find("CreateCountText").GetComponent<TextMeshProUGUI>();
        _createCountImage_L = transform.Find("CreateCountImage_L").GetComponent<Image>();
        _createCountImage_R = transform.Find("CreateCountImage_R").GetComponent<Image>();
    }

    private readonly float delayBetweenInputs = 0.2f;
    private float time = 0;
    private void LateUpdate()
    {
        if(_curCreateCount != 0)
        {
            if (Input.GetKey(KeyCode.A) && time <= 0)
            {
                SetCreateCount(0, _maxCreateCount);
                time = delayBetweenInputs;
            }
            if (Input.GetKey(KeyCode.D) && time <= 0)
            {
                SetCreateCount(1, _maxCreateCount);
                time = delayBetweenInputs;
            }
            time -= Time.deltaTime;
        }
    }
    public static void RefreshCreateCount(MPData unitMPData, MPData? subUnitMPData = null)
    {
        if (GetCurCreateCount() <= 0)
        {
            SetCurCreateCount(1);
        }
        if(subUnitMPData.HasValue)
        {
            SetMaxCreateCount(unitMPData, subUnitMPData.Value);
        }
        else
        {
            SetMaxCreateCount(unitMPData);
        }
        ChangeCreateCountText();
    }

    public static void InitCreateCount(MPData unitMPData, string guideText, MPData? subUnitMPData = null)
    {
        if(subUnitMPData.HasValue)
        {
            RefreshCreateCount(unitMPData, subUnitMPData);
        }
        else
        {
            RefreshCreateCount(unitMPData);
        }
        ChangeGuideText(guideText);
    }
    public static void ConsumeCurCreateCount(int consumeCount)
    {
        int createCount = _curCreateCount;
        createCount -= consumeCount;
        SetCurCreateCount(createCount);
    }

    public static void SetMaxCreateCount(MPData unitMPData)
    {
        float maxCreateCount = MPController.Instance.MP_StatusSlider.value / unitMPData.MP_ConsValue;
        _maxCreateCount = Mathf.FloorToInt(maxCreateCount);
    }
    public static void SetMaxCreateCount(MPData unitMPData, MPData subUnitMPData)
    {
        float maxCreateCount = (MPController.Instance.MP_StatusSlider.value - unitMPData.MP_ConsValue) /  subUnitMPData.MP_ConsValue;
        maxCreateCount = Mathf.Abs(maxCreateCount);
        _maxCreateCount = Mathf.FloorToInt(maxCreateCount);
    }

    public static int GetMaxCreateCount()
    {
        return _maxCreateCount;
    }
    public static void SetCurCreateCount(int createCount)
    {
         _curCreateCount = createCount;
    }
    
    public static int GetCurCreateCount()
    {
        return _curCreateCount;
    }

    public static void ChangeGuideText(string text)
    {
        _guideText.text = text;
    }
    public static void ChangeCreateCountText()
    {

        _createCountText.text = $"{_curCreateCount} / {_maxCreateCount}";
    }
    private static bool _isActive = false;
    public static void SetActiveCount(bool active)
    {
        _createCountText.gameObject.SetActive(active);
        _createCountImage_L.gameObject.SetActive(active);
        _createCountImage_R.gameObject.SetActive(active);
    }
    public static bool IsActive()
    {
        return _isActive;
    }

    private const int _initialValue = 1;
    public void SetCreateCount(int button, int maxCreateCount)
    {
        
        if(button == 0)
        {
            if(_curCreateCount <= 1)
            {
                _curCreateCount = maxCreateCount;
                _maxCreateCount = maxCreateCount;
                ChangeCreateCountText();
                return;
            }
            _curCreateCount -= _initialValue;
            _maxCreateCount = maxCreateCount;

            ChangeCreateCountText ();
            return;
        }
        if(button == 1)
        {
            if(_curCreateCount >= maxCreateCount)
            {
                _curCreateCount = _initialValue;
                _maxCreateCount = maxCreateCount;
                ChangeCreateCountText();
                return;
            }
            _curCreateCount += _initialValue;
            _maxCreateCount = maxCreateCount;
            ChangeCreateCountText();
            return;    
        }
        Debug.Log("SetCreateCount �Ķ���� button�� 0 �Ǵ� 1�� �����մϴ�.");

    }

    private void OnEnable()
    {
        _curCreateCount = 1;
    }
    private void OnDisable()
    {
        _curCreateCount = 0;
    }
}
