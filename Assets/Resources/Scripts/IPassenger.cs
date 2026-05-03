using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPassenger
{
    void OnUnboard(Vector3 targetPosition, Vector3 enemyPosition);
}
