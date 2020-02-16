using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float totalMana;
    public float manaRechargeTime;
    public int playerHandSize;

    public Text manaText;
    
    public float currentMana;
    float timer;

    public List<Hero> heroes = new List<Hero>(); //need to find a way to fill this with heroes.
    List<Card> playerDeck = new List<Card>();
    List<Card> playerHand = new List<Card>(); //Cards in player's hand

    void Start()
    {
        FillDeck();
        DrawCard();
    }


    void Update()
    {
        manaText.text = "Mana: " + currentMana.ToString();
        ManaRegen();
        UseCard();
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

    void UseCard() //press button on keyboard to use card
        //Make public to call from UI?
    {
        if (Input.GetKeyDown("space"))
        {
            if (currentMana >= playerHand[0].manaCost)
            {
                currentMana -= playerHand[0].manaCost; //spend mana to use the card
                playerHand[0].CardAction(); //execute the card you are using
                playerDeck.Add(playerHand[0]); //add card back into deck
                playerHand.RemoveAt(0); //remove card from hand
                DrawCard();
            }
        }

    }

    void DrawCard() //remove card from position 0 of the deck and add it to player's hand
    {
        if (playerHand.Count < playerHandSize)
        {
            playerHand.Add(playerDeck[0]);
            playerDeck.RemoveAt(0);
        }
    }

      void FillDeck() //fill deck with cards from heroes' lists.
    {
        foreach(Hero hero in heroes)
        {
            foreach (Card card in hero.heroDeck)
            {
                playerDeck.Add(card);
            }
        }

    }

    void ShuffleDeck() //shuffle cards in deck (needs to be less shitty)
    {
        for (int i = 0; i < playerDeck.Count; i++)
        {
            Card temp = playerDeck[i];
            int index = Random.Range(0, playerDeck.Count - 1);
            playerDeck[i] = playerDeck[index];
            playerDeck[index] = temp;
        }
    }
}
