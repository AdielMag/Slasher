﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class PlayerController : MonoBehaviour
{
    #region Singelton
    public static PlayerController instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public float horizontalMovementSpeed = 5, verticalMovementSpeed = 4;
    float origVerMovSpeed, origHorizMovSpeed;
    public float floorWidth = 2.5f;
    public float movementHorizntal = .5f, rotationHorizontal;
    public float rotationForce = 6, rotationSpeed = 3;

    int screenWidth;

    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public PlayerItemsController pItemsCon;
    GameManager gMan;

    void Start()
    {
        screenWidth = Screen.width;

        anim = GetComponent<Animator>();
        gMan = GameManager.instance;
        pItemsCon = GetComponent<PlayerItemsController>();

        origVerMovSpeed = verticalMovementSpeed;
        origHorizMovSpeed = horizontalMovementSpeed;

        for (int i = 0; i < pItemsCon.slashes.Length; i++)
        {
            // slashPlane[i] = slashes[i].transform.GetChild(0);
        }

        LoadPlayerItems();
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
        if (!gMan.playing)
            return;

        MovePlayer();
        RotatePlayer();
        ObjectiveIdentification();
    }

    void MovePlayer()
    {
        //verticalMovementSpeed = Mathf.Lerp(verticalMovementSpeed, gMan.isTouching ? origVerMovSpeed : origVerMovSpeed * .6f, Time.deltaTime * 5);

        Vector3 targetPos = new Vector3(Mathf.Lerp(-floorWidth, floorWidth, movementHorizntal)
            , transform.position.y, transform.position.z + verticalMovementSpeed);

        //horizontalMovementSpeed = Mathf.Lerp(horizontalMovementSpeed,gMan.isTouching ? origHorizMovSpeed : origHorizMovSpeed * .3f, Time.deltaTime * 5);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * horizontalMovementSpeed);
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
    int pointsMultiplier;
    public LayerMask slashLayerMask;
    void ObjectiveIdentification()
    {
        if (Physics.BoxCast(transform.position + Vector3.up * 1.2f, halfExtents,
            transform.forward, out RaycastHit hit, transform.rotation, 1.3f, slashLayerMask))
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
        pItemsCon.currentSwordTrail.Stop();

        pItemsCon.slashes[slashNum - 1].SetActive(true);

        if (targetObjective)
        {
            if (!Physics.Raycast(targetObjective.position, targetObjective.forward, out RaycastHit hit, 5f, slashLayerMask) || hit.transform.tag == "Undestructable")
                anim.SetBool("Slash", false);

            targetObjective.tag = "Untagged";
            StartCoroutine(DestroyObjective(slashNum, targetObjective.gameObject));
        }
        else
            anim.SetBool("Slash", false);

        switch (targetObjective.GetComponent<Objective>().type)
        {
            case SpawnInformation.ObjectType.Regular:
                pointsMultiplier = 2;
                break;
            case SpawnInformation.ObjectType.Shooter:
                pointsMultiplier = 3;
                break;
            case SpawnInformation.ObjectType.BonusPoints:
                pointsMultiplier = 4;
                break;
        }

        ComboCounter.instance.AddSlashToCombo(pointsMultiplier);

        StartCoroutine(StartSwordTrail());

        gMan.AddTime(.12f);
    }

    public Material slashMaterial;
    Transform[] slashPlane = new Transform[4];
    void Slice(int sliceNum, GameObject objectToSlice)
    {
        SlicedHull hull = objectToSlice.Slice(pItemsCon.slashes[sliceNum - 1].transform.GetChild(0).position,
            pItemsCon.slashes[sliceNum - 1].transform.GetChild(0).up);

        if (hull != null)
        {
            GameObject firstHalf = hull.CreateLowerHull(objectToSlice, slashMaterial);
            GameObject secondHalf = hull.CreateUpperHull(objectToSlice, slashMaterial);

            #region Rigidbody 
            Rigidbody firstHalfRb = firstHalf.AddComponent<Rigidbody>();
            Rigidbody secondHalfRb = secondHalf.AddComponent<Rigidbody>();

            firstHalfRb.useGravity = false;
            secondHalfRb.useGravity = false;

            Vector3 firstHalfCenter = firstHalf.GetComponent<Renderer>().bounds.center;
            Vector3 secondHalfCenter = secondHalf.GetComponent<Renderer>().bounds.center;

            firstHalfRb.AddForce((firstHalfCenter - secondHalfCenter) * 4, ForceMode.Impulse);
            secondHalfRb.AddForce((secondHalfCenter - firstHalfCenter) * 4, ForceMode.Impulse);
            #endregion


            //firstHalf.AddComponent<DissolveEffect>();
            //secondHalf.AddComponent<DissolveEffect>();

            firstHalf.AddComponent<DissapearEffect>();
            secondHalf.AddComponent<DissapearEffect>();

            objectToSlice.SetActive(false);
        }
    }

    IEnumerator StartSwordTrail()
    {
        yield return new WaitForSeconds(.25f);
        pItemsCon.currentSwordTrail.Play();

    }
    IEnumerator DestroyObjective(int slashNum, GameObject obj)
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
        if (!gMan.playing)
            return;

        switch (other.tag)
        {
            case "Obj":
                LostGame();
                break;
            case "Undestructable":
                LostGame();
                break;
            case "Shield":
                other.gameObject.SetActive(false);
                break;
        }
    }

    bool firstRun = true;
    public void ResetPlayer()
    {
        pItemsCon.currentSwordTrail.Play();

        transform.position = Vector3.zero;

        if (firstRun)
            firstRun = false;
        else
            anim.SetTrigger("Live");
    }

    public void LostGame()
    {
        anim.SetTrigger("Death");

        gMan.LostGame();
        /* 
         * player stops moving and responding to touch
         * Ui menu opens
         * Points (coins?) are saved.
         */
    }

    void LoadPlayerItems()
    {
        JsonDataManager.instance.LoadData();

        StartCoroutine (pItemsCon.EquipItems());
    }
}