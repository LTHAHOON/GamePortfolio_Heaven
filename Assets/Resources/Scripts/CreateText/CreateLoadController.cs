using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLoadController : MonoBehaviour
{
    [SerializeField]
    CreateLoadComponent loadingTextPrefab;
    [SerializeField]
    private Transform parent;
    void Awake()
    {
        CreateLoad.OnStartCreateLoad += StartCreateLoad;
    }

    private void StartCreateLoad(CreateLoad createLoad, float createTime, float loadingDelayTime)
    {
        CreateLoadComponent loadingTextComponent = Instantiate(loadingTextPrefab, parent);
        createLoad._loadingTextComponent = loadingTextComponent;
        loadingTextComponent.StartLoadingText(createLoad, createTime, loadingDelayTime);
        ObjectVisbilitySystem.AddToList(loadingTextComponent);
    }

}
