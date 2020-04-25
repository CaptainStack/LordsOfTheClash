using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : Mirror.NetworkBehaviour
{
    public float totalMana;
    public float manaRechargeTime;
    public int playerHandSize;

    public Text manaText;
    public Text handText;
    public Text deckText;
    public Button cardButton1;
    public Button cardButton2;
    public Text cardButton1Text;
    public Text cardButton2Text;
    public CursorScript playerCursor;
    
    [Mirror.SyncVar]
    public float currentMana;
    public int cardSelected; //player input on button click determines which card is selected
    float timer;
    ApplicationStateManager applicationStateManager;

    // Used for networking
    private Mirror.NetworkIdentity networkIdentity;

    public List<Hero> heroes = new List<Hero>();
    public List<Card> playerDeck = new List<Card>();
    public List<Card> playerHand = new List<Card>(); //Cards in player's hand

    void Start()
    {
        applicationStateManager = FindObjectOfType<ApplicationStateManager>();
        networkIdentity = gameObject.GetComponent<Mirror.NetworkIdentity>();
        if (!networkIdentity)
            networkIdentity = gameObject.AddComponent<Mirror.NetworkIdentity>();

        playerCursor = GetComponentInChildren<CursorScript>();

        FillDeck();
        FillHand();

        if (!isLocalPlayer)
        {
            // Disable all non-local player canvas UI elements
            gameObject.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

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

        cardButton1Text.text = playerHand[0].cardName;
        cardButton2Text.text = playerHand[1].cardName;

        HandleInput();
    }

    void HandleInput()
    {
        if (!applicationStateManager.pauseMenuOn) //makes it pressing "Fire1" to unpause doesn't also make you use a spell.
        {
            if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Fire1"))
            {
                if (playerCursor.collidingWithButton)
                {
                    cardSelected = playerCursor.collidingButton.GetComponent<ButtonNumber>().cardNumber;
                }
                else
                {
                    UseCard(playerCursor.cursorPosition);
                }
            }

            if (Input.GetButtonDown("Card1"))
            {
                cardSelected = 0;
            }

            if (Input.GetButtonDown("Card2"))
            {
                cardSelected = 1;
            }
            
            SwitchSelectedCard();
        }
    }

    private void SwitchSelectedCard() //use bumpers to change selected card
    {
        if (Input.GetButtonDown("NextCard"))
        {
            if (cardSelected == playerHandSize - 1)
            {
                cardSelected = 0;
            }
            else
            {
                cardSelected += 1;
            }
        }
        if (Input.GetButtonDown("PreviousCard"))
        {
            if (cardSelected != 0)
            {
                cardSelected -= 1;
            }
            else
            {
                cardSelected = playerHandSize - 1;
            }
        }
    }

    void HighlightSelectedCard() //Highlights card currently selected
    {
        if (cardSelected == 0 && !applicationStateManager.pauseMenuOn)
        {
            cardButton1.Select(); //Selects button
            cardButton1.OnSelect(null); //highlights button
        }
        else if (cardSelected == 1 && !applicationStateManager.pauseMenuOn)
        {
            cardButton2.Select();
            cardButton2.OnSelect(null);
        }
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
    public void UseCard(Vector2 position) 
    {
        // Ignore clicks over UI elements and check mana
        if (!EventSystem.current.IsPointerOverGameObject() && !playerCursor.GetComponent<CursorScript>().collidingWithButton && currentMana >= playerHand[cardSelected].manaCost)
        {
            currentMana -= playerHand[cardSelected].manaCost;

            Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, Camera.main.nearClipPlane));
            point.z = 0f;

            Faction faction = isServer ? Faction.Friendly : Faction.Enemy;
            CmdDoCardAction(cardSelected, (Vector2)point, faction);
            RemoveCardFromHand();
            DrawCardFromDeck();
        }
    }

    [Mirror.Command]
    void CmdDoCardAction(int cardIndexInHand, Vector2 position, Faction faction)
    {
        cardSelected = cardIndexInHand;
        playerHand[cardSelected].DoCardAction(position, faction);
        RemoveCardFromHand();
        DrawCardFromDeck();
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
