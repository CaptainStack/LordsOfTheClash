using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public int cardSelected; //player input on button click determines which card is selected
    float timer;
    

    public List<Hero> heroes = new List<Hero>();
    public List<Card> playerDeck = new List<Card>();
    public List<Card> playerHand = new List<Card>(); //Cards in player's hand

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
        // Ignore clicks over UI elements and check mana
        if (!EventSystem.current.IsPointerOverGameObject() && currentMana >= playerHand[cardSelected].manaCost)
        {
            currentMana -= playerHand[cardSelected].manaCost;
            playerHand[cardSelected].DoCardAction();
            RemoveCardFromHand();
            DrawCardFromDeck();
        }
    }

    void RemoveCardFromHand()
    {
        playerDeck.Add(playerHand[cardSelected]);
        playerHand.RemoveAt(cardSelected);
    }
    void DrawCardFromDeck()
    {
        playerHand.Add(playerDeck[cardSelected]);
        playerDeck.RemoveAt(cardSelected);
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

    public void SelectCard0() //on button click select card 0
    {
        cardSelected = 0;
        Debug.Log(playerHand[cardSelected].name);
    }

    public void SelectCard1() //on button click select card 1
    {
        cardSelected = 1;
        Debug.Log(playerHand[cardSelected].name);
    }

}
