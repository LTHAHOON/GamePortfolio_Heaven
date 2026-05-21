using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IPoolComponent
{
}
public class PoolComponent<T> : IPoolComponent where T : UnityEngine.Object 
{
    private int _initialPoolSize = 5;
    private int _maxPoolSize = 10;
    private T _poolObjPrefab;
    private Transform _poolParent;
    private Stack<T> _poolStack = new();
    public PoolComponent(T poolObjPrefab, Transform parent = null, int initalPoolSize = 5, int maxPoolSize = 10)
    {
        _poolObjPrefab = poolObjPrefab;
        _initialPoolSize = initalPoolSize;
        _maxPoolSize = maxPoolSize;
        _poolParent = parent;
        if (parent)
        {
            SetUpPool();
        }
    }

    private void SetUpPool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            T poolObject = UnityEngine.Object.Instantiate(_poolObjPrefab, _poolParent);
            _poolStack.Push(poolObject);

            SetActivePoolObject(poolObject, false);
        }
    }
    
    private void SetActivePoolObject(T poolObject, bool active)
    {
        if(poolObject is GameObject gameObj)
        {
            gameObj.SetActive(active);
        }
        else if(poolObject is MonoBehaviour monoObj)
        {
            monoObj.gameObject.SetActive(active);
        }
    }

    public T PopPoolObject()
    {
        if (_poolStack.Count <= 0)
        {
            T newPoolObject = UnityEngine.Object.Instantiate(_poolObjPrefab, _poolParent);
            return newPoolObject;
        }
        T poolObject = _poolStack.Pop();
        SetActivePoolObject(poolObject, true);
        return poolObject;
    }

    public void ReturnPoolObject(T poolObject, float delayTime)
    {
        if (!poolObject) return;
        if (delayTime > 0)
        {
            PoolManager.Instance.DelayReturnPoolObject(ReturnPoolObject, poolObject, delayTime);
        }
        else
        {
            ReturnPoolObject(poolObject);
        }
    }
    public void ReturnPoolObject(T poolObject)
    {
        if (!poolObject) return;
        if (_poolStack.Contains(poolObject)) return;
        if (_poolStack.Count < _maxPoolSize)
        {
            _poolStack.Push(poolObject);
            SetActivePoolObject(poolObject, false);
        }
        else
        {
            UnityEngine.Object.Destroy(poolObject);
        }
    }

}
