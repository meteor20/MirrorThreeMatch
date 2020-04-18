using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BombCountTimeBar : MonoBehaviour
{
    public TMPro.TextMeshProUGUI  text;
    public GameObject  countdownPivot;
    public  bool hideImageInFullTime=true ;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Obstacle>())
            GetComponent<Obstacle>().OnTakeDamage += UpdateTimeBar;
        if (hideImageInFullTime)
            countdownPivot.gameObject.SetActive(false);
    }

    private void UpdateTimeBar(float ratio)
    {
        text.text = (GetComponent<Obstacle>().healthValue * ratio).ToString ();//"还剩" +  +"次";

        countdownPivot.gameObject.SetActive(ratio>0);
    }

}
