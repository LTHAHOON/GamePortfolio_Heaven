using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIFollowObject))]
public class CreateLoadComponent : LoadingTextComponent
{
    [SerializeField]
    private UIFollowObject _uiFollowObject;
    [HideInInspector]
    public CreateLoad _createLoad;

    public void Awake()
    {
        _uiFollowObject = GetComponent<UIFollowObject>();
    }
    public override void Update()
    {
        base.Update();

    }
    public void LateUpdate()
    {
        if (!_uiFollowObject) return;
        _uiFollowObject.FollowObject(Camera.main, _createLoad.gameObject, gameObject, _createLoad._screenOffset, _createLoad._localOffset);
    }
    public override void StartLoadingText<T>(T target, float loadingTime, float delayTime)
    {
        base.StartLoadingText(loadingTime, delayTime);
        if(target is CreateLoad createLoad)
        {
            _createLoad = createLoad;
        }
    }
    private void OnDestroy()
    {
        ObjectVisbilitySystem.RemoveToList(this);
    }
}
