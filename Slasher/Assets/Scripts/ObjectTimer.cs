using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTimer : MonoBehaviour,IPooledObject
{
    public float time;
    float spawnedTime;

    Rigidbody rgb;

    private void Start()
    {
        rgb = GetComponent<Rigidbody>();
    }

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

    private void OnDisable()
    {
        if (rgb)
        {
            rgb.velocity = Vector3.zero;
            rgb.angularVelocity = Vector3.zero;
        }
    }
}
