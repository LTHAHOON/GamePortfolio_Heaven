using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolAttackDecal : MonoBehaviour
{
    [SerializeField]
    private GameObject poolingObjectPrefab;

    private static Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();
    private void Awake()
    {
        Initialize(25);    // 풀링 초기 갯수
    }

    private void Initialize(int initCount)
    {
        for (int i = 0; i < initCount; i++)
        {
            poolingObjectQueue.Enqueue(CreateDecalPrefab());
        }
    }
    private GameObject CreateDecalPrefab()
    {

        GameObject newObj = Instantiate(poolingObjectPrefab);
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;

    }

    public void PoolingWork(Vector3 position, Quaternion rotation)
    {

        GameObject decalPrefab = GetDecalPrefab(position, rotation);
        ReturnDecalPrefab(decalPrefab, 3f);


    }

    public GameObject GetDecalPrefab(Vector3 position, Quaternion rotation)
    {

        if (poolingObjectQueue.Count > 0)
        {
            GameObject obj = poolingObjectQueue.Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            GameObject newObj = CreateDecalPrefab();
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;
            newObj.gameObject.SetActive(true);
            return newObj;
        }

    }
    public void ReturnDecalPrefab(GameObject obj, float delayTime)
    {

        StartCoroutine(IEReturnObject(delayTime, obj));

    }

    IEnumerator IEReturnObject(float delayTime, GameObject poolingObject)
    {

        yield return new WaitForSeconds(delayTime);
        
        if (transform.childCount > 25)
        {
            Destroy(poolingObject);
        }
        else
        {
            poolingObject.gameObject.SetActive(false);
            poolingObjectQueue.Enqueue(poolingObject);
        }

    }

}
