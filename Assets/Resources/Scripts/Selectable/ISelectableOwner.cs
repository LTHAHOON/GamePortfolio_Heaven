using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectableOwner
{
    void OnSelected();
    void OnDeSelected();
    void OnDragSelected() { }
    void OnDragDeSelected() { }
}
