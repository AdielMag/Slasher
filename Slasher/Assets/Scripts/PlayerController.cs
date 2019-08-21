using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class PlayerController : MonoBehaviour
{
    #region Singelton
    PlayerController instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public float horizontalMovementSpeed = 5, verticalMovementSpeed = 4;
    public float floorWidth = 2.5f;
    public float movementHorizntal = .5f,rotationHorizontal;
    public float rotationForce = 6, rotationSpeed = 3;

    int screenWidth;

    public ParticleSystem swordTrail;
    public GameObject[] slashes;

    Animator anim;

    void Start()
    {
        screenWidth = Screen.width;

        anim = GetComponent<Animator>();

        for(int i=0; i < slashes.Length; i++)
        {
            slashPlane[i] = slashes[i].transform.GetChild(0);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        movementHorizntal = Input.mousePosition.x / screenWidth;
        rotationHorizontal = Input.GetAxis("Mouse X");
#endif
        if (Input.touchCount > 0)
        {
            movementHorizntal = Input.GetTouch(0).position.x / screenWidth;
            rotationHorizontal = Input.GetTouch(0).deltaPosition.x / 65;
        }
        else
        {
#if !UNITY_EDITOR
            rotationHorizontal = 0;
#endif
        }

    }

    void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
        ObjectiveIdentification();
    }

    void MovePlayer()
    {
        Vector3 targetPos = new Vector3(Mathf.Lerp(-floorWidth, floorWidth, movementHorizntal),
            transform.position.y, transform.position.z + verticalMovementSpeed);

        transform.position = Vector3.Lerp(transform.position,targetPos, Time.deltaTime * horizontalMovementSpeed);
    }

    Vector3 targetRotation = Vector3.zero;
    void RotatePlayer()
    {
        targetRotation += Vector3.up * rotationHorizontal * rotationForce;
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * rotationSpeed);

        transform.rotation = Quaternion.Euler(targetRotation);
    }

  
    Vector3 halfExtents = new Vector3(.7f, 1.3f, 2.4f);
    Transform targetObjective;
    void ObjectiveIdentification()
    {
        if (Physics.BoxCast(transform.position + Vector3.up * 1.2f, halfExtents,
            transform.forward, out RaycastHit hit, transform.rotation, 1.3f))
        {
            if (hit.transform.tag == "Obj" && hit.transform != targetObjective)
            {
                targetObjective = hit.transform;

                anim.SetInteger("Randomizer", Random.Range(1, 3));
                anim.SetBool("Slash", true);
            }
        }
    }

    public void Slash(int slashNum)
    {
        swordTrail.Stop();

        slashes[slashNum - 1].SetActive(true);


        if (targetObjective)
        {
            if (!Physics.Raycast(targetObjective.position, targetObjective.forward, 5f))
                anim.SetBool("Slash", false);

            StartCoroutine(DestroyObjective(slashNum,targetObjective.gameObject));
        }
        else
            anim.SetBool("Slash", false);

        ComboCounter.instance.AddSlashToCombo();

        StartCoroutine(StartSwordTrail());
    }

    public Material dissolveMat;
    Transform[] slashPlane = new Transform[4];
    void Slice(int sliceNum,GameObject objectToSlice )
    {
        SlicedHull hull = objectToSlice.Slice(slashPlane[sliceNum - 1].position, slashPlane[sliceNum - 1].up);

        if (hull != null)
        {
            Rigidbody firstHalf = hull.CreateLowerHull(objectToSlice).AddComponent<Rigidbody>();
            Rigidbody secondHalf = hull.CreateUpperHull(objectToSlice).AddComponent<Rigidbody>();

            //firstHalf.GetComponent<Renderer>().materials = dissolveMat;
            //secondHalf.GetComponent<Renderer>().materials = dissolveMat;

            firstHalf.useGravity = false;
            secondHalf.useGravity = false;
            Vector3 firstHalfCenter = firstHalf.GetComponent<Renderer>().bounds.center;
            Vector3 secondHalfCenter = secondHalf.GetComponent<Renderer>().bounds.center;
            firstHalf.AddForce((firstHalfCenter - secondHalfCenter) * 10, ForceMode.Impulse);
            secondHalf.AddForce((secondHalfCenter - firstHalfCenter) * 10, ForceMode.Impulse);

            objectToSlice.SetActive(false);
        }
    }

    IEnumerator StartSwordTrail()
    {
        yield return new WaitForSeconds(.25f);
        swordTrail.Play();
    }
    IEnumerator DestroyObjective(int slashNum,GameObject obj)
    {
        yield return new WaitForSeconds(.05f);
        Slice(slashNum, obj);
        targetObjective = null;
    }

    public void Footstep(int right)
    {
        return;
        Ray ray = new Ray(anim.GetBoneTransform(right == 1 ? HumanBodyBones.RightFoot : HumanBodyBones.LeftFoot).position, -Vector3.up);

        if (Physics.Raycast(ray, out RaycastHit hit))
            ObjectPooler.instance.SpawnFromPool("LightningFootstep", hit.point + Vector3.up * .2f, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Obj":
                Debug.Log("Lost");
                break;
            case "Shield":
                other.gameObject.SetActive(false);
                break;
        }
    }
}
