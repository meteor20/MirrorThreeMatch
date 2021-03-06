﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour,IImmoveable 
{
    public IntVector2 position;
    public float healthValue=2;
    private float currentHealth;
    public UnityAction<float> OnTakeDamage;
    // Start is called before the first frame update
    public virtual  void Start()
    {
        currentHealth = healthValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool  TakeDamage(int value=1)
    {
        currentHealth -= value;
        OnTakeDamage?.Invoke(currentHealth /healthValue );
        if (currentHealth >0)
        {
            return false ;
        }
        else
        {
            return true ;
        }
    }

    public virtual  void Death()
    {
        Destroy(Instantiate(GetComponent <Piece >(). DestroyVFX,transform.position, Quaternion.identity), 1f);
        Destroy(GetComponent<Obstacle>().gameObject);
       
    }
}
