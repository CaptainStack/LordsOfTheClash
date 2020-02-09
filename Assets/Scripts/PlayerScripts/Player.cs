using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float totalMana;
    public float manaRechargeTime;
    public int totalHeroes = 1;
    public int playerDeckSize;
    public int playerHandSize;
    public int heroDeckSize;

    float currentMana;
    float timer;

    List<Card> playerDeck = new List<Card>();
    List<Card> playerHand = new List<Card>(); //Cards in player's hand
    List<Hero> heroes = new List<Hero>(); //need to find a way to fill this with heroes.

    void Start()
    {

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

    void UseCard() //call DrawCard() at the end of this function?
    {

    }

    void DrawCard()
    {
        if (playerHand.Count < playerHandSize)
        {
            playerHand.Add(playerDeck[0]);
            playerDeck.RemoveAt(0);
        }
    }

      void FillDeck() //fill deck with cards from heroes' lists.
    {
        for (int i = 0; i <totalHeroes; i++) //loop through heroes
        {
            for (int j = 0; j < heroDeckSize; j++) //loop through a hero's deck
            { 
                playerDeck.Add(heroes[i].heroCards[j]);
            }
        }
    }

    void ShuffleDeck() //shuffle cards in deck (needs to be less shitty)
    {
        for (int i = 0; i < playerDeckSize - 1; i++)
        {
            Card temp = playerDeck[i];
            int index = Random.Range(0, playerDeckSize - 1);
            playerDeck[i] = playerDeck[index];
            playerDeck[index] = temp;
        }
    }
}
