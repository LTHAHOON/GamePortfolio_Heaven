using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICullingUI
{
    GameObject Owner { get; }
    Collider ColliderForCulling { get; }
    bool IsForceHideUI { get; set; }
    void SetForceHideUI(bool isForceHide);
}
