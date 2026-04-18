using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerData 
{
    public CreatureFSM _passenger;
    public int _passengerCount = 0;

    public PassengerData(CreatureFSM passenger, int passengerCount)
    {
        _passenger = passenger;
        _passengerCount = passengerCount;
    }
}
