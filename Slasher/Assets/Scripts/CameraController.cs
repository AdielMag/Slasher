using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float cameraMovementSpeed = 4;

    Vector3 positionOffset;

    private void Start()
    {
        positionOffset = transform.position;   
    }

    private void FixedUpdate()
    {
        Vector3 targetPos = new Vector3(target.position.x /3.2f, transform.position.y, target.position.z + positionOffset.z);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * cameraMovementSpeed);
    }
}
