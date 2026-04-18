using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene01 : MonoBehaviour {

    public GameObject fadeScreenIn;
    public GameObject charYemaya;
    public GameObject TextBox;

    void Start()
    {
        StartCoroutine(EventStart());
    }

    IEnumerator EventStart()
    {
        yield return new WaitForSeconds(2);
        fadeScreenIn.SetActive(false);
        charYemaya.SetActive(true);
        //yield return new WaitForSeconds(2);
        //this is where our text function will be
        TextBox.SetActive(true);
        yield return new WaitForSeconds(2);
        
    }

}
