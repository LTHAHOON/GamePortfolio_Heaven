using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDecal : MonoBehaviour
{
    private void Start()
    {
          //StartCoroutine(IEReturnObject(3f, gameObject));
    }

    IEnumerator IEReturnObject(float delayTime, GameObject poolingObject)
    {
        yield return new WaitForSeconds(delayTime);
        Destroy(poolingObject);
    }
}
