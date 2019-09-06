using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    #region Singelton
    static public LoadingScreen instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Enter()
    {
        anim.SetTrigger("Enter");
    }

    public void Done()
    {
        anim.SetTrigger("Done");
    }
}
