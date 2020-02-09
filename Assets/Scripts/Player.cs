using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float totalMana;
    public float manaRechargeTime;
    public int totalHeroes = 1;
    public int playerDeckSize;

    float currentMana;
    float timer;


    void Start()
    {
        Hero[] heroes = new Hero[totalHeroes];
        Card[] deck = new Card[playerDeckSize];
    }


    void Update()
    {
        ManaRegen();
    }

    void ManaRegen()
    {
        if (currentMana < totalMana)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                currentMana++;
                timer = manaRechargeTime;
            }
        }
    }

    void UseCard()
    {

    }

    void ShuffleDeck() //pull cards from heroes then shuffle
    {

    }

    void DrawCard()
    {

    }
}
