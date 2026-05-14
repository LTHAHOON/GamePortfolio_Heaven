using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragSelectable : Selectable
{
    public void OnDragSelected()
    {
        _owner?.OnDragSelected();
    }
    public void OnDragDeSelected()
    {
        _owner?.OnDragDeSelected();
    }


}
