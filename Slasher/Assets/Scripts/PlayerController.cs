using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public ParticleSystem[] slashes;

    Animator anim;

    void Start()
    {
        screenWidth = Screen.width;

        anim = GetComponent<Animator>();
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
            rotationHorizontal = Input.GetTouch(0).deltaPosition.x / 30;
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
        if (slashes[slashNum - 1].isPlaying)
            slashes[slashNum - 1].Clear();
        slashes[slashNum - 1].Play();

        if (targetObjective)
        {
            if (!Physics.Raycast(targetObjective.position, targetObjective.forward, 5f))
                anim.SetBool("Slash", false);

            StartCoroutine(DestroyObjective(targetObjective.gameObject));
        }
        else
            anim.SetBool("Slash", false);

        ComboCounter.instance.AddSlashToCombo();

        StartCoroutine(StartSwordTrail());
    }

    IEnumerator StartSwordTrail()
    {
        yield return new WaitForSeconds(.25f);
        swordTrail.Play();
    }
    IEnumerator DestroyObjective(GameObject obj)
    {
        yield return new WaitForSeconds(.05f);
        obj.SetActive(false);
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
