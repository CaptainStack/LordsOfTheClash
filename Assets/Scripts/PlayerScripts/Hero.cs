using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero
{
    public int hp;
    public int heroDeckSize;
    public List<Card> heroCards;
    //public int heroHandSize;



    void Start()
    {

       //heroCards = new Card[heroDeckSize];
        //Card[] heroHand = new Card[heroHandSize]; 
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void FillCards()
    {
        for (int i = 0; i < heroDeckSize; i++)
        {
            return;
        }
    }
}
