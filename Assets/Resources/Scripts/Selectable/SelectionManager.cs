using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface ISelection
{
    void OnSelectOrClearSelection(bool isSelected);
    void ClearSelectedList();
    void AddToSelectedList(ISelectableOwner selectedTarget);
    bool CanSelect(Selectable selectable);
}
public abstract class Selection<T> : Singleton<Selection<T>>, ISelection where T : ISelectableOwner
{

    protected List<T> _selectedList = new();
    protected bool _isSelected = false;
    public bool IsSelected => _isSelected;
    public virtual void Awake()
    {
        SelectionManager.Instance.AddSelection(this);
    }

    public void ClearSelectedList()
    {
        for (int i = 0; i < _selectedList.Count; i++)
        {
            _selectedList[i].OnDeSelected();
        }
        _isSelected = false;
        _selectedList.Clear();
    }
    public void OnSelectOrClearSelection(bool isSelected)
    {
        _isSelected = isSelected;
        if (!_isSelected)
        {
            if (_selectedList.Count > 0)
            {
                ClearSelectedList();
            }
        }
    }

    public void AddToSelectedList(ISelectableOwner selectedTarget)
    {
        if (selectedTarget.Owner.CompareTag(GameTags.Ally))
        {
            if (selectedTarget is T selectedCretureFSM)
            {
                _selectedList.Add(selectedCretureFSM);
            }
        }
    }
    public void RemoveToSelectedCharacters(T selectedCharacter)
    {
        _selectedList.Remove(selectedCharacter);
    }

    public int GetSelectionCharactersCount()
    {
        return _selectedList.Count;
    }
    public virtual List<TComp> GetSelectionComponents<TComp>() where TComp : Component
    {
        return null;
    }
    public bool CanSelect(Selectable selectable)
    {
        return selectable.Owner is T;
    }
}
public class SelectionManager : Singleton<SelectionManager>
{
    [SerializeField]
    protected LayerMask _clickColliderLayer;
    private readonly List<ISelection> _selectionList = new();
    private void Update()
    {
        if (!MiniMapController.IsPointerOverMiniMap && !CreateCountController.IsActive())
        {
            SelectionProcess();
        }
    }
    public void SelectionProcess()
    {
        ISelection selection = InputManager.Instance.TrySelection(out bool bOnClick, Camera.main,
                                            _clickColliderLayer, true);
        bool bSelected = selection != null && bOnClick;
        if (bSelected)
        {
            selection.OnSelectOrClearSelection(bSelected);
            return;
        }
        else if (bOnClick)
        {
            Debug.Log("Clicked on empty space");
            ClearAllSelection();
        }
        List<ISelection> selectionList = InputManager.Instance.TryDragSelectionByUnitType(out bool bOnDrag, Camera.main, UnitType.Creature);
        bSelected = bOnDrag && selectionList != null && selectionList.Count > 0;
        if (bSelected)
        {
            for (int i = 0; i < selectionList.Count; i++)
            {
                selectionList[i].OnSelectOrClearSelection(bSelected);
            }
        }
    }

    public void AddSelection(ISelection selection)
    {
        if (!_selectionList.Contains(selection))
        {
            _selectionList.Add(selection);
        }
    }
    public bool TryGetSelection<T>(out T selection) where T : ISelection
    {
        for (int i = 0; i < _selectionList.Count; i++)
        {
            if (_selectionList[i] is T selecteionTemp)
            {
                selection = selecteionTemp;
                return true;
            }
        }
        selection = default;
        return false;
    }
    public ISelection GetSelection(Selectable selectable)
    {
        for (int i = 0; i < _selectionList.Count; i++)
        {
            if (_selectionList[i].CanSelect(selectable))
            {
                return _selectionList[i];
            }
        }
        return null;
    }



    public void ClearAllSelection()
    {
        for (int i = 0; i < _selectionList.Count; i++)
        {
            _selectionList[i].ClearSelectedList();
        }
    }   
}
