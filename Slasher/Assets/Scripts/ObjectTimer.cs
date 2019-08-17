using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTimer : MonoBehaviour,IPooledObject
{
    public float time;
    float spawnedTime;

    public void OnObjectSpawn()
    {
    }

    void OnEnable()
    {
        spawnedTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > spawnedTime + time)
            gameObject.SetActive(false);
    }
}
