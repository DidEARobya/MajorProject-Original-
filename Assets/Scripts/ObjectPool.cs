using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    int poolSize;
    GameObject pooledPrefab;
    Transform parent;

    List<GameObject> poolList;
    public ObjectPool(int capacity, GameObject pooledObject, Transform parent)
    {
        poolSize = capacity;
        pooledPrefab = pooledObject;

        poolList = new List<GameObject>();
        poolList.Capacity = capacity;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefab = GameObject.Instantiate(pooledPrefab);
            prefab.SetActive(false);
            prefab.transform.SetParent(parent, false);
            poolList.Add(prefab);
        }
    }

    public void ClearExcess()
    {
        if (poolList.Count > poolSize)
        {
            for (int i = poolSize; i < poolList.Count; i++)
            {
                GameObject.Destroy(poolList[i]);
            }
        }
    }
    public GameObject SpawnObject(Vector2 position, Quaternion rotation)
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            if (poolList[i].activeInHierarchy == false)
            {
                poolList[i].transform.position = position;
                poolList[i].transform.rotation = rotation;

                poolList[i].SetActive(true);
                return poolList[i];
            }
        }

        GameObject prefab = GameObject.Instantiate(pooledPrefab);
        prefab.SetActive(true);
        prefab.transform.SetParent(parent, false);
        poolList.Add(prefab);

        return prefab;
    }

    public void DespawnObject(GameObject obj)
    {
        for (int i = 0; i < poolList.Count; i++)
        {
            if (obj == poolList[i])
            {
                poolList[i].SetActive(false);
                return;
            }
        }
    }
}