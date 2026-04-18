using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatusData", menuName = "StatusData/UnitStatusData", order = 1)]
public class UnitStatusData : BaseUnitStatusData
{
    public AllStatusNames[] allStatusName =
    {
        AllStatusNames.ATK,
        AllStatusNames.DEF,
        AllStatusNames.HIT,
        AllStatusNames.Critical,
        AllStatusNames.DEX,
        AllStatusNames.CON,
        AllStatusNames.HealingPower,
    };


    public override AllStatusNames[] GetAllStatusNames()
    {
        return allStatusName;
    }
    public int ATK;
    public int DEF;
    public int HIT;
    public int Critical;
    public int DEX;
    public int CON;
    public int HealingPower;
}
