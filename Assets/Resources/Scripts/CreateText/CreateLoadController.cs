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

    private LoadingTask StartCreateLoad(CreateLoad createLoad, float createTime, float loadingDelayTime)
    {
        CreateLoadComponent loadingTextComponent = Instantiate(loadingTextPrefab, parent);
        createLoad._loadingTextComponent = loadingTextComponent;
        LoadingTask loadingTask = loadingTextComponent.StartLoadingText(createLoad, createTime, loadingDelayTime);
        ObjectVisbilitySystem.Instance.AddToList(loadingTextComponent);
        return loadingTask;
    }
}
