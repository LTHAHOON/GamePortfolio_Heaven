using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealhStatusData", menuName = "StatusData/HealhStatusData", order = 2)]
public class UnitHealhStatusData : BaseUnitStatusData
{
    public AllStatusNames[] allStatusName =
    {
        AllStatusNames.CON,
        AllStatusNames.HealingPower,
    };
    public override AllStatusNames[] GetAllStatusNames()
    {
        return allStatusName;
    }
    public int CON;
    public int HealingPower;
}
