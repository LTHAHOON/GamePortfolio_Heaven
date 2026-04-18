using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnitSearchResultController;

public class UnitEditController : MonoBehaviour
{
    [SerializeField]
    private GameObject _unitEditObject;
    [SerializeField]
    private TextMeshProUGUI _title_Added;
    [SerializeField]
    private TextMeshProUGUI _title_Remaining;

    [SerializeField]
    private GameObject _InvenBackGround_Added;
    [SerializeField]
    private GameObject _InvenBackGround_Remaining;

    [SerializeField]
    private GameObject _remaingingUnitChipInvenViewPort;
    [SerializeField]
    private GameObject _addedUnitChipInvenViewPort;

    [SerializeField]
    private GameObject[] _arrRadioBoxObj;

    private List<GameObject> _remainingContentGroupParentList = new();
    private List<GameObject> _addedContentGroupParentList =new();

    [SerializeField]
    private GameObject _contentGroupPrefab;
    [SerializeField]
    private Unit_MyRoomMng _myRoomMng;
    [SerializeField]
    private UnitPropertyMng _unitPropertyMng;
    private SearchButtonName[] _searchButtonNames;
    [SerializeField]
    private UnitProperty[] _arrPropertyOrder =
    {
        UnitProperty.FireProperty, UnitProperty.WaterProperty, UnitProperty.GroundProperty, UnitProperty.ElectricityProperty
    };
    private void Start()
    {
        InitializeUnitEditControl();
    }

    private IRadioBoxes unitEditSearchRadioBox;
    private IRadioBoxes propertySortRadioBoxList_Added;
    private IRadioBoxes propertySortRadioBoxList_Remaining;

    private void InitializeUnitEditControl()
    {
        InitializeList();
        _searchButtonNames = UnitSearchResultController.Instance.GetSearchButtonNames();
        UnitChipState[] unitChipState = _myRoomMng.GetAllUnitChips(false);
        for (int i = 0; i < _searchButtonNames.Length; ++i)
        {
            CreateUnitChipEdit(unitChipState, i);
        }
        OnClickUnitEditSearchButton(0);
        RefreshAllUnitButtonOrder(propertySortRadioBoxList_Added, true);
        RefreshAllUnitButtonOrder(propertySortRadioBoxList_Remaining, true);
    }
    private void InitalizeVariable()
    {
        if (_arrRadioBoxObj.Length > 0)
        {
            for (int i = 0; i < _arrRadioBoxObj.Length; i++)
            {
                if (_arrRadioBoxObj[i].TryGetComponent(out UnitEditSearchRadioBoxControl radioBox))
                {
                    unitEditSearchRadioBox = radioBox;
                }
                else if (_arrRadioBoxObj[i].TryGetComponent(out PropertySortRadioBoxControl radioBox2))
                {
                    if(_arrRadioBoxObj[i].tag == _addedUnitChipInvenViewPort.tag)
                    {
                        propertySortRadioBoxList_Added = radioBox2;
                    }
                    else
                    {
                        propertySortRadioBoxList_Remaining = radioBox2;
                    }
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


    private void Update()
    {
        if (_unitEditObject.activeSelf == false) { return; }
        DragAndDrop();
    }

    private void DragAndDrop()
    {
        
        if (Input.GetMouseButton(0))
        {
            DragProcess();
        }
        else
        {
            DropProcess();
        }
    }

    private void InitializeList()
    {
        for (int i = 0; i < 3; i++)
        {
            _dicContentGroupList_Added.Add((UnitType)i, new List<GameObject>());
            _dicContentGroupList_Remaining.Add((UnitType)i, new List<GameObject>());
            _changedUnitChipsData.Add((UnitType)i, new());
        }

        Transform remainTransform = _remaingingUnitChipInvenViewPort.transform;
        Transform addedTransform = _addedUnitChipInvenViewPort.transform;

        for (int i = 0; i < remainTransform.childCount; i++)
        {
            _remainingContentGroupParentList.Add(remainTransform.GetChild(i).gameObject);
        }
        for (int i = 0; i < addedTransform.childCount; i++)
        {
            _addedContentGroupParentList.Add(addedTransform.GetChild(i).gameObject);
        }
    }

  


    private GameObject contentParent;
    private Vector3 offestPoint;
    private UnitChipState selectedUnitChip;
    private List<RaycastResult> raycastResult = new List<RaycastResult>();
    private void DragProcess()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        if (raycastResult.Count > 0)
        {
            if (contentParent.tag == "contentGroup")
            {
                selectedUnitChip.GetComponent<Button>().interactable = false;
                contentParent.GetComponent<HorizontalLayoutGroup>().enabled = false;

                if (selectedUnitChip.TryGetComponent<RectTransform>(out var rectTransform))
                {
                    //A(마우스위치) - C(오프셋) = B(버튼위치)  ->  C'(이동 오프셋)+ A(마우스위치) - C(오프셋) = B(버튼위치) + C'(이동 오프셋)
                    rectTransform.transform.position = Input.mousePosition - offestPoint;
                }
            }
            else
            {
                if (raycastResult.Count > 0)
                {
                    raycastResult.Clear();
                }
            }
        }
        else
        {
            EventSystem.current.RaycastAll(pointerData, raycastResult);
            if (raycastResult.Count > 0)
            {
                if(raycastResult[0].gameObject.TryGetComponent(out UnitChipState unitChipState))
                {
                    selectedUnitChip = unitChipState;
                    //A(마우스 위치) - B(버튼위치) = C(오프셋) 
                    offestPoint = Input.mousePosition - selectedUnitChip.transform.position;
                    contentParent = selectedUnitChip.transform.parent.gameObject;
                }
                else
                {
                    raycastResult.Clear();
                }
            }
        }
    }

    private void DropProcess()
    {
        if (contentParent != null && raycastResult.Count > 0)
        {
            if (selectedUnitChip.TryGetComponent(out Button buttonComponent))
            {
                buttonComponent.interactable = true;
            }

            if (contentParent.TryGetComponent(out HorizontalLayoutGroup layoutGroup))
            {
                layoutGroup.enabled = true;
            }

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            EventSystem.current.RaycastAll(pointerData, raycastResult);

            if (selectedUnitChip.TryGetComponent(out UnitChipState selectedUnitChipState))
            {
                if (selectedUnitChipState.GetIsAddedUnitChip()) // 선택한 유닛칩이 추가된 유닛칩인 경우
                {
                    SetDropSlot(selectedUnitChip, _dicContentGroupList_Remaining, _remainingContentGroupParentList[_searchButtonIndex], _InvenBackGround_Remaining);
                }
                else // 선택한 유닛칩이 추가된 유닛칩인 아닐 경우
                {
                    SetDropSlot(selectedUnitChip, _dicContentGroupList_Added, _addedContentGroupParentList[_searchButtonIndex], _InvenBackGround_Added);
                }
            }
        }
        if (raycastResult.Count > 0)
        {
            raycastResult.Clear();
        }

    }

    private Dictionary<UnitType, List<UnitChipState>> _changedUnitChipsData = new();
    private void SetDropSlot(UnitChipState unitChip ,  Dictionary<UnitType, List<GameObject>> contentGroupListToMove, GameObject contentGroupParentToMove, GameObject slot)
    {
        GameObject[] arrRaycastResult = raycastResult.Select((result) => result.gameObject).ToArray();

        if (CheckInvenBackGround(arrRaycastResult, slot)) // 마우스 위치가 이동할 InvenBackGround일 경우
        {
            bool bContain = _changedUnitChipsData[_searchButtonType].Contains(unitChip);
            if (!bContain)
            {
                _changedUnitChipsData[_searchButtonType].Add(unitChip);
            }
            else
            {
                _changedUnitChipsData[_searchButtonType].Remove(unitChip);
            }
            ModfiyUnitChipProcess(unitChip, contentGroupListToMove, contentGroupParentToMove, false);
        }
    }

    private void ModfiyUnitChipProcess(UnitChipState unitChip, Dictionary<UnitType, List<GameObject>> contentGroupListToMove, GameObject contentGroupParentToMove, bool isUnDO)
    {
        Dictionary<UnitType, List<GameObject>> otherContentGroupList = contentGroupListToMove == _dicContentGroupList_Added ? _dicContentGroupList_Remaining : _dicContentGroupList_Added;

        int contentGroupListIndexToArrage = 0;
        if (isUnDO == false)
        {
            //TODO: 이동 전의 선택된 유닛칩이 있는 contentGroup의 contentGroupList 인덱스 구하기
            contentGroupListIndexToArrage = FindContentGroupListIndex(selectedUnitChip, otherContentGroupList);
        }


        //TODO: 유닛칩 수정하기(이동시키기)
        ModifyUnitChip(unitChip, contentGroupListToMove, contentGroupParentToMove);

        if (isUnDO == false)
        {
            //TODO: 유닛칩이 이동할 경우 이동 전의 위치 순서 정리
            ArrangeUnitChip(otherContentGroupList, contentGroupListIndexToArrage);
        }


        //TODO: UnitSearchResult 새로고침
        UnitSearchResultController.Instance.SetAddedUnitChip(ToggleButtonSystem.FindRadioBoxIndexToIsOn(unitEditSearchRadioBox));

        //TODO: 속성 배열하기
        RefreshAllUnitButtonOrder(propertySortRadioBoxList_Added, true);
        RefreshAllUnitButtonOrder(propertySortRadioBoxList_Remaining, true);
    }


    private void ArrangeUnitChip( Dictionary<UnitType, List<GameObject>> contentGroupListToArrage, int contentGroupListIndexToArrange)
    {
        GameObject lastContentGroup = FindLastContentGroup(contentGroupListToArrage);
        int contentGroupListMaxIndex = 0;
        if (lastContentGroup.transform.childCount > 0)
        {
            if(lastContentGroup.transform.GetChild(0).gameObject.TryGetComponent(out UnitChipState unitChipState))
            {
                contentGroupListMaxIndex = FindContentGroupListIndex(unitChipState, contentGroupListToArrage);
                if (contentGroupListMaxIndex < 0) { return; }
            }
        }
        else
        {
            return;
        }

        for (int i = contentGroupListIndexToArrange; i <= contentGroupListMaxIndex; i++)
        {
            int nextIndex = i + 1;
            if(nextIndex <= contentGroupListMaxIndex)
            {
                contentGroupListToArrage[_searchButtonType][nextIndex].transform.GetChild(0).transform.SetParent(contentGroupListToArrage[_searchButtonType][i].transform);
            }
        }

        for (int i = 0; i <= contentGroupListMaxIndex; i++)
        {
            if(contentGroupListToArrage[_searchButtonType][i].transform.childCount <= 0)
            {
                RemoveContentGroup(contentGroupListToArrage, contentGroupListToArrage[_searchButtonType][i]);
            }
        }
    }

    private bool CheckInvenBackGround(GameObject[] arrRaycastResult, GameObject InvenBackGroundToFind)
    {
        for (int i = 0; i < arrRaycastResult.Length; i++)
        {
            if (arrRaycastResult[i] == InvenBackGroundToFind) { return true; }
        }

        Debug.Log("Not InvenBackGround Position");
        return false;
    }

    private void ModifyUnitChip(UnitChipState unitChipToModify ,  Dictionary<UnitType, List<GameObject>> contentGroupListOfUnitChip,  GameObject contentGroupParentOfUnitChip)
    {
        GameObject lastContentGroup = FindLastContentGroup(contentGroupListOfUnitChip);
        int contentGroupChildCount = lastContentGroup.transform.childCount;
        bool isAddedUnitChip = !unitChipToModify.GetIsAddedUnitChip();
        unitChipToModify.GetComponent<UnitChipState>().SetIsAddedUnitChip(isAddedUnitChip);

        //TODO: 유닛칩 이동 전: 이동 후의 위치 생성과정
        if (contentGroupChildCount >= 3)
        {
            CreateContentGroup(contentGroupListOfUnitChip, contentGroupParentOfUnitChip, _searchButtonType);
            lastContentGroup = FindLastContentGroup(contentGroupListOfUnitChip);
        }

        //TODO: 유닛칩 이동
        MoveUnitChip(unitChipToModify, lastContentGroup, isAddedUnitChip);

        //TODO: 유닛칩 이동 후: 이동 전의 위치 삭제과정
        Dictionary<UnitType, List<GameObject>> otherContentGroupList = contentGroupListOfUnitChip == _dicContentGroupList_Added ? _dicContentGroupList_Remaining : _dicContentGroupList_Added;
        GameObject lastOtherContentGroup = FindLastContentGroup(otherContentGroupList);
        int otherContentGroupChildCount = lastOtherContentGroup.transform.childCount;
        if (otherContentGroupChildCount <= 0 && otherContentGroupList[_searchButtonType].Count > 1)
        {
            RemoveContentGroup(otherContentGroupList, lastOtherContentGroup);
        }
    }


    private void MoveUnitChip(UnitChipState unitChipToEdit, GameObject contentGroup, bool isAdding)
    {
        if(isAdding)
        {
            _myRoomMng.AddUnitChip(unitChipToEdit, _myRoomMng.FindAddedUnitChipWithType);
            unitChipToEdit.transform.SetParent(contentGroup.transform);
            _myRoomMng.RemoveUnitChip(unitChipToEdit, _myRoomMng.FindRemaingUnitChipWithType);
        }
        else
        {
            _myRoomMng.AddUnitChip(unitChipToEdit, _myRoomMng.FindRemaingUnitChipWithType);
            unitChipToEdit.transform.SetParent(contentGroup.transform);
            _myRoomMng.RemoveUnitChip(unitChipToEdit, _myRoomMng.FindAddedUnitChipWithType);
        }

    }

    private Dictionary<UnitType, List<GameObject>> _dicContentGroupList_Remaining = new();
    private Dictionary<UnitType, List<GameObject>> _dicContentGroupList_Added = new();
    public void CreateUnitChipEdit(UnitChipState[] allUnitPrefabTemp, int searchButtonIndex)
    {
        for (int i = 0; i < allUnitPrefabTemp.Length; i++)
        {
            UnitChipState unitChipState = allUnitPrefabTemp[i];
            UnitData unitData = unitChipState.GetUnitData();
            if (unitData.Type == _searchButtonNames[searchButtonIndex].type)
            {
                if (unitChipState.GetIsAddedUnitChip() == true)
                {
                    UnitChipState InstantUnit = CreateInstantUnitChip(unitChipState, _dicContentGroupList_Added, _addedContentGroupParentList[searchButtonIndex], searchButtonIndex);
                    _myRoomMng.AddUnitChip(InstantUnit, _myRoomMng.FindAddedUnitChipWithType);
                }
                else
                {
                    UnitChipState InstantUnit = CreateInstantUnitChip(unitChipState, _dicContentGroupList_Remaining, _remainingContentGroupParentList[searchButtonIndex], searchButtonIndex);
                    _myRoomMng.AddUnitChip(InstantUnit, _myRoomMng.FindRemaingUnitChipWithType);
                }
                _myRoomMng.RemoveUnitOfAllUnitChip(unitChipState);
            }

        }

        //초기화
        contentGroupIndex_Added = 0;
        contentGroupIndex_Remaining = 0;
        count_Added = 0;
        count_Remaing = 0;

    }

    private GameObject FindLastContentGroup( Dictionary<UnitType, List<GameObject>> contentGroupList)
    {
        if(contentGroupList[_searchButtonType].Count <= 0)
        {
            GameObject contentGroupParent = contentGroupList == _dicContentGroupList_Added ? _addedContentGroupParentList[_searchButtonIndex] : _remainingContentGroupParentList[_searchButtonIndex];
            CreateContentGroup(contentGroupList, contentGroupParent, _searchButtonType);
        }
        if (contentGroupList[_searchButtonType][contentGroupList[_searchButtonType].Count - 1] == null)
        {
            Debug.Log("ERROR : _contentGroupList == null");
        }
        return contentGroupList[_searchButtonType][contentGroupList[_searchButtonType].Count - 1];
    }

    private int FindContentGroupListIndex(UnitChipState unitChip,  Dictionary<UnitType, List<GameObject>> contentGroupList)
    {
        for (int i = 0; i < contentGroupList[_searchButtonType].Count; i++)
        {
            for(int j = 0; j < contentGroupList[_searchButtonType][i].transform.childCount; j++)
            {
                if (unitChip.gameObject == contentGroupList[_searchButtonType][i].transform.GetChild(j).gameObject)
                {
                    return i;
                }
            }
        }

        Debug.Log("Failed FindContentGrop()");
        return -1;
    }

    private void CreateContentGroup( Dictionary<UnitType, List<GameObject>> conetnGroupList, GameObject conentGroupParent, UnitType searchButtonIndex)
    {
        var contentGroup = Instantiate(_contentGroupPrefab, conentGroupParent.transform);

        conetnGroupList[searchButtonIndex].Add(contentGroup);
    }

    private void RemoveContentGroup( Dictionary<UnitType, List<GameObject>> conetnGroupList, GameObject contentGrouptoDel)
    {
        conetnGroupList[_searchButtonType].Remove(contentGrouptoDel);

        Destroy(contentGrouptoDel);
    }

    private int contentGroupIndex_Added = 0;
    private int contentGroupIndex_Remaining = 0;
    private int count_Added = 0;
    private int count_Remaing = 0;
    private const int currentCountLimit = 3;
    private UnitChipState CreateInstantUnitChip(UnitChipState unitChip,  Dictionary<UnitType, List<GameObject>> contentGroupListOfUnitChip, GameObject contentGroupParentOfUnitChip, int searchButtonIndex) // 초기 인스턴스 유닛칩 생성
    {
        int currentCount = contentGroupParentOfUnitChip == _addedContentGroupParentList[searchButtonIndex] ? count_Added : count_Remaing;
        int curContentGroupIndex = contentGroupParentOfUnitChip == _addedContentGroupParentList[searchButtonIndex] ? contentGroupIndex_Added : contentGroupIndex_Remaining;

        if (currentCount == 0 || currentCount >= currentCountLimit)
        {
            if (currentCount >= currentCountLimit)
            {
                ++curContentGroupIndex;
                currentCount = 0;
            }
            CreateContentGroup(contentGroupListOfUnitChip, contentGroupParentOfUnitChip, (UnitType)searchButtonIndex);
        }
        ++currentCount;

        if (contentGroupParentOfUnitChip == _addedContentGroupParentList[searchButtonIndex])
        {
            count_Added = currentCount;
            contentGroupIndex_Added = curContentGroupIndex;
        }
        else
        {
            count_Remaing = currentCount;
            contentGroupIndex_Remaining = curContentGroupIndex;
        }

        RectTransform rect = unitChip.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(190f, 113f);
        rect.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        UnitChipState InstantObj = Instantiate(unitChip, contentGroupListOfUnitChip[(UnitType)searchButtonIndex][curContentGroupIndex].transform);
        InstantObj.name = InstantObj.name.Replace("Clone", "");
        return InstantObj;
    }

    private int _searchButtonIndex;
    private UnitType _searchButtonType;

    private void SetActiveContentParent(List<GameObject> contentParent, int contentGroupParentIndex, bool isActive)
    {
        contentParent[contentGroupParentIndex].SetActive(isActive);
    }

    private void EnableOnlyContentGroup(int contentGroupParentIndex)
    {
        for (int index = 0; index < _searchButtonNames.Length - 1; index++)
        {
            if(index == contentGroupParentIndex)
            {
                SetActiveContentParent(_addedContentGroupParentList, index, true);
                SetActiveContentParent(_remainingContentGroupParentList, index, true);
            }
            else
            {
                SetActiveContentParent(_addedContentGroupParentList, index, false);
                SetActiveContentParent(_remainingContentGroupParentList, index, false);
            }
        }
        
    }


    public void OnClickUnitEditSearchButton(int index)
    {
        InitalizeVariable();
        

        List<Toggle> radioBoxList = unitEditSearchRadioBox.GetRadioBoxList();
        if (index < radioBoxList.Count || index >= 0)
        {
            int indexToIsOn = ToggleButtonSystem.FindRadioBoxIndexToIsOn(unitEditSearchRadioBox);
            if(index == indexToIsOn)
            {
                _searchButtonType = (UnitType)index;
                _searchButtonIndex = index;
                _title_Remaining.text = $"남은<color=#00EE06> {_searchButtonNames[index].name}</color>";
                _title_Added.text = $"추가된<color=#00EE06> {_searchButtonNames[index].name}</color>";

                EnableOnlyContentGroup(index);
                UnitSearchResultController.Instance.SetAddedUnitChip(index);

                InitalizeAllUnitButtonOrder(propertySortRadioBoxList_Added, 0);
                InitalizeAllUnitButtonOrder(propertySortRadioBoxList_Remaining, 0);
            }
        }
        else
        {
            Debug.Log("WRONG Index: OnClickUnitEditSearchButton()");
        }

    }


    private void RefreshAllUnitButtonOrder(IRadioBoxes radioBox, bool forceToRun)
    {
        _isAddedInven = radioBox == propertySortRadioBoxList_Added ? true : false;
        int radioBoxListIndex = ToggleButtonSystem.FindRadioBoxIndexToIsOn(radioBox);
        _forceToRun = forceToRun;
        OnClickPropertySortRadioBoxButton(radioBoxListIndex);
        _forceToRun = false;
    }

    private void InitalizeAllUnitButtonOrder(IRadioBoxes radioBox, int radioBoxListIndex)
    {
        _allUnitButtonsOrder.Clear();
        _arrPropertyOrder = new UnitProperty[]
        {
            UnitProperty.FireProperty, UnitProperty.WaterProperty, UnitProperty.GroundProperty, UnitProperty.ElectricityProperty
        };

        _isAddedInven = radioBox == propertySortRadioBoxList_Added ? true : false;
        _forceToRun = true;
        OnClickPropertySortRadioBoxButton(radioBoxListIndex);
        _forceToRun = false;

        int sequence = radioBoxListIndex + 1;
        ToggleButtonSystem.IsOnUnitRadioBox(radioBox, sequence, true);
    }

    public void OnClickPropertySortRadioBoxButton(GameObject radioBox)
    {
        if(radioBox.tag == _addedUnitChipInvenViewPort.tag)
        {
            _isAddedInven = true;
        }
        else
        {
            _isAddedInven = false;
        }
    }


    public List<GameObject> _allUnitButtonsOrder = new();
    private bool _isAddedInven;
    private bool _forceToRun = false;
    public void OnClickPropertySortRadioBoxButton(int propertyIndex)
    {
        if (_arrPropertyOrder.Length > 0 && _forceToRun == false)
        {
            if (_arrPropertyOrder[0] == (UnitProperty)propertyIndex) //같은 속성 배열일 경우
            {
                return;
            }
        }

        SortPropertyProcess((UnitProperty)propertyIndex, _arrPropertyOrder, _allUnitButtonsOrder);
    }

    private void SortPropertyProcess(UnitProperty property, UnitProperty[] arrPropertyOrder, List<GameObject> allUnitButtonsOrder)
    {
        _unitPropertyMng.SetPropertyOrder(property, arrPropertyOrder);
        _unitPropertyMng.ClearAllUnitButtonOrder(allUnitButtonsOrder);
        SetAllUnitButtonsOrder(arrPropertyOrder, allUnitButtonsOrder);

        _unitPropertyMng.ActiveAllPropertyUnit(allUnitButtonsOrder, false);
        ChangeToAllUnitButtonOrder(allUnitButtonsOrder);
        _unitPropertyMng.ActiveAllPropertyUnit(allUnitButtonsOrder, true);

    }

    private void SetAllUnitButtonsOrder(UnitProperty[] arrPropertyOrder, List<GameObject> allUnitButtonsOrder)
    {
        for (int i = 0; i < arrPropertyOrder.Length; i++)
        {
            AddUnitButtonToAllUnitButtonOrder(arrPropertyOrder[i], allUnitButtonsOrder);
        }
    }

    private void AddUnitButtonToAllUnitButtonOrder(UnitProperty property, List<GameObject> allUnitButtonsOrder)
    {
        Dictionary<UnitType, List<GameObject>> contentGroupList = _isAddedInven ? _dicContentGroupList_Added : _dicContentGroupList_Remaining;
      
        foreach (var contentGroup in contentGroupList[_searchButtonType])
        {
            for (int i = 0; i < contentGroup.transform.childCount; i++)
            {
                GameObject unitButton = contentGroup.transform.GetChild(i).gameObject;
                if(unitButton.TryGetComponent(out UnitChipState unitChipState))
                {
                    UnitData unitData = unitChipState.GetUnitData();
                    if (unitData.Property == property)
                    {
                        allUnitButtonsOrder.Add(unitButton);
                    }
                }
                
            }
        }
    }

    private void ChangeToAllUnitButtonOrder(List<GameObject> allUnitButtonsOrder)
    {
        Dictionary<UnitType, List<GameObject>> contentGroupList = _isAddedInven ? _dicContentGroupList_Added : _dicContentGroupList_Remaining;

        foreach (var contentGroup in contentGroupList[_searchButtonType])
        {
            contentGroup.transform.DetachChildren();
        }

        int orderCount = 3;
        int orderIndex = 0;
        foreach (var contentGroup in contentGroupList[_searchButtonType])
        {
            for (int i = 0; i < orderCount; i++)
            {
                if (orderIndex < allUnitButtonsOrder.Count)
                {
                    allUnitButtonsOrder[orderIndex].transform.SetParent(contentGroup.transform);
                    ++orderIndex;
                }
                
            }
        }
    }

    public void OnClickCloseButton()
    {
        for (int i = 0; i < _changedUnitChipsData.Count; i++)
        {
            _changedUnitChipsData[(UnitType)i].Clear();
        }

        _myRoomMng.ClearUnDoAllUnitChip();
        GameObject window = transform.GetChild(0).gameObject;
        ButtonSystem.buttonInvoker.PressButton(ButtonIdentifier.Close, window);
    }

    public void OnClickOpenButton()
    {
        SelectedUnitPopController.Instance.SetActiveOfSelectedUnitPop(false);

        _myRoomMng.AddToUnDoAllUnitChip();
        GameObject window = transform.GetChild(0).gameObject;
        ButtonSystem.buttonInvoker.PressButton(ButtonIdentifier.Open, window);
        int searchButtonIndex = UnitSearchResultController.Instance.GetCurrentButtonNumber();
        ToggleButtonSystem.IsOnUnitRadioBox(unitEditSearchRadioBox, searchButtonIndex + 1, true);
        OnClickUnitEditSearchButton(searchButtonIndex);
    }

    public void OnClickUnitEditUnDoButton()
    {
        UnDoUnitChipButton();
    }

    private void UnDoUnitChipButton()
    {
        int changedCount = _changedUnitChipsData[_searchButtonType].Count;
        int unDoAllUnitChipCount = _myRoomMng._unDoAllUnitChip.Count;
        int removeCount = 0;    
        for (int i = 0; i < changedCount; i++)
        {
            for (int j = 0; j < unDoAllUnitChipCount; j++)
            {
                if (_changedUnitChipsData[_searchButtonType][i - removeCount] == _myRoomMng._unDoAllUnitChip[j])
                {
                    if (_changedUnitChipsData[_searchButtonType][i - removeCount].TryGetComponent(out UnitChipState unitChipState))
                    {
                        if (unitChipState.GetIsAddedUnitChip()) // 선택한 유닛칩이 추가된 유닛칩인 경우
                        {
                            ModfiyUnitChipProcess(_changedUnitChipsData[_searchButtonType][i - removeCount], _dicContentGroupList_Remaining, _remainingContentGroupParentList[_searchButtonIndex], true);
                        }
                        else // 선택한 유닛칩이 추가된 유닛칩인 아닐 경우
                        {
                            ModfiyUnitChipProcess(_changedUnitChipsData[_searchButtonType][i - removeCount], _dicContentGroupList_Added, _addedContentGroupParentList[_searchButtonIndex], true);
                        }
                    }
                    _changedUnitChipsData[_searchButtonType].RemoveAt(i - removeCount);
                    ++removeCount;
                    break;
                }
            }
        }
        Debug.Log("UnDo UnitChip");

    }
}
