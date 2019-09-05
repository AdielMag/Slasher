using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public string destructionEffectName;

    GameManager gMan;

    private void Start()
    {
        gMan = GameManager.instace;
    }

    private void OnDisable()
    {
        if (Time.time > 1)
        {
            if (destructionEffectName != "")
                ObjectPooler.instance.SpawnFromPool(destructionEffectName, transform.position, Quaternion.identity);

            if (gMan)
                gMan.currentSpawnedObj--;
            else
                GameManager.instace.currentSpawnedObj--;
        }
    }
}
