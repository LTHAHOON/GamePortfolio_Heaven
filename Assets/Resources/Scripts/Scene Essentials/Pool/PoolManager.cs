using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    private readonly Dictionary<UnityEngine.Object, IPoolComponent> _dicPool = new();
    private readonly Dictionary<int, IPoolComponent> _dicPool_ID = new();
    public void AddPool<T>(T poolObject, int initalPoolSize = 5, int maxPoolSize = 10, Transform poolParent = null) where T : UnityEngine.Object
    {
        if (poolParent == null)
            poolParent = transform;
        PoolComponent<T> pool = new(poolObject, poolParent, initalPoolSize, maxPoolSize);
        _dicPool.Add(poolObject, pool);
    }
    public void AddPoolUsingID<T>(int id, T poolObject, int initalPoolSize = 5, int maxPoolSize = 10, Transform poolParent = null) where T : UnityEngine.Object
    {
        if (poolParent == null)
            poolParent = transform;
        PoolComponent<T> pool = new(poolObject, poolParent, initalPoolSize, maxPoolSize);
        _dicPool_ID.Add(id, pool);
    }
    public void RemovePool<T>(T poolObject) where T : UnityEngine.Object
    {
        if(_dicPool.ContainsKey(poolObject))
        {
            _dicPool.Remove(poolObject);
        }
    }
    public void RemovePoolUsingID(int id)
    {
        if (_dicPool_ID.ContainsKey(id))
        {
            _dicPool_ID.Remove(id);
        }
    }

    public bool TryGetPool<IN, OUT>(IN poolObject, out OUT poolComponent) where IN : UnityEngine.Object where OUT : class
    {
        if (!_dicPool.TryGetValue(poolObject, out IPoolComponent poolComp))
        {
            AddPool(poolObject);
            poolComponent = _dicPool[poolObject] as OUT;
            return false;
        }
        poolComponent = poolComp as OUT;
        return true;
    }

    public bool TryGetPoolUsingID<OUT>(int id, out OUT poolComponent) where OUT : class
    {
        if (!_dicPool_ID.TryGetValue(id, out IPoolComponent poolComp))
        {
            poolComponent = null;
            return false;
        }
        poolComponent = poolComp as OUT;
        return true;
    }

    public void DelayReturnPoolObject<T>(Action<T> returnPoolObject, T poolObject, float delayTime)
    {
        StartCoroutine(IEDelayReturnPoolObject(returnPoolObject, poolObject, delayTime));

    }
    public IEnumerator IEDelayReturnPoolObject<T>(Action<T> returnPoolObject, T poolObject, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        returnPoolObject?.Invoke(poolObject);
    }
}
