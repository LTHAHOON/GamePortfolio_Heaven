using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface ISelectableOwner
{
    MonoBehaviour Owner { get; }
    void OnSelected() { }
    void OnDeSelected() { }
    void OnDragSelected() { }
    void OnDragDeSelected() { }
}
