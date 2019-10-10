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

    StoreManager sMan;
    JsonDataManager jDataMan;

    private void Start()
    {
        sMan = StoreManager.instance;
    }

    public IEnumerator EquipItems()
    {
        PlayerController pCon = GetComponent<PlayerController>();
        jDataMan = JsonDataManager.instance;

        // Destroy Character Object (player weapons destroyes with it)
        foreach (Transform obj in transform)
        {
            if (obj.tag == "Character")
                Destroy(obj.gameObject);
        }

        // Enable equipped character
        Instantiate(characters[jDataMan.storeData.EquippedCharacter], pCon.transform);

        yield return new WaitForEndOfFrame();

        pCon.anim.Rebind();

        SetCurrentWeaponParent();

        // Disable all weapons in the current weapons parent
        foreach (Transform obj in currentWeaponsParent)
        {
            obj.gameObject.SetActive(false);
        }

        // Enable equipped weapon and trail
        currentWeaponsParent.GetChild(jDataMan.storeData.EquippedWeapon).gameObject.SetActive(true);

        currentSwordTrail = currentWeaponsParent.GetChild(jDataMan.storeData.EquippedWeapon)
            .GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
    }

    void SetCurrentWeaponParent()
    {
        Transform characterHand = GetComponent<PlayerController>().anim.GetBoneTransform(HumanBodyBones.RightHand);

        GameObject obj = Instantiate(weaponsParentPrefab, characterHand);

        currentWeaponsParent = obj.transform.GetChild(0);
    }
}
