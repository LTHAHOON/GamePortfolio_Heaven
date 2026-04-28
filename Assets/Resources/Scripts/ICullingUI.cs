using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICullingUI
{
    GameObject ThisGameObject { get; }
    Collider ColliderForCulling { get; }
    bool IsForceHideUI { get;}
    void SetForceHideUI(bool isForceHide);
}
