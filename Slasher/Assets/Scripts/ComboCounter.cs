using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter : MonoBehaviour
{
    int comboCounter;

    public float timeToAddForSlash = 2;
    float lastTimeSlashed;

    public float lerpMultiplier = 1;

    Quaternion origRot;
    Vector3 origScale;
    Color origCol;

    Animator anim;
    RectTransform rect;
    Text text;
    Shadow shadow;

    static public ComboCounter instance;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        rect = GetComponent<RectTransform>();
        text = GetComponent<Text>();
        shadow = GetComponent<Shadow>();

        origRot = rect.rotation;
        origScale = rect.localScale;
        origCol = shadow.effectColor;
    }


    void Update()
    {
        if (Time.time > lastTimeSlashed)
        {
            anim.SetBool("InCombo", false);
            ResetCombo();
        }
        else
        {
            rect.rotation = Quaternion.Lerp(rect.rotation, origRot, Time.deltaTime * lerpMultiplier);
            rect.localScale = Vector3.Lerp(rect.localScale, origScale, Time.deltaTime * lerpMultiplier);
            shadow.effectColor = Color.Lerp(shadow.effectColor, origCol, Time.deltaTime * (lerpMultiplier /2));
        }
    }


    void ResetCombo()
    {
        PointsCounter.instance.AddPoints(comboCounter * 5);
        comboCounter = 0;
    }

    public void AddSlashToCombo()
    {
        anim.SetBool("InCombo", true);
        comboCounter++;
        lastTimeSlashed = Time.time + timeToAddForSlash;
        text.text = "+" + (comboCounter * 5).ToString();

        rect.localScale *= 1.24f;
        rect.localRotation *= Quaternion.Euler(0, Random.Range(-5, 5), Random.Range(-5, 5));
        shadow.effectColor = new Color(shadow.effectColor.r, shadow.effectColor.g + 0.1f, shadow.effectColor.b + 0.1f);
    }
}
