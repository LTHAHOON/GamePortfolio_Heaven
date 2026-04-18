using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonSystem : MonoBehaviour // 수동적으로 라디오박스 기능 설정
{
    public static void IsOnUnitRadioBox(IRadioBoxes radioBox, int sequence, bool isOn)
    {
        List<Toggle> radioBoxList = radioBox.GetRadioBoxList();
        int ChildIndex = sequence - 1;
        if (CheckListIndex(radioBoxList, ChildIndex))
        {
            radioBoxList[ChildIndex].isOn = isOn;
        }
    }

    public static void SetActiveRadioBox(IRadioBoxes radioBox, int sequence, bool isActive)
    {
        List<Toggle> radioBoxList = radioBox.GetRadioBoxList();
        int ChildIndex = sequence - 1;
        if(CheckListIndex(radioBoxList, ChildIndex))
        {
            radioBoxList[ChildIndex].gameObject.SetActive(isActive);
        }
    }

    public static bool CheckListIndex<T>(List<T> list, int indexToCheck)
    {
        if (list.Count > indexToCheck || indexToCheck >= 0)
        {
            return true;
        }
        else
        {
            Debug.Log("ERROR: WRONG RadioBox ChildIndex");
        }
        return false;
    }

    public static int FindRadioBoxIndexToIsOn(IRadioBoxes radioBox)
    {
        List<Toggle> unitEditSearchRadioBoxes = radioBox.GetRadioBoxList();
        if (unitEditSearchRadioBoxes.Count > 0)
        {
            for (int i = 0; i < unitEditSearchRadioBoxes.Count; i++)
            {
                if (unitEditSearchRadioBoxes[i].isOn) { return i; }
            }
        }
        else
        {
            Debug.Log("Null: unitEditSearchRadioBoxes");
        }
        return -1;
    }
}

