using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
#if UNITY_EDITOR
    [SerializeField]
    private bool editorFrameRateLock = false;
    [SerializeField]
    private int _fixedFrameRate = 30;
#endif

    void Awake()
    {
#if UNITY_EDITOR
        if (editorFrameRateLock)
        {
            Application.targetFrameRate = _fixedFrameRate;
        }
#endif

    }
}
