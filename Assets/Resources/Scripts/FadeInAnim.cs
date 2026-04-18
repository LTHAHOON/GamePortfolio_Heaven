using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class FadeInAnim : MonoBehaviour
{
    [SerializeField]
    private float _fadeSpeed;

    private float _dealyTime = 0;
    private const float _maxTime = 3f;
    public void Update()
    {
        if (_image != null)
        {
            if(_dealyTime <= _maxTime)
            {
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _dealyTime * _fadeSpeed);

            }
        }
        else if(_text != null)
        {
            if (_dealyTime <= _maxTime)
            {
                _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _dealyTime * _fadeSpeed);
            }
        }

        _dealyTime += Time.deltaTime;

    }

    private Image _image;
    private TextMeshProUGUI _text;
    public void Awake()
    {
        if(TryGetComponent(out Image image))
        {
            _image = image;
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _dealyTime);
        }
        else if (TryGetComponent(out TextMeshProUGUI text))
        {
            _text = text;
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _dealyTime);
        }
        else { Debug.Log("ERROR: image or text == null"); }
    }

    public void OnDisable()
    {
        _dealyTime = 0;
    }
}
