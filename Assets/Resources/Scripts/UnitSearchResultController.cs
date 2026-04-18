using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[Serializable]
public struct SearchButtonName
{
    public string name;
    public UnitType type;
}
public class UnitSearchResultController : Singleton<UnitSearchResultController>
{
    [Serializable]
    public struct CurrentPropertyUnitChipsData
    {
        public List<UnitChipState> _unitButtons;
        public Transform _propertySuperObj;
        public UnitProperty _property;
    }
    [SerializeField]
    private CurrentPropertyUnitChipsData[] _currentPropertyUnitChipsData = new CurrentPropertyUnitChipsData[4];

    [SerializeField]
    private UnitPropertyMng _unitPropertyMng;
    
    [SerializeField]
    private TextMeshProUGUI _resultNameText;
    public static UnitSearchResultController _instance;

    [SerializeField]
    private Unit_MyRoomMng _myRoomMng;
    
    [SerializeField]
    private GameObject[] _arrRadioBoxObj;
    
        private Dictionary<int, UnitChipState[]> _dicAddedUnitChip;

    [SerializeField]
    private SearchButtonName[] _searchButtonNames = new SearchButtonName[4];

    [SerializeField]
    private UnitProperty[] _arrPropertyOrder =
    {
        UnitProperty.FireProperty, UnitProperty.WaterProperty, UnitProperty.GroundProperty, UnitProperty.ElectricityProperty
    };

    private bool _firstStart;
    //TODO: UnitSearchResult 새로고침 하기
    public void SetAddedUnitChip(int searchButtonIndexToResfreh) 
    {
        _dicAddedUnitChip = _myRoomMng.GetDicUnitChip(_myRoomMng._addedUnitChips.ToList());
        InitalizeVariable();
        OnClickSearchButton(searchButtonIndexToResfreh);
    }

    private void ClearUnitButton()
    {
        for (int i = 0; i < _currentPropertyUnitChipsData.Length; i++)
        {
            DestroyUnitButton(_currentPropertyUnitChipsData[i]._propertySuperObj);
        }
        for (int i = 0; i < _currentPropertyUnitChipsData.Length; i++)
        {
            _currentPropertyUnitChipsData[i]._unitButtons.Clear();
        }
    }

    public void DestroyUnitButton(Transform unitPropertySuperObj)
    {
        for (int j = 0; j < unitPropertySuperObj.childCount; j++)
        {
            Destroy(unitPropertySuperObj.GetChild(j).gameObject);
        }
        // TODO: Destroy 프레임이 완전히 끝나기 전에 자식 풀려놓기
        unitPropertySuperObj.DetachChildren();

    }

    private void InitalizeVariable()
    {
        _firstStart = true;
        _curButtonNumber = -1;
        _currentRadioIndex = new int[4];
        
        if(_arrRadioBoxObj.Length > 0)
        {
            for (int i = 0; i < _arrRadioBoxObj.Length; i++)
            {
                if(_arrRadioBoxObj[i].TryGetComponent(out UnitChipSearchRadioBoxControl radioBox))
                {
                    unitChipSearchRadioBox = radioBox;
                }
                else if(_arrRadioBoxObj[i].TryGetComponent(out PropertySortRadioBoxControl radioBox2))
                {
                    propertySortRadioBox = radioBox2;
                }
                else
                {
                    Debug.Log("WRONG: Compnent Empty UnitChipSearchRadioBoxControl or PropertySortRadioBoxControl");
                }
            }
        }
        else
        {
            Debug.Log("WRONG: _arrRadioBoxObj.Length <= 0");
        }
    }
    
    public int GetCurrentButtonNumber()
    {
        return _curButtonNumber;
    }   

    private UnitChipState[] _unitButton;
    private int _curButtonNumber = -1;
    private IRadioBoxes unitChipSearchRadioBox;
    private IRadioBoxes propertySortRadioBox;
    //TODO: Search Process
    public void OnClickSearchButton(int buttonNumber)  
    {
        if(_curButtonNumber == buttonNumber) { return;} // 같은 Search버튼을 누를 경우
        else { _firstStart = true; } // 다른 Search버튼을 누를 경우
        _curButtonNumber = buttonNumber;
        _unitButton = _dicAddedUnitChip[buttonNumber];
        _resultNameText.text = _searchButtonNames[buttonNumber].type.ToString();

        _unitPropertyMng.ClearAllUnitButtonOrder(_allUnitButtonsOrder);
        ClearUnitButton();
        SetAllUnitProperty();

        if (_firstStart)
        {
            SelectedUnitPopController.Instance.SetActiveOfSelectedUnitPop(false);

            if (_dicAddedUnitChip[buttonNumber].Length > 9)
            {
                ToggleButtonSystem.SetActiveRadioBox(unitChipSearchRadioBox, 4, true);
            }
            else
            {
                ToggleButtonSystem.SetActiveRadioBox(unitChipSearchRadioBox,4, false);
            }
            ToggleButtonSystem.IsOnUnitRadioBox(unitChipSearchRadioBox, _currentRadioIndex[buttonNumber] + 1, true);
            ToggleButtonSystem.IsOnUnitRadioBox(propertySortRadioBox, 1, true);
            OnClickPropertySortRadioBoxButton(0);
            OnClickUnitRadioBoxButton(_currentRadioIndex[buttonNumber]);
            _firstStart = false;
        }

    }

    private int[] _currentRadioIndex = new int[4];
    public void OnClickUnitRadioBoxButton(int index)
    {
        _currentRadioIndex[_curButtonNumber] = index;
        SetAllUnitTransform(_allUnitButtonsOrder);
        _unitPropertyMng.ActiveAllPropertyUnit(_allUnitButtonsOrder, false);
        _unitPropertyMng.ActivePropertyUnit(_currentRadioIndex[_curButtonNumber], _allUnitButtonsOrder, true);
        
    }

    public List<GameObject> _allUnitButtonsOrder = new();


    public void OnClickPropertySortRadioBoxButton(int propertyIndex)
    {
        SortPropertyProcess((UnitProperty)propertyIndex, _arrPropertyOrder, _allUnitButtonsOrder);
    }

    private void SortPropertyProcess(UnitProperty property, UnitProperty[] arrPropertyOrder, List<GameObject> allUnitButtonsOrder)
    {
        _unitPropertyMng.SetPropertyOrder(property, arrPropertyOrder);

        _unitPropertyMng.ClearAllUnitButtonOrder(allUnitButtonsOrder);
        SetAllUnitButtonsOrder(arrPropertyOrder, allUnitButtonsOrder);
        
        SetAllUnitTransform(allUnitButtonsOrder);
        _unitPropertyMng.ActiveAllPropertyUnit(allUnitButtonsOrder, false);
        _unitPropertyMng.ActivePropertyUnit(0, allUnitButtonsOrder, true);

        ToggleButtonSystem.IsOnUnitRadioBox(unitChipSearchRadioBox, 1, true);

    }

    [SerializeField]
    private UnitChipTransformData _unitChipTransformData;
    private float addValuePosX;
    public void SetAllUnitTransform(List<GameObject> allUnitButtonsOrder)
    {
        addValuePosX = 0;
        RectTransform[] rectTrasform = allUnitButtonsOrder.Select(gameObject => gameObject.GetComponent<RectTransform>()).ToArray();
        for (int i = 0; i < rectTrasform.Length; i++)
        {
            rectTrasform[i].anchorMin = new Vector2(0.5f, 0.5f);
            rectTrasform[i].anchorMax = new Vector2(0.5f, 0.5f);
            rectTrasform[i].anchoredPosition = new Vector3(_unitChipTransformData._posX + (addValuePosX * 170f), _unitChipTransformData._posY, _unitChipTransformData._posZ);
            rectTrasform[i].sizeDelta = new Vector2(_unitChipTransformData._width, _unitChipTransformData._height);
            rectTrasform[i].localScale = new Vector3(_unitChipTransformData._scaleX, _unitChipTransformData._scaleY, _unitChipTransformData._scaleZ);

            addValuePosX++;
            if (addValuePosX == 3)
            {
                addValuePosX = 0f;
            }
        }
    }

    private void SetAllUnitButtonsOrder(UnitProperty[] arrPropertyOrder, List<GameObject> allUnitButtonsOrder)
    {
        for (int i = 0; i < arrPropertyOrder.Length; i++)
        {
            for (int j = 0; j < _currentPropertyUnitChipsData.Length; j++)
            {
                if (_currentPropertyUnitChipsData[j]._property == _arrPropertyOrder[i])
                {
                    AddUnitButtonToAllUnitButtonOrder(_currentPropertyUnitChipsData[j]._propertySuperObj, allUnitButtonsOrder);
                    break;
                }
            }
        }
    }

    private void AddUnitButtonToAllUnitButtonOrder(Transform propertySuperObj, List<GameObject> allUnitButtonsOrder)
    {
        for (int i = 0; i < propertySuperObj.childCount; i++)
        {
            allUnitButtonsOrder.Add(propertySuperObj.GetChild(i).gameObject);
        }
    }


    public void AddUnitButton(UnitChipState unitButton, UnitProperty proerty)
    {
     
        if (unitButton == null) { Debug.Log("유닛버튼이 비어있습니다."); }
        for (int i = 0; i < _currentPropertyUnitChipsData.Length; i++)
        {
            if (_currentPropertyUnitChipsData[i]._property == proerty)
            {
                _currentPropertyUnitChipsData[i]._unitButtons.Add(unitButton);
                return;
            }
        }
    }

    public void SetUnitProperty(UnitProperty property)
    {
        
        UnitChipState[] unitButton = _unitButton;
        for (int i = 0; i < unitButton.Length; i++)
        {
            if(unitButton[i].TryGetComponent<UnitChipState>(out UnitChipState unitChipState))
            {
                UnitData unitData = unitChipState.GetUnitData();
                if (unitData.Property == property)
                {
                    AddUnitButton(unitButton[i], property);
                //    _unitButton = _unitButton.Where((gameObject, index) => _unitButton[index] != unitButton[i]).ToArray();
                }
            }
   
        }
        CreateUnitButton(property);
    }

    public void SetAllUnitProperty()
    {
        for (int i = 0; i < _arrPropertyOrder.Length; i++)
        {
            SetUnitProperty(_arrPropertyOrder[i]);
        }
    }

    public void CreateUnitButton(UnitProperty proerty)
    {
        for (int i = 0; i < _currentPropertyUnitChipsData.Length; i++)
        {
            if (_currentPropertyUnitChipsData[i]._property == proerty)
            {
                CreateUnitButton(_currentPropertyUnitChipsData[i]._unitButtons, _currentPropertyUnitChipsData[i]._propertySuperObj);
                return;
            }
        }
    }

    private void CreateUnitButton(List<UnitChipState> unitButtons, Transform propertySuperObj)
    {
        GameObject unitButton;
        foreach (UnitChipState FireUnitButton in unitButtons)
        {
            unitButton = Instantiate(FireUnitButton.gameObject, propertySuperObj);
            unitButton.name = unitButton.name.Replace("(Clone)", "");
        }
    }

    public SearchButtonName[] GetSearchButtonNames()
    {
        return _searchButtonNames;
    }

}
