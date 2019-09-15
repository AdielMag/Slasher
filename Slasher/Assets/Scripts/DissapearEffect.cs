using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissapearEffect : MonoBehaviour
{
    float a = 10, b = 1, c = 2;

    float localTime, startingTime;

    private void OnEnable()
    {
        startingTime = Time.time;
    }

    private void Start()
    {
        startingTime = Time.time;

        c = transform.localScale.x;
    }
    void FixedUpdate()
    {
        localTime = Time.time - startingTime;

        transform.localScale =Vector3.one * (-a * localTime * localTime + b * localTime + c);

        if (transform.localScale.x <= 0)
            Destroy(gameObject);
    }

}
