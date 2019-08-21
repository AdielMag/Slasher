using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class MySlicer : MonoBehaviour
{

    public GameObject objectToSlice; // non-null

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Slice(transform.position, transform.up);
        }
    }

    public SlicedHull Slice(Vector3 planeWorldPosition, Vector3 planeWorldDirection)
    {
        return objectToSlice.Slice(planeWorldPosition, planeWorldDirection);
    }



}
