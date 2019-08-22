using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    // public Color effectColor = Color.cyan;
    public float effectSpeed = 1;

    float dissolveAmount = .2f;

    int effectAmountId;

    Material[] mat;

    private void Start()
    {
        mat = GetComponent<Renderer>().materials;

        effectAmountId = Shader.PropertyToID("_EffectAmount");
    }

    void Update()
    {
        dissolveAmount += effectSpeed / 100;

        for (int i = 0; i < mat.Length; i++)
        {
            mat[i].SetFloat(effectAmountId, dissolveAmount);

            if (dissolveAmount >= 1)
                Destroy(gameObject);
        }
    }
}
