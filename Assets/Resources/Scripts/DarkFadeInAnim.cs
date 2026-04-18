using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DarkFadeInAnim : MonoBehaviour
{
    [SerializeField]
    private float _fadeSpeed;

    private float _dealyTime = 0;
    private const float _maxTime = 3f;

    public void Update()
    {
        if (_image != null)
        {
            if (_dealyTime <= _maxTime)
            {
                _image.color = Color.HSVToRGB(0, 0, _dealyTime * _fadeSpeed);
                _dealyTime += Time.deltaTime;
            }
        }
            
    }

    private Image _image;
    public void Awake()
    {
        if(TryGetComponent(out Image image))
        {
            _image = image;
        }
        else
        {
            Debug.Log("ERROR: _imageColor == null");
        }
    }

    public void OnDisable()
    {
        _dealyTime = 0;
    }
}
