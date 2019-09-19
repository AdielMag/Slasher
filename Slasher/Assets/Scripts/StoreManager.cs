using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
   public static StoreManager instance;
    private void Awake()
    {
        instance = this;
    }


    public Transform myCamera;
    Animator objectDisplayer;
    Quaternion origRot;

    public Animator storeAC, mainMenuAc;

    bool weapons, characters;
    int weaponItemNum, charactersItemNum;

    LoadingScreen loadScr;
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
        loadScr = LoadingScreen.instance;

        objectDisplayer = myCamera.GetChild(2).GetComponent<Animator>();
        origRot = myCamera.rotation;
    }



    public void OpenStore()
    {
        StartCoroutine(OpenStoreDelayed());
    }

    public void CloseStore()
    {
        StartCoroutine(CloseStoreDelayed());
    }

    IEnumerator OpenStoreDelayed()
    {
        loadScr.Done();
        loadScr.Enter();
        yield return new WaitForSeconds(.3f);
        myCamera.rotation = Quaternion.Euler(0, -90, 0);
        objectDisplayer.gameObject.SetActive(true);
        storeAC.SetBool("On", true);
        mainMenuAc.SetTrigger("Close");

        objectDisplayer.SetBool("Off", false);

    }

    IEnumerator CloseStoreDelayed()
    {
        loadScr.Done();
        loadScr.Enter();
        yield return new WaitForSeconds(.3f);
        myCamera.rotation = origRot;
        objectDisplayer.gameObject.SetActive(false);
        storeAC.SetBool("On", false);
        mainMenuAc.SetTrigger("Open");

        objectDisplayer.SetBool("Off", true);

    }

    public void NextItem()
    {
        if (weapons) { }
        else
        { }
    }
    public void LastItem()
    {
    }

    public void OpenMenu(string menuType)
    {
        if (menuType == "Weapons")
        {
            weapons = true;
            characters = false;
            objectDisplayer.SetBool("Weapons",true);
            objectDisplayer.SetBool("Characters", false);
        }
        else
        {
            weapons = false;
            characters = true;
            objectDisplayer.SetBool("Weapons", false);
            objectDisplayer.SetBool("Characters", true);
        }
    }
    public void BuyItem() { }
}
