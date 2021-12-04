using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] ObjectPool pool;
    [SerializeField] GameObject prefab;
    [SerializeField] int size;

    private IObjectPool _objectPooler;

    void Start()
    {
        _objectPooler = pool;
        _objectPooler.CreatePool(transform, prefab, size);

        StartCoroutine(SpawnObject());
    }

    private IEnumerator SpawnObject()
    {
        _objectPooler.SpawnFromPool();
        int rand = Random.Range(0, 2);
        yield return new WaitForSeconds(rand);
        StartCoroutine(SpawnObject());
    }
}
