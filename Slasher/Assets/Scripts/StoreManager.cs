using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    #region Singelton
    public static StoreManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public int coins;
    [Space]

    public Transform myCamera;
    Animator objectDisplayer;
    Quaternion origRot;

    public Animator storeAC, mainMenuAc;
    public Transform buyIconsParent;
    public Text coinsIndicator, priceText;

    Transform charactersParent, weaponsParent;
    
    bool weapons = true;

    int weaponItemNum, charactersItemNum;
   // [HideInInspector]
   // public int equippedWeaponItemNum, equippedCharacterItemNum;

    JsonDataManager jDataMan;
    LoadingScreen loadScr;
    PlayerController pCon;
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
        loadScr = LoadingScreen.instance;
        pCon = PlayerController.instance;
        jDataMan = JsonDataManager.instance;

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
        LoadStoreData();
        yield return new WaitForSeconds(.3f);
        OpenMenu("Weapons");
        myCamera.rotation = Quaternion.Euler(0, -90, 0);
        objectDisplayer.gameObject.SetActive(true);
        storeAC.gameObject.SetActive(true);
        storeAC.SetBool("On", true);
        mainMenuAc.SetTrigger("Close");

        objectDisplayer.SetBool("Off", false);

        coinsIndicator.text = coins.ToString();

        GameManager.instance.inGameScoreIndicator.gameObject.SetActive(false);
        GameManager.instance.timeLeftIndicator.gameObject.SetActive(false);
    }

    IEnumerator CloseStoreDelayed()
    {
        loadScr.Done();
        loadScr.Enter();
        SaveStoreData();
        yield return new WaitForSeconds(.3f);
        myCamera.rotation = origRot;
        objectDisplayer.gameObject.SetActive(false);
        storeAC.gameObject.SetActive(false);
        storeAC.SetBool("On", false);
        mainMenuAc.SetTrigger("Open");

        objectDisplayer.SetBool("Off", true);

        GameManager.instance.inGameScoreIndicator.gameObject.SetActive(true);
        GameManager.instance.timeLeftIndicator.gameObject.SetActive(true);

        StartCoroutine (pCon.pItemsCon.EquipItems());
    }

    public void NextItem()
    {
        if (weapons)
        {
            if (weaponItemNum == weaponsParent.childCount - 1)
                return;
            weaponsParent.GetChild(weaponItemNum).gameObject.SetActive(false);
            weaponItemNum++;
            weaponsParent.GetChild(weaponItemNum).gameObject.SetActive(true);
        }
        else
        {
            if (charactersItemNum == charactersParent.childCount - 1)
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
            objectDisplayer.SetBool("Weapons", true);
            objectDisplayer.SetBool("Characters", false);
        }
        else
        {
            weapons = false;
            objectDisplayer.SetBool("Weapons", false);
            objectDisplayer.SetBool("Characters", true);
        }
        CheckItemConditions();
    }

    void CheckItemConditions()
    {
        StoreItem itemToCheck;
        if (weapons)
            itemToCheck = weaponsParent.GetChild(weaponItemNum).GetComponent<StoreItem>();
        else
            itemToCheck = charactersParent.GetChild(charactersItemNum).GetComponent<StoreItem>();

        foreach (Transform obj in buyIconsParent)
        {
            obj.gameObject.SetActive(false);
        }

        if (itemToCheck.equipped)
            buyIconsParent.GetChild(2).gameObject.SetActive(true);
        else if (itemToCheck.bought)
            buyIconsParent.GetChild(1).gameObject.SetActive(true);
        else
        {
            buyIconsParent.GetChild(0).gameObject.SetActive(true);

            priceText.text = itemToCheck.price.ToString();
        }
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
                if (weapons)
                {
                    if (weaponItemNum != jDataMan.storeData.EquippedWeapon)
                        weaponsParent.GetChild(jDataMan.storeData.EquippedWeapon).GetComponent<StoreItem>().equipped = false;
                    jDataMan.storeData.EquippedWeapon = weaponItemNum;
                }
                else
                {
                    if (charactersItemNum != jDataMan.storeData.EquippedCharacter)
                        charactersParent.GetChild(jDataMan.storeData.EquippedCharacter).GetComponent<StoreItem>().equipped = false;
                    jDataMan.storeData.EquippedCharacter = charactersItemNum;
                }
            }
            else
                currentItem.bought = true;
        }
        CheckItemConditions();

        coinsIndicator.text = coins.ToString();
    }

    void LoadStoreData()
    {
        jDataMan.LoadData();

        // Characters.
        for (int i = 0; i < charactersParent.childCount; i++)
        {
            if(i != 0)
                charactersParent.GetChild(i).GetComponent<StoreItem>().bought = false;
            charactersParent.GetChild(i).GetComponent<StoreItem>().equipped = false;
        }
        for (int i = 0; i < jDataMan.storeData.CharactersBought.Length; i++)
            charactersParent.GetChild(jDataMan.storeData.CharactersBought[i]).GetComponent<StoreItem>().bought = true;
        charactersParent.GetChild(jDataMan.storeData.EquippedCharacter).GetComponent<StoreItem>().equipped = true;
        // Weapons.
        for (int i = 0; i < weaponsParent.childCount; i++)
        {
            if (i != 0)
                weaponsParent.GetChild(i).GetComponent<StoreItem>().bought = false;
            weaponsParent.GetChild(i).GetComponent<StoreItem>().equipped = false;
        }
        for (int i = 0; i < jDataMan.storeData.WeaponsBought.Length; i++)
            weaponsParent.GetChild(jDataMan.storeData.WeaponsBought[i]).GetComponent<StoreItem>().bought = true;
        weaponsParent.GetChild(jDataMan.storeData.EquippedWeapon).GetComponent<StoreItem>().equipped = true;

        coins = jDataMan.storeData.Coins;
    }

    void SaveStoreData()
    {
        // Characters.
        List<int> characterList = new List<int>();
        for (int i = 0; i < charactersParent.childCount; i++)
        {
            if (charactersParent.GetChild(i).GetComponent<StoreItem>().bought)
            {
                characterList.Add(i);
                //jDataMan.storeData.CharactersBought[charactersBoughtCounter] = i;

                if (charactersParent.GetChild(i).GetComponent<StoreItem>().equipped)
                    jDataMan.storeData.EquippedCharacter = i;
            }
        }
        jDataMan.storeData.CharactersBought = characterList.ToArray();

        // Weapons.
        List<int> weaponsList = new List<int>();
        for (int i = 0; i < weaponsParent.childCount; i++)
        {
            if (weaponsParent.GetChild(i).GetComponent<StoreItem>().bought)
            {
                weaponsList.Add(i);

                if (weaponsParent.GetChild(i).GetComponent<StoreItem>().equipped)
                    jDataMan.storeData.EquippedWeapon = i;
            }
        }
        jDataMan.storeData.WeaponsBought = weaponsList.ToArray();

        jDataMan.storeData.Coins = coins;

        jDataMan.SaveData();
    }
}
