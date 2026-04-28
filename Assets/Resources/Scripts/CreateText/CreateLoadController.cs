using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLoadController : MonoBehaviour
{
    [SerializeField]
    CreateLoadComponent loadingTextPrefab;
    void Awake()
    {
        CreateLoad.OnStartCreateLoad += StartCreateLoad;
    }

    private LoadingTask StartCreateLoad(CreateLoad createLoad, float createTime, float loadingDelayTime)
    {
        CreateLoadComponent loadingTextComponent = Instantiate(loadingTextPrefab, transform);
        createLoad._loadingTextComponent = loadingTextComponent;
        LoadingTask loadingTask = loadingTextComponent.StartLoadingText(createLoad, createTime, loadingDelayTime);
        ObjectVisbilitySystem.Instance.AddToList(loadingTextComponent);
        return loadingTask;
    }
}
