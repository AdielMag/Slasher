using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public bool shooter;
    [HideInInspector]
    public bool shootRight;

    public SpawnInformation.ObjectType type;

    public string destructionEffectName;

    GameManager gMan;
    ObjectPooler objPooler;

    private void Start()
    {
        gMan = GameManager.instace;
        objPooler = ObjectPooler.instance;

        if (shooter)
        {
            float x = transform.position.x;

            if (x >= 2)
                shootRight = false;
            else if (x <= -2)
                shootRight = true;
            else
                shootRight = Random.Range(1f, 2f) == 1 ? true : false;

            InvokeRepeating("LaunchProjectile", 0, 3);
        }
    }

    void LaunchProjectile()
    {
        Vector3 pos = transform.position + transform.right * (shootRight ? 1 : -1);
        Vector3 dir = shootRight ? transform.right : -transform.right;

        GameObject projectile = objPooler.SpawnFromPool("ShooterProjectile", pos, Quaternion.identity);

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
