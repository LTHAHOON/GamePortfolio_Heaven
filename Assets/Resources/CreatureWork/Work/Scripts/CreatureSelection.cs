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

public class CreatureSelection : Selection<CreatureFSM>
{
    private void OnDestroy()
    {
        _selectedList.Clear();
    }

    public override List<TComp> GetSelectionComponents<TComp>()
    {
        if (_selectedList.Count <= 0) return null;
        List<TComp> components = new(_selectedList.Count);
        if (typeof(TComp) == typeof(CreatureFSM))
        {
            components = (List<TComp>)(object)_selectedList;
            return components;
        }
        for (int i = 0; i < _selectedList.Count; i++)
        {
            if (typeof(TComp) == typeof(Animator))
            {
                components.Add(_selectedList[i].GetAnimator() as TComp);
            }
            else if (typeof(TComp) == typeof(NavMeshAgent))
            {
                components.Add(_selectedList[i].GetNavMeshAgent() as TComp);
            }
            else if (_selectedList[i].TryGetComponent(out TComp component))
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
