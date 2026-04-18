using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveGameObject : MonoBehaviour
{
    private static GameObject _gameObject;
    private void Awake()
    {
        _gameObject = transform.GetChild(0).gameObject;
        _gameObject.SetActive(false);
    }
    public static void SetActive(bool active)
    {
        _gameObject.SetActive(active);
    }
}
