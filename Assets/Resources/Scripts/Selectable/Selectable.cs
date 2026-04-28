using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour, ISelectable
{
    protected ISelectableOwner _owner;
    public ISelectableOwner Owner => _owner;
    private void Awake()
    {
        TryGetComponent<ISelectableOwner>(out _owner);
    }
    public void OnSelected()
    {
        _owner?.OnSelected();
    }
    public void OnDeSelected()
    {
        _owner?.OnDeSelected();
    }

}
