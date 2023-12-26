using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSize : MonoBehaviour
{
    public GameObject[] envPrefabs;
    public float minDisX=-4f,maxDisX=4f;
    void Start()
    {
        int createNum=Random.Range(1,3);
        for(int i=0;i<createNum;++i)
        {
            GenerateEnvItem();
        }

    }

    public void GenerateEnvItem()
    {
        int temp=Random.Range(1,envPrefabs.Length);
        Quaternion randomRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f),Random.Range(0f, 360f)); 
        GameObject envItem = Instantiate(envPrefabs[temp], transform.position, randomRotation);
        envItem.transform.SetParent( transform);
        envItem.transform.localScale=new Vector3(Random.Range(3f,8f),Random.Range(3f,8f),Random.Range(3f,8f));
        envItem.transform.localPosition+=new Vector3(Random.Range(minDisX,maxDisX),Random.Range(-4f,4f),Random.Range(-4f,4f));

    }

   
}
