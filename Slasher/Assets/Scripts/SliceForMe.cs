using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class SliceForMe : MonoBehaviour
{

    public GameObject objectToSlice; // non-null
    
    public Material[] dissolveMat; // non-null

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
           // Slice(transform.position, transform.up, crossSectionMaterial);
            SlicedHull hull = Slice(transform.position, transform.up);
            if(hull != null)
            {
                Rigidbody firstHalf = hull.CreateLowerHull(objectToSlice).AddComponent<Rigidbody>();
                Rigidbody secondHalf = hull.CreateUpperHull(objectToSlice).AddComponent<Rigidbody>();

                firstHalf.GetComponent<Renderer>().materials = dissolveMat;
                secondHalf.GetComponent<Renderer>().materials = dissolveMat;

                firstHalf.useGravity = false;
                secondHalf.useGravity = false;
                Vector3 firstHalfCenter = firstHalf.GetComponent<Renderer>().bounds.center;
                Vector3 secondHalfCenter = secondHalf.GetComponent<Renderer>().bounds.center;
                firstHalf.AddForce(Vector3.up * 2 + (firstHalfCenter - secondHalfCenter) * 10, ForceMode.Impulse);
                secondHalf.AddForce(Vector3.up * 2 + (secondHalfCenter - firstHalfCenter) * 10, ForceMode.Impulse);

                
                objectToSlice.SetActive(false);
            }
        }
    }
    public SlicedHull Slice(Vector3 planeWorldPosition, Vector3 planeWorldDirection)
    {
        return objectToSlice.Slice(planeWorldPosition, planeWorldDirection);
    }

}
