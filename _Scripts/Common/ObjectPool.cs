using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private uint initPoolsize;
    [SerializeField] private PooledObject prefab;
    Queue<PooledObject> queue;

    private void Awake() {
         SetUpPool();
    }

    private void SetUpPool()
    {
        queue = new Queue<PooledObject>();
        PooledObject instance = null;
        for (int i = 0; i < initPoolsize; ++i)
        {
            instance = Instantiate(prefab, transform.position, Quaternion.identity);
            instance.Pool = this;
            instance.gameObject.SetActive(false);
            queue.Enqueue(instance);
        }
    }

    public PooledObject GetPoolObj()
    {
        if (queue.Count == 0)
        {
            PooledObject instance = Instantiate(prefab, transform.position, Quaternion.identity);
            instance.Pool = this;
            instance.gameObject.SetActive(true);

            return instance;
        }
        PooledObject nextinstance = queue.Dequeue();
        nextinstance.gameObject.SetActive(true);
        return nextinstance;
    }
    public void ReturnPool(PooledObject pooledObject)
    {
        pooledObject.transform.SetParent(this.transform);
        pooledObject.gameObject.SetActive(false);
        queue.Enqueue(pooledObject);
    }
}
