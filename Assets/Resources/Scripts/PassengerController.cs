using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PassengerController : Unit
{
    private Dictionary<long, PassengerData> _dicPassengerDatas = new();
    public List<PassengerData> GetPassengerDatas()
    {
        return _dicPassengerDatas.Values.ToList();
    }

    protected void AddPassengerInData(Unit creature, int passengerCount)
    {
        if(_dicPassengerDatas.ContainsKey(creature.ID))
        {
            _dicPassengerDatas[creature.ID]._passengerCount += passengerCount;
        }
        else
        {
            _dicPassengerDatas.Add(creature.ID, new PassengerData(creature, passengerCount));
        }

    }


    protected void RemovePassengerInData(Creature creature)
    {
        if(_dicPassengerDatas.ContainsKey(creature.ID))
        {
            _dicPassengerDatas.Remove(creature.ID);
        }
    }
    protected Unit[] GetPassengers()
    {
        return _dicPassengerDatas.Values.Select(v => v._passenger).ToArray();
    }
    protected void ClearPassengerDatas()
    {
        _dicPassengerDatas.Clear();
    }
    protected int GetPassengerCountInData(long id)
    {
        return _dicPassengerDatas[id]._passengerCount;
    }

    protected bool HasPassenger => _dicPassengerDatas.Count >= 1;
}
