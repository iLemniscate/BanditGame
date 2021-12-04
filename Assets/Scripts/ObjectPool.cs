using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour, IObjectPool
{
    private int _size = 0;
    private Transform _parent;

    public void CreatePool(Transform parent, GameObject prefab, int size)
    {
        // Create Pool and Instantiate All Objects

        // Set gameObject.GetComponent<Bandit>().Die method with DespawnToPool
    }

    public void DespawnToPool(GameObject gameObject)
    {
        // Disable gameObject and put back to Pool
    }

    public GameObject SpawnFromPool()
    {
        // Take gameObject from Pool and Enable it

        // Call gameObject.GetComponent<Bandit>().Live() method to activate Bandit/Enemy
        return null;
    }

}
