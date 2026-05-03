using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerData 
{
    public Unit Passenger { get; set; }
    public int PassengerCount { get; set; } = 0;

    public PassengerData(Unit passenger, int passengerCount)
    {
        Passenger = passenger;
        PassengerCount = passengerCount;
    }
}
