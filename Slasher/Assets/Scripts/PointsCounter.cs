using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsCounter : MonoBehaviour
{
    float tempPoints;
    public int points;

    Text text;

    #region Singelton
    static public PointsCounter instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    private void Start()
    {
        text = GetComponent<Text>();
    }

    private void FixedUpdate()
    {
        tempPoints += .01f;
        points = Mathf.RoundToInt(tempPoints);

        text.text = points.ToString();
    }

    public void AddPoints(int amount)
    {
        tempPoints += amount;
    }
}
