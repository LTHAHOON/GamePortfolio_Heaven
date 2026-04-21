using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

public class CreatureSelection : MonoBehaviour
{
    private static List<CreatureFSM> _selectedCharacters = new();
    [SerializeField]
    private LayerMask _clickColliderLayer;
    [SerializeField]
    private MyUnitPrefabDataControl _myUnitPrefabDataControl;
    [SerializeField]
    private string _allyTag = Fraction.Ally.ToString();

    private void Update()
    {
        SelectionProcess();
    }

    private void SelectionProcess()
    {
        if (UIManager.Instance.IsSubCameraActive && !MiniMapController.IsPointerOverMiniMap && !CreateCountController.IsActive())
        {
            int selectedCount = InputManager.Instance.TrySelectionByUnitType(out bool bOnClick, Camera.main, 
                                                _clickColliderLayer, UnitType.Creature, true, AddToSelectedCharacters);
            if(bOnClick)
            {
                OnSelectOrClearSelection(selectedCount);
            }
            else
            {
                selectedCount = InputManager.Instance.TryDragSelectionByUnitType(out bool bOnDrag, Camera.main,
                                                                            UnitType.Creature, AddToSelectedCharacters);
                if(bOnDrag)
                {
                    OnSelectOrClearSelection(selectedCount);
                }
            }
        }
        else
        {
            if (CreatureControl._isSelect)
            {

                ClearSelectedCreatures();
            }
        }
    }

    private void OnSelectOrClearSelection(int selectedCount)
    {
        if (selectedCount <= 0)
        {
            if(_selectedCharacters.Count > 0)
            {
                ClearSelectedCreatures();
            }
        }
        else
        {
            CreatureControl._isSelect = true;
        }
    }


    public static void ClearSelectedCreatures()
    {
        for (int i = 0; i < _selectedCharacters.Count; i++)
        {
            _selectedCharacters[i].OnDeSelected();
        }
        CreatureControl._isSelect = false;
        _selectedCharacters.Clear();
    }

    public void AddToSelectedCharacters(Unit selectedCharacter)
    {
        if (!CreatureControl._isSelect)
        {
            CreatureControl._isSelect = true;
        }
        if(selectedCharacter.CompareTag(_allyTag))
        {
            if(selectedCharacter is CreatureFSM selectedCretureFSM)
            {
                selectedCretureFSM._isSelected = CreatureControl._isSelect;
                _selectedCharacters.Add(selectedCretureFSM);
            }
        }
    }
    public static void RemoveToSelectedCharacters(CreatureFSM selectedCharacter)
    {
        _selectedCharacters.Remove(selectedCharacter);
    }
 

    public static int GetSelectionCharactersCount()
    {
        return _selectedCharacters.Count;
    }

    public static List<T> GetSelectionCharacters<T>() where T : Component
    {
        if (_selectedCharacters.Count <= 0) return null;
        List<T> components = new(_selectedCharacters.Count);
        if (typeof(T) == typeof(CreatureFSM))
        {
            components = (List<T>)(object)_selectedCharacters;
            return components;
        }
        for (int i = 0; i < _selectedCharacters.Count; i++)
        {
            if (typeof(T) == typeof(Animator))
            {
                components.Add(_selectedCharacters[i].GetAnimator() as T);
            }
            else if (typeof(T) == typeof(NavMeshAgent))
            {
                components.Add(_selectedCharacters[i].GetNavMeshAgent() as T);
            }
            else if (_selectedCharacters[i].TryGetComponent(out T component))
            {
                components.Add(component);
            }
            else
            {
                Debug.LogError("Error: Type not found.");
                return null;
            }
        }
        return components;
    }
}
