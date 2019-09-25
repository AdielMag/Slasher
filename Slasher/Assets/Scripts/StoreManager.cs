using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
   public static StoreManager instance;
    private void Awake()
    {
        instance = this;
    }

    public int coins;
    [Space]

    public Transform myCamera;
    Animator objectDisplayer;
    Quaternion origRot;

    public Animator storeAC, mainMenuAc;
    public Text buyOrEquip;

    Transform charactersParent, weaponsParent;
    bool weapons = true, characters;

    int weaponItemNum, charactersItemNum;
    int equippedWeaponItemNum = 0, equippedCharacterItemNum = 0;

    LoadingScreen loadScr;
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
        loadScr = LoadingScreen.instance;

        objectDisplayer = myCamera.GetChild(2).GetComponent<Animator>();
        origRot = myCamera.rotation;

        weaponsParent = objectDisplayer.transform.GetChild(2);
        charactersParent = objectDisplayer.transform.GetChild(3);

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
        OpenMenu("Weapons");
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
        if (weapons)
        {
            if (weaponItemNum == weaponsParent.childCount -1)
                return;
            weaponsParent.GetChild(weaponItemNum).gameObject.SetActive(false);
            weaponItemNum++;
            weaponsParent.GetChild(weaponItemNum).gameObject.SetActive(true);
        }
        else
        {
            if (charactersItemNum == charactersParent.childCount -1)
                return;
            charactersParent.GetChild(charactersItemNum).gameObject.SetActive(false);
            charactersItemNum++;
            charactersParent.GetChild(charactersItemNum).gameObject.SetActive(true);
        }

        CheckItemConditions();

    }
    public void LastItem()
    {
        if (weapons)
        {
            if (weaponItemNum == 0)
                return;
            weaponsParent.GetChild(weaponItemNum).gameObject.SetActive(false);
            weaponItemNum--;
            weaponsParent.GetChild(weaponItemNum).gameObject.SetActive(true);
        }
        else
        {
            if (charactersItemNum == 0)
                return;
            charactersParent.GetChild(charactersItemNum).gameObject.SetActive(false);
            charactersItemNum--;
            charactersParent.GetChild(charactersItemNum).gameObject.SetActive(true);
        }

        CheckItemConditions();

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
        CheckItemConditions();
    }

    void CheckItemConditions( )
    {
        StoreItem itemToCheck;
        if (weapons)
            itemToCheck = weaponsParent.GetChild(weaponItemNum).GetComponent<StoreItem>();
        else
            itemToCheck = charactersParent.GetChild(charactersItemNum).GetComponent<StoreItem>();

        if(itemToCheck.equipped)
            buyOrEquip.text = "Equipped";
        else if (itemToCheck.bought)
            buyOrEquip.text = "Equip";
        else
            buyOrEquip.text = "Buy";

    }

    public void BuyItem()
    {
        StoreItem currentItem;
        if (weapons)
            currentItem = weaponsParent.GetChild(weaponItemNum).GetComponent<StoreItem>();
        else
            currentItem = charactersParent.GetChild(charactersItemNum).GetComponent<StoreItem>();

        if (coins >= currentItem.price)
        {
            if (currentItem.bought)
            {
                currentItem.equipped = true;
                if (weapons && weaponItemNum != equippedWeaponItemNum)
                    weaponsParent.GetChild(equippedWeaponItemNum).GetComponent<StoreItem>().equipped = false;
                else if(characters && charactersItemNum != equippedCharacterItemNum )
                    charactersParent.GetChild(equippedCharacterItemNum).GetComponent<StoreItem>().equipped = false;


                EquipItem();
            }
            else
                currentItem.bought = true;
        }
        CheckItemConditions();
    }

    public Transform playerCharacters, playerWeapons;
    void EquipItem()
    {
        if (weapons)
            playerWeapons.GetChild(equippedWeaponItemNum).gameObject.SetActive(false);
        else
            playerCharacters.GetChild(equippedCharacterItemNum).gameObject.SetActive(false);

        equippedCharacterItemNum = weapons ? weaponItemNum : charactersItemNum;

        if (weapons)
            playerWeapons.GetChild(equippedWeaponItemNum).gameObject.SetActive(true);
        else
            playerCharacters.GetChild(equippedCharacterItemNum).gameObject.SetActive(true);
    }
}
