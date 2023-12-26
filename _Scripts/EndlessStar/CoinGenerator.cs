using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    public ObjectPool coinPool;
    // public List<GameObject> tempCoins;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        coinPool = GameObject.FindObjectOfType<ObjectPool>();
        int gridx=Random.Range(0,3);
        CreateCoin(gridx);
    }

    public void CreateCoin(int gridx)
    {
        PooledObject pooledObject = coinPool.GetPoolObj();
        pooledObject.transform.position = new Vector3(transform.position.x+gridx*3.5f, transform.position.y , transform.position.z);
        // tempCoins.Add(pooledObject.gameObject);
    }


}
