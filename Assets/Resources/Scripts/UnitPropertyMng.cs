using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class UnitPropertyMng : MonoBehaviour
{
    public void ClearAllUnitButtonOrder(List<GameObject> allUnitButtonsOrder)
    {
        if (allUnitButtonsOrder.Count > 0)
        {
            allUnitButtonsOrder.Clear();
        }
    }

    public void ActiveAllPropertyUnit(List<GameObject> allUnitButtonsOrder, bool active)
    {
        foreach (GameObject unitButton in allUnitButtonsOrder)
        {
            unitButton.SetActive(active);
        }
    }

    public void ActivePropertyUnit(int index, List<GameObject> allUnitButtonsOrder, bool active)
    {
        try
        {
            for (int i = arrOrderIndex[index]; i < arrOrderIndex[index] + 3; i++)
            {
                allUnitButtonsOrder[i].SetActive(true);
            }
        }
        catch (ArgumentOutOfRangeException) { return; }
    }

    private readonly int[] arrOrderIndex = { 0, 3, 6, 9 };
    public void SetPropertyOrder(UnitProperty property, UnitProperty[] arrPropertyOrder)
    {
        SwapPropertyOrder(property, arrPropertyOrder);
    }

    private void SwapPropertyOrder(UnitProperty property, UnitProperty[] arrPropertyOrder)
    {
        if(arrPropertyOrder.Length > 0)
        {
            int swapCount = Array.IndexOf(arrPropertyOrder, property);
            if (swapCount > 0)
            {
                for (int i = 0; i < swapCount; i++)
                {
                    for (int j = 0; j < arrPropertyOrder.Length - 1; j++)
                    {
                        UnitProperty tempProperty = arrPropertyOrder[j + 1];
                        arrPropertyOrder[j + 1] = arrPropertyOrder[j];
                        arrPropertyOrder[j] = tempProperty;
                    }
                }
            }
        }
    }
}
