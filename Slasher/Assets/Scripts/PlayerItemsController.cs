using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemsController : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject weaponsParentPrefab;

    public Transform currentWeaponsParent;
    public ParticleSystem currentSwordTrail;

    public GameObject[] slashes;

    Animator anim;
    PlayerController pCon;
    StoreManager sMan;

    private void Start()
    {
        anim = GetComponent<Animator>();
        pCon = PlayerController.instance;
        sMan = StoreManager.instance;
    }

    public void EquipItems()
    {
        // Destroy Character Object (player weapons destroyes with it)
        foreach (Transform obj in transform)
        {
            if (obj.tag == "Character")
                Destroy(obj.gameObject);
        }

        // Enable equipped character
        Instantiate(pCon.pItemsCon.characters[sMan.equippedCharacterItemNum], pCon.transform);

        anim.Rebind();
        SetCurrentWeaponParent();

        // Disable all weapons in the current weapons parent
        foreach (Transform obj in currentWeaponsParent)
        {
            obj.gameObject.SetActive(false);
        }

        // Enable equipped weapon and trail
        currentWeaponsParent.GetChild(sMan.equippedWeaponItemNum).gameObject.SetActive(true);

        currentSwordTrail = currentWeaponsParent.GetChild(sMan.equippedWeaponItemNum)
            .GetChild(0).GetChild(0).GetComponent<ParticleSystem>();

    }

    void SetCurrentWeaponParent()
    {
        //Animator currentCharacterAnimator = StoreManager.instance.playerCharacters.
        //    GetChild(JsonDataManager.instance.storeData.EquippedCharacter).GetComponent<Animator>();

        Transform characterHand = anim.GetBoneTransform(HumanBodyBones.RightHand);

        GameObject obj = Instantiate(weaponsParentPrefab, characterHand);

        currentWeaponsParent = obj.transform.GetChild(0);
    }
}
