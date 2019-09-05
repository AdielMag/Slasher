using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsCounter : MonoBehaviour
{
    float tempPoints;
    public int points;

    Text text;
    GameManager gMan;

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
        gMan = GameManager.instace;
    }

    private void FixedUpdate()
    {
        if (gMan.playing)
        {
            tempPoints += .01f;
            points = Mathf.RoundToInt(tempPoints);
        }

        text.text = points.ToString();
    }

    public void AddPoints(int amount)
    {
        tempPoints += amount;
    }

    public void Reset()
    {
        tempPoints = points = 0;
    }
}
