using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private readonly Dictionary<GameObject, PoolComponent> _dicPool = new();

    public void AddPool(GameObject poolObject, int initalPoolSize = 5, int maxPoolSize = 10, Transform poolParent = null)
    {
        if (poolParent == null)
            poolParent = transform;
        PoolComponent pool = new(poolObject, poolParent, initalPoolSize, maxPoolSize);
        _dicPool.Add(poolObject, pool);
    }

    public PoolComponent GetPool(GameObject poolObject)
    {
        if (!_dicPool.TryGetValue(poolObject, out PoolComponent poolComponent))
        {
            AddPool(poolObject);
            poolComponent = _dicPool[poolObject];
        }
        return poolComponent;
    }
    public void DelayReturnPoolObject(Action<GameObject> returnPoolObject, GameObject poolObject, float delayTime)
    {
        StartCoroutine(IEDelayReturnPoolObject(returnPoolObject, poolObject, delayTime));

    }
    public IEnumerator IEDelayReturnPoolObject(Action<GameObject> returnPoolObject, GameObject poolObject, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        returnPoolObject?.Invoke(poolObject);
    }
}
