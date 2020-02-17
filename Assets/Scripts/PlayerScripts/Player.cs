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
    public Text handText;
    public Text deckText;
    
    public float currentMana;
    float timer;

    public List<Hero> heroes = new List<Hero>(); //need to find a way to fill this with heroes.
    List<Card> playerDeck = new List<Card>();
    List<Card> playerHand = new List<Card>(); //Cards in player's hand

    void Start()
    {
        FillDeck();
        FillHand();
    }


    void Update()
    {
        manaText.text = "Mana: " + currentMana.ToString();
        ManaRegen();

        handText.text = $"Hand: [";
        foreach(Card card in playerHand)
        {
            handText.text += $"{card.cardName} ({card.manaCost}), ";
        }
        handText.text += "]";

        deckText.text = $"Deck: [";
        foreach(Card card in playerDeck)
        {
            deckText.text += $"{card.cardName} ({card.manaCost}), ";
        }
        deckText.text += "]";
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

    //press button on keyboard to use card 
    //Make public to call from UI?
    public void UseCard() 
    {
        if (currentMana >= playerHand[0].manaCost)
        {
            currentMana -= playerHand[0].manaCost;
            playerHand[0].DoCardAction();
            RemoveCardFromHand();
            DrawCardFromDeck();
        }
    }

    void RemoveCardFromHand()
    {
        playerDeck.Add(playerHand[0]);
        playerHand.RemoveAt(0);
    }
    void DrawCardFromDeck()
    {
        playerHand.Add(playerDeck[0]);
        playerDeck.RemoveAt(0);
    }

    void FillDeck()
    {
        foreach (Hero hero in heroes)
        {
            foreach (Card card in hero.heroDeck)
            {
                playerDeck.Add(card);
            }
        }
    }

    void FillHand()
    {
        while (playerHand.Count < playerHandSize)
        {
            DrawCardFromDeck();
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < playerDeck.Count; i++)
        {
            Card temp = playerDeck[i];
            int placementIndex = Random.Range(0, playerDeck.Count - 1);
            playerDeck[i] = playerDeck[placementIndex];
            playerDeck[placementIndex] = temp;
        }
    }
}
