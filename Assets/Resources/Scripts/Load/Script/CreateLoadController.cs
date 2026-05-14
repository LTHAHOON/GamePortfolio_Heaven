using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLoadController : Singleton<CreateLoadController>
{
    [SerializeField]
    private CreateLoadComponent loadingTextPrefab;
    [SerializeField]
    private List<CreateLoadComponent> _loadComponentList;
    void Awake()
    {
        CreateLoad.OnStartCreateLoad += StartCreateLoad;
    }

    private void Update()
    {
        for (int i = 0; i < _loadComponentList.Count; i++)
        {
            if(_loadComponentList[i])
            {
                _loadComponentList[i].UpdateLoadingText();
            }
        }
    }

    public void AddCreateLoadComponent(CreateLoadComponent createLoadComponent)
    {
        _loadComponentList.Add(createLoadComponent);
    }
    public void RemoveCreateLoadComponent(CreateLoadComponent createLoadComponent)
    {
        _loadComponentList.Remove(createLoadComponent);
    }

    private LoadingTask StartCreateLoad(CreateLoad createLoad, float createTime, float loadingDelayTime)
    {
        CreateLoadComponent loadingTextComponent = Instantiate(loadingTextPrefab, transform);
        AddCreateLoadComponent(loadingTextComponent);
        createLoad._loadingTextComponent = loadingTextComponent;
        LoadingTask loadingTask = loadingTextComponent.StartLoadingText(createLoad, createTime, loadingDelayTime);
        ObjectVisbilitySystem.Instance.AddToList(loadingTextComponent);
        return loadingTask;
    }
}
