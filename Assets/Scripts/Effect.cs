﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Effect : MonoBehaviour
{
    [Header("Character Object:")] //only need to fill out these values if it's a Summon card
    public Character summon;

    public float spellDamage;
    public float healPower;
    public float radius;

    
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

   public void Action()
    {
        Debug.Log(spellDamage);
    }
}
