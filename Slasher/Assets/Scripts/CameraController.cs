using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float cameraMovementSpeed = 4;

    Vector3 positionOffset;

    public Transform floor;

    GameManager gMan;

    private void Start()
    {
        positionOffset = transform.position;
        gMan = GameManager.instace;
    }

    private void FixedUpdate()
    {
        if (!gMan.playing)
        {
            return;
        }


        Vector3 targetPos = new Vector3(target.position.x /3.2f, transform.position.y, target.position.z + positionOffset.z);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * cameraMovementSpeed);

        floor.position = new Vector3(floor.position.x, 0, transform.position.z + 110);
    }

    public void DisableAnimator()
    {
        GetComponent<Animator>().enabled = false;
    }

}
