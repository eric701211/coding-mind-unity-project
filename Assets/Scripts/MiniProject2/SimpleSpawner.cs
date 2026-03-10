using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpawner : MonoBehaviour
{
    public float startWaitTime = 4;
    public float inBetweenWaitTime = 10;
    public GameObject spawningPrefab;

    void Start()
    {
        InvokeRepeating("Spawn", startWaitTime,inBetweenWaitTime);
    }

    void Spawn()
    {
        Instantiate(spawningPrefab,this.transform);
    }
}
