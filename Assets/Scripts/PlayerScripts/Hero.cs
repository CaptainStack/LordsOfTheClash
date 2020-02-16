using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public int hp;
    public List<Card> heroDeck;
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

    public void FillCards()
    {
        for (int i = 0; i < heroDeck.Count; i++)
        {
            return;
        }
    }
}
