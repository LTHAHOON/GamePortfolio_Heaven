using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PassengerController : Unit
{
    //인스턴스 중심의 Passengers 데이터(Spawn된 상태)
    private List<Unit> _spawnedPassengerList = new();
    //Passenger Count 중심의 Passengers 데이터(Spawn되지 않은 상태)
    private readonly Dictionary<long, PassengerData> _dicUnSpawnedPassenger = new();
    public List<PassengerData> GetUnSpawnedPassengers()
    {
        return _dicUnSpawnedPassenger.Values.ToList();
    }
    public List<Unit> GetSpawnedPassengers()
    {
        return _spawnedPassengerList;
    }

    public void AddUnSpawnedPassenger(IPassenger passenger, int passengerCount)
    {
        if (passenger is Unit unit)
        {
            if (_dicUnSpawnedPassenger.ContainsKey(unit.ID))
            {
                _dicUnSpawnedPassenger[unit.ID].PassengerCount += 1;
            }
            else
            {
                _dicUnSpawnedPassenger.Add(unit.ID, new PassengerData(unit, passengerCount));
            }
        }
    }

    public void AddSpawnedPassenger(IPassenger passenger)
    {
        if (passenger is Unit unit)
        {
            if(!_spawnedPassengerList.Contains(unit))
            {
                _spawnedPassengerList.Add(unit);
            }
            
        }
    }
    public void AddSpawnedPassengers(List<IPassenger> passengers)
    {
        for (int i = 0; i < passengers.Count; i++)
        {
            AddSpawnedPassenger(passengers[i]);
        }
    }

    public void RemoveUnSpawnedPassenger(Unit passenger)
    {
        if(_dicUnSpawnedPassenger.ContainsKey(passenger.ID))
        {
            _dicUnSpawnedPassenger.Remove(passenger.ID);
        }
    }
    public void RemoveSpawnedPassenger(Unit passenger)
    {
        if(_spawnedPassengerList.Contains(passenger))
        {
            _spawnedPassengerList.Remove(passenger);
        }
    }
    public void ClearPassengerDatas()
    {
        _dicUnSpawnedPassenger.Clear();
        _spawnedPassengerList.Clear();
    }
    public int GetPassengerCountInData(long id)
    {
        return _dicUnSpawnedPassenger[id].PassengerCount;
    }
    public int AllPassengerCount => _dicUnSpawnedPassenger.Values.Sum(v => v.PassengerCount) + _spawnedPassengerList.Count;
    public bool HasPassenger => _dicUnSpawnedPassenger.Count > 0 ||_spawnedPassengerList.Count > 0;
}
