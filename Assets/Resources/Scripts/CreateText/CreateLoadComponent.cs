using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIFollowObject))]
public class CreateLoadComponent : LoadingTextComponent<CreateLoad>, ICullingUI
{
    [SerializeField]
    private UIFollowObject _uiFollowObject;

    public bool ForceHideUI => false;
    public GameObject ThisGameObject => gameObject;
    public Collider ColliderForCulling => _load._collider;

    public bool IsForceHideUI { get; set; }

    public void Awake()
    {
        IsForceHideUI = false;
        _uiFollowObject = GetComponent<UIFollowObject>();
    }

    public void LateUpdate()
    {
        if (!_uiFollowObject || !_load) return;
        _uiFollowObject.FollowObject(Camera.main, _load.gameObject, gameObject, _load._screenOffset, _load._localOffset);
    }

    public void SetForceHideUI(bool isForceHide)
    {
        IsForceHideUI = isForceHide;
        gameObject.SetActive(!isForceHide);
    }

    private void OnDestroy()
    {
        ObjectVisbilitySystem.Instance.RemoveToList(this);
    }
}
