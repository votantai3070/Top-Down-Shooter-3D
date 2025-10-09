using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int poolSize = 5;

    Queue<GameObject> objectPool;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        objectPool = new Queue<GameObject>();
        CreateInitialPool();
    }

    private void CreateInitialPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    private void CreateNewObject()
    {
        GameObject obj = Instantiate(objectPrefab, transform);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }

    public GameObject Get()
    {
        if (objectPool.Count == 0)
            CreateNewObject();

        GameObject bulletToGet = objectPool.Dequeue();

        // reset trail trước khi bật lại
        var trail = bulletToGet.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.Clear(); // xoá toàn bộ dữ liệu vẽ cũ
        }

        bulletToGet.SetActive(true);
        bulletToGet.transform.parent = null;

        return bulletToGet;
    }

    public void ReturnPool(GameObject obj)
    {
        obj.SetActive(false);
        objectPool.Enqueue(obj);
        obj.transform.parent = transform;
    }
}
