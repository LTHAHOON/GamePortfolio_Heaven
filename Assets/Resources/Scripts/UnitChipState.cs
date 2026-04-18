using UnityEngine;

public class UnitChipState : MonoBehaviour
{
    [SerializeField]
    private bool _isAddedUnitChip;
    [SerializeField]
    private StatusComponent _statusComponent;
    [Space(2f)]
    [Header("Home Extent(면적)")]
    [SerializeField]
    private float _horizontalLength;
    [SerializeField]
    private float _verticalLength;
    [Header("Home Function(기능 설명)")]
    [SerializeField]
    private string _functionDecription;

    public void SetIsAddedUnitChip(bool isAddedUnitChip)
    {
        _isAddedUnitChip = isAddedUnitChip;
    }
    public bool GetIsAddedUnitChip()
    {
        return _isAddedUnitChip;
    }

    public float GetHorizontalLength()
    {
        return _horizontalLength;
    }

    public float GetVerticalLength()
    {
        return _verticalLength;
    }

    public string GetFunctionDecription()
    {
        return _functionDecription;
    }

    public UnitData GetUnitData()
    {
        return _statusComponent.GetUnitData();
    }
}
