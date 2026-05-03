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

    protected void AddPassengerInData(IPassenger passenger, int passengerCount)
    {
        if (passenger is Unit unit)
        {
            if(_dicPassengerDatas.ContainsKey(unit.ID))
            {
                _dicPassengerDatas[unit.ID].PassengerCount += passengerCount;
            }
            else
            {
                _dicPassengerDatas.Add(unit.ID, new PassengerData(unit, passengerCount));
            }
        }
    }


    protected void RemovePassengerInData(CreatureController creatureController)
    {
        if(_dicPassengerDatas.ContainsKey(creatureController.ID))
        {
            _dicPassengerDatas.Remove(creatureController.ID);
        }
    }
    protected Unit[] GetPassengers()
    {
        return _dicPassengerDatas.Values.Select(v => v.Passenger).ToArray();
    }
    protected void ClearPassengerDatas()
    {
        _dicPassengerDatas.Clear();
    }
    protected int GetPassengerCountInData(long id)
    {
        return _dicPassengerDatas[id].PassengerCount;
    }
    public int AllPassengerCount => _dicPassengerDatas.Values.Sum(v => v.PassengerCount);
    protected bool HasPassenger => _dicPassengerDatas.Count >= 1;
}
