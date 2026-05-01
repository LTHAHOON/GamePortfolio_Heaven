using UnityEngine;

public class UnitChipState : MonoBehaviour
{
    [SerializeField]
    private bool _isAddedUnitChip;
    [SerializeField]
    private UnitInfo unitInfo;

    public void SetIsAddedUnitChip(bool isAddedUnitChip)
    {
        _isAddedUnitChip = isAddedUnitChip;
    }
    public bool GetIsAddedUnitChip()
    {
        return _isAddedUnitChip;
    }

    public UnitInfo GetUnitData()
    {
        return unitInfo;
    }
}
