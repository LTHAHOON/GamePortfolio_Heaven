using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PassengerController : Unit
{
    private Dictionary<long, PassengerData> _passengerDatas = new();
    protected List<PassengerData> GetPassengerDatas()
    {
        return _passengerDatas.Values.ToList();
    }

    protected void SetPassengerInData(CreatureFSM creature, int passengerCount)
    {
        if(!_passengerDatas.ContainsKey(creature.GetID()))
        {
            _passengerDatas.Add(creature.GetID(), new PassengerData(creature, passengerCount));
        }

    }
    protected CreatureFSM[] GetPassengers()
    {
        return _passengerDatas.Values.Select(v => v._passenger).ToArray();
    }
    protected void ClearPassengerDatas()
    {
        _passengerDatas.Clear();
    }
    protected int GetPassengerCountInData(long id)
    {
        return _passengerDatas[id]._passengerCount;
    }
}
