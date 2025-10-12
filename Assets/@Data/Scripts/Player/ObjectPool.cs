using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private int poolSize = 5;

    Dictionary<GameObject, Queue<GameObject>> poolDict = new();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void InitializeNewPool(GameObject prefab)
    {
        poolDict[prefab] = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab);
        }
    }
    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObj = Instantiate(prefab, transform);
        newObj.AddComponent<PooledObject>().originalPrefab = prefab;
        newObj.SetActive(false);

        poolDict[prefab].Enqueue(newObj);
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (!poolDict.ContainsKey(prefab))
            InitializeNewPool(prefab);

        if (poolDict[prefab].Count == 0)
            CreateNewObject(prefab);

        GameObject objectToGet = poolDict[prefab].Dequeue();

        // reset trail trước khi bật lại
        if (objectToGet.TryGetComponent<TrailRenderer>(out var trail))
        {
            trail.Clear(); // xoá toàn bộ dữ liệu vẽ cũ
        }

        objectToGet.SetActive(true);
        objectToGet.transform.parent = null;

        return objectToGet;
    }

    #region Return To Pool

    public void ReturnToPool(GameObject objectToReturn)
    {
        GameObject originalPrefab = objectToReturn.GetComponent<PooledObject>().originalPrefab;

        objectToReturn.SetActive(false);
        objectToReturn.transform.parent = transform;

        poolDict[originalPrefab].Enqueue(objectToReturn);
    }

    public void DelayReturnToPool(GameObject objectToReturn, float delay = .001f)
    {
        StartCoroutine(DelayReturn(delay, objectToReturn));
    }

    private IEnumerator DelayReturn(float delay, GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);

        ReturnToPool(objectToReturn);
    }

    #endregion
}
