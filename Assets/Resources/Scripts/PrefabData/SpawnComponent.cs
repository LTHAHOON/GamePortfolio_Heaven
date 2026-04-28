using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnComponent : MonoBehaviour
{
    [SerializeField]
    private float _spawnHeight = 5f;

    [SerializeField]
    private PrefabDatabase _prefabDatabase;

    private GameObject spawnPrefab;

    public GameObject GetSpawnPrefab(long id)
    {
        if(spawnPrefab == null)
        {
            spawnPrefab = _prefabDatabase.Get(id);
        }
        return spawnPrefab;
    }
    public float SpawnHeight => _spawnHeight;
}
