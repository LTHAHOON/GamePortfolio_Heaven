using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPassenger
{
    bool SuccessBoard { get; }
    void OnBoard();
    void OnUnboard(Vector3 targetPosition);
}
