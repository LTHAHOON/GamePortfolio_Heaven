using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateCountController : MonoBehaviour
{
    [SerializeField] 
    private GameObject _creaeCountUI;
    [SerializeField]
    private  TextMeshProUGUI _guideText;
    [SerializeField]
    private TextMeshProUGUI _createCountText;
    [SerializeField]
    private Image _createCountImage_L;
    [SerializeField]
    private Image _createCountImage_R;
    private int _curCreateCount = 1;
    private int _maxCreateCount = 1;
    private readonly float delayBetweenInputs = 0.2f;
    private readonly int _initialCount = 1;
    private float _curTime = 0f;
    private void LateUpdate()
    {
        if(_curCreateCount != 0)
        {
            if (Input.GetKey(KeyCode.A) && _curTime <= 0)
            {
                SetCreateCount(0, _maxCreateCount);
                _curTime = delayBetweenInputs;
            }
            if (Input.GetKey(KeyCode.D) && _curTime <= 0)
            {
                SetCreateCount(1, _maxCreateCount);
                _curTime = delayBetweenInputs;
            }
            _curTime -= Time.deltaTime;
        }
    }
    public void RefreshCreateCount(MPData mpData, MPData subMpData = null)
    {
        if (GetCurCreateCount() <= 0)
        {
            SetCurCreateCount(1);
        }
        if(subMpData != null)
        {
            SetMaxCreateCount(mpData, subMpData);
        }
        else
        {
            SetMaxCreateCount(mpData);
        }
        ChangeCreateCountText();
    }

    public void InitCreateCount(MPData mpData, string guideText, MPData subMpData = null)
    {
        if(subMpData != null)
        {
            RefreshCreateCount(mpData, subMpData);
        }
        else
        {
            RefreshCreateCount(mpData);
        }
        ChangeGuideText(guideText);
    }
    public void ConsumeCurCreateCount(int consumeCount)
    {
        int createCount = _curCreateCount;
        createCount -= consumeCount;
        SetCurCreateCount(createCount);
    }

    public void SetMaxCreateCount(MPData mpData)
    {
        float maxCreateCount = MPDataController.Instance.MP_StatusSlider.value / mpData.MP_ConsValue;
        _maxCreateCount = Mathf.FloorToInt(maxCreateCount);
    }
    public void SetMaxCreateCount(MPData mpData,MPData subMpData)
    {
        float maxCreateCount = (MPDataController.Instance.MP_StatusSlider.value - mpData.MP_ConsValue) /  subMpData.MP_ConsValue;
        maxCreateCount = Mathf.Abs(maxCreateCount);
        _maxCreateCount = Mathf.FloorToInt(maxCreateCount);
    }

    public int GetMaxCreateCount()
    {
        return _maxCreateCount;
    }
    public void SetCurCreateCount(int createCount)
    {
         _curCreateCount = createCount;
    }
    
    public int GetCurCreateCount()
    {
        return _curCreateCount;
    }

    public void ChangeGuideText(string text)
    {
        _guideText.text = text;
    }
    public void ChangeCreateCountText()
    {
        _createCountText.text = $"{_curCreateCount} / {_maxCreateCount}";
    }
    public void SetActiveCount(bool active)
    {
        _creaeCountUI.gameObject.SetActive(active);
    }


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
            _curCreateCount -= _initialCount;
            _maxCreateCount = maxCreateCount;

            ChangeCreateCountText ();
            return;
        }
        if(button == 1)
        {
            if(_curCreateCount >= maxCreateCount)
            {
                _curCreateCount = _initialCount;
                _maxCreateCount = maxCreateCount;
                ChangeCreateCountText();
                return;
            }
            _curCreateCount += _initialCount;
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
