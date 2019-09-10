using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public bool shooter;
    [HideInInspector]
    public bool shootRight;

    public string destructionEffectName;

    GameManager gMan;
    ObjectPooler objPooler;

    private void Start()
    {
        gMan = GameManager.instace;
        objPooler = ObjectPooler.instance;

        if (shooter)
            InvokeRepeating("LaunchProjectile", 0, 3);
    }

    void LaunchProjectile()
    {
        Vector3 pos = transform.position + transform.right * (shootRight ? 1 : -1);
        Vector3 dir = shootRight ? transform.right : -transform.right;

        GameObject projectile = objPooler.SpawnFromPool("shooterProjectile", pos, Quaternion.identity);

        projectile.GetComponent<Rigidbody>().AddForce(dir * 100, ForceMode.Acceleration);
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

            tag = "Obj";
        }

        if (shooter)
            CancelInvoke("LaunchProjectile");
    }
}
